using Library.LibraryUtilities.Extensions;
using Library.McKessonDWInterface.DataModel;
using Library.McKessonDWInterface.DataSetMapper;
using Library.McKessonDWInterface.McKessonOracleInterface;
using Library.TenTenInterface.DataModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

namespace Library.McKessonDWInterface
{
    public class McKessonDWInterfaceImp : IMcKessonDWInterface
    {
        private readonly IMcKessonOracleInterface _mcKessonOracleInterface;
        private readonly IDataSetMapper _dataSetMapper;
        private readonly IOptions<McKessonDWConfig> _config;
        private readonly ILogger<McKessonDWInterfaceImp> _logger;

        public McKessonDWInterfaceImp(
            IMcKessonOracleInterface mcKessonOracleInterface,
            IDataSetMapper dataSetMapper,
            IOptions<McKessonDWConfig> config,
            ILogger<McKessonDWInterfaceImp> logger)
        {
            _mcKessonOracleInterface = mcKessonOracleInterface ?? throw new ArgumentNullException(nameof(mcKessonOracleInterface));
            _dataSetMapper = dataSetMapper ?? throw new ArgumentNullException(nameof(dataSetMapper));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        /// <inheritdoc />
        public async Task WriteListToFileAsync<T>(
            List<T> downloadList,
            string downloadFileName,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            CancellationToken c)
        {
            await this.WriteListToFileAsync<T>(
               downloadList,
               downloadFileName,
               hasHeaderRow,
               delimiter,
               textQualifier,
               makeExtractWhenNoData,
               false,
               c).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task WriteListToFileAsync<T>(
            List<T> downloadList,
            string downloadFileName,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            bool shouldAppendToExistingFile,
            CancellationToken c)
        {
            downloadFileName = Path.Combine(_config.Value.DataFileExportPath, downloadFileName);
            delimiter = delimiter ?? string.Empty;
            textQualifier = textQualifier ?? string.Empty;
            var output = new StringBuilder();
            var fields = new Collection<string>();
            Type elementType = typeof(T);

            _logger.LogInformation("Output data in typed List<T> to output file [{0}].", downloadFileName);

            if (File.Exists(downloadFileName) && !shouldAppendToExistingFile)
            {
                _logger.LogWarning("Output file [{0}] already exists and shouldAppendToExistingFile=false, so now deleting this existing output file.", downloadFileName);
                File.Delete(downloadFileName);
            }

            //If the file already exists and shouldAppendToExistingFile=true, then do not output the header row with the append.
            hasHeaderRow = (
                hasHeaderRow &&
                shouldAppendToExistingFile &&
                File.Exists(downloadFileName) ?
                    false : hasHeaderRow);

            using (var writerOutputData = new StreamWriter(downloadFileName, shouldAppendToExistingFile))
            {
                if (hasHeaderRow)
                {
                    foreach (var propInfo in elementType.GetProperties())
                    {
                        //Get the column label from the ExportHeaderColumnLabelAttribute if it exists, otherwise use the property name.
                        string columnLabel = propInfo.Name;
                        var attributes = propInfo.GetCustomAttributes(false);
                        var columnMapping = attributes.FirstOrDefault(a => a.GetType() == typeof(ExportHeaderColumnLabelAttribute));
                        if (columnMapping != null)
                        {
                            var mapsto = columnMapping as ExportHeaderColumnLabelAttribute;

                            if (mapsto != null)
                                columnLabel = mapsto.Name;
                        }

                        fields.Add(string.Format("{0}{1}{0}", textQualifier, columnLabel));
                    }

                    await writerOutputData.WriteLineAsync(string.Join(delimiter, fields.ToArray())).ConfigureAwait(false);
                    fields.Clear();
                    hasHeaderRow = false;
                }

                foreach (T record in downloadList)
                {
                    fields.Clear();

                    foreach (var propInfo in elementType.GetProperties())
                    {
                        if ((propInfo.GetValue(record, null) ?? DBNull.Value).GetType() == typeof(System.String))
                        {
                            fields.Add(string.Format("{0}{1}{0}", textQualifier, (propInfo.GetValue(record, null) ?? DBNull.Value).ToString()));
                        }
                        else
                        {
                            fields.Add((propInfo.GetValue(record, null) ?? DBNull.Value).ToString() ?? string.Empty);
                        }
                    }

                    await writerOutputData.WriteLineAsync(string.Join(delimiter, fields.ToArray())).ConfigureAwait(false);
                }
            }

            if (!makeExtractWhenNoData && downloadList.Count == 0)
            {
                _logger.LogInformation("List has zero rows and job is set to not create an empty file, so now deleting this empty output file.");
                if (File.Exists(downloadFileName))
                    File.Delete(downloadFileName);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<OmnisysClaimRow>> GetOmnisysClaimsAsync(DateOnly runDate, CancellationToken c)
        {
            Dictionary<string, string> queryLiterals = new()
            {
                { ":RunDate", runDate.ToSqlDateLiteral() }
            };
            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithLiteralsToDataSetAsync(
                CommandType.Text,
                "SelectOmnisysClaim",
                queryLiterals,
                c);

            return _dataSetMapper.MapOmnisysClaim(ds);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<NewTagPatientGroupsRow>> GetNewTagPatientGroupsAsync(DateOnly runDate, CancellationToken c)
        {
            OracleParameter[] oracleParams = [
                new OracleParameter("RunDate", OracleDbType.Date, ParameterDirection.Input)
            ];
            oracleParams[0].Value = runDate.ToDateTime(new TimeOnly(0));
            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithParamsToDataSetAsync(
                CommandType.Text,
                "SelectNewTagPatientGroups",
                oracleParams,
                c);
            return _dataSetMapper.MapNewTagPatientGroups(ds);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<RxErpRow>> GetRxErpAsync(DateOnly runDate, CancellationToken c)
        {
            OracleParameter[] oracleParams = [
                new OracleParameter("RunDate", OracleDbType.Date, ParameterDirection.Input)
            ];
            oracleParams[0].Value = runDate.ToDateTime(new TimeOnly(0));
            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithParamsToDataSetAsync(
                CommandType.Text,
                "SelectRxErp",
                oracleParams,
                c);
            return _dataSetMapper.MapRxErp(ds);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SoldDetailRow>> GetSoldDetailAsync(DateOnly runDate, CancellationToken c)
        {
            OracleParameter[] oracleParams = [
                new OracleParameter("RunDate", OracleDbType.Date, ParameterDirection.Input)
            ];
            oracleParams[0].Value = runDate.ToDateTime(new TimeOnly(0));
            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithParamsToDataSetAsync(
                CommandType.Text,
                "SelectSoldDetail",
                oracleParams,
                c);
            return _dataSetMapper.MapSoldDetail(ds);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<StoreInventoryHistoryRow>> GetStoreInventoryHistoryAsync(DateOnly runDate, CancellationToken c)
        {
            OracleParameter[] oracleParams = [
                new OracleParameter("RunDate", OracleDbType.Date, ParameterDirection.Input)
            ];
            oracleParams[0].Value = runDate.ToDateTime(new TimeOnly(0));
            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithParamsToDataSetAsync(
                CommandType.Text,
                "SelectStoreInventoryHistory",
                oracleParams,
                c);
            return _dataSetMapper.MapStoreInventoryHistory(ds);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<TatRawDataRow>> GetTatRawDataAsync(DateOnly startDate, DateOnly endDate, string tatTarget, CancellationToken c)
        {
            Dictionary<string, string> queryLiterals = new Dictionary<string, string>();
            queryLiterals.Add(":Start_Date", startDate.ToSqlDateLiteral());
            queryLiterals.Add(":End_Date", endDate.ToSqlDateLiteral());
            queryLiterals.Add(":TAT_Target", tatTarget.ToSqlStringLiteral());

            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithLiteralsToDataSetAsync(
                CommandType.Text,
                "SelectTurnaroundTime",
                queryLiterals,
                c);

            return _dataSetMapper.MapTatRawData(ds);
        }

        /// <inheritdoc />
        public IEnumerable<TatDetailsRow> GetTatDetailsReport(IEnumerable<TatRawDataRow> dataRows, string tatTarget)
        {
            return _dataSetMapper.PopulateTatDerivedDetails(dataRows, tatTarget);
        }

        /// <inheritdoc />
        public IEnumerable<TatSummaryRow> DeriveTatSummaryReport(IEnumerable<TatDetailsRow> dataRows, string tatTarget)
        {
            return _dataSetMapper.PopulateTatDerivedSummary(dataRows, tatTarget);
        }

        /// <inheritdoc />
        public IEnumerable<TatSummaryMaxRxRow> DeriveTatSummaryMaxRxReport(IEnumerable<TatSummaryRow> summaryRows)
        {
            return _dataSetMapper.GetOneRxWithinAnOrderHavingLargestDaysNetTat(summaryRows);
        }

        public async Task<IEnumerable<TpcDataRow>> GetThirdPartyClaimsBaseAsync(DateOnly runDate, CancellationToken c)
        {
            Dictionary<string, string> queryLiterals = new()
            {
                { ":RunDate", runDate.ToSqlDateLiteral() }
            };

            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithLiteralsToDataSetAsync(
                CommandType.Text,
                "SelectThirdPartyClaimsBase",
                queryLiterals,
                c);

            return _dataSetMapper.MapThirdPartyClaimBase(ds);
        }

        public async Task<decimal?> GetThirdPartyClaimsAcquisitionCostAsync(decimal? rxFillSeq, decimal? rxRecordNum, decimal? fillStateKey, CancellationToken c)
        {
            Dictionary<string, string> queryLiterals = new()
            {
                { ":RX_FILL_SEQ", rxFillSeq.ToString() ?? string.Empty},
                { ":RX_RECORD_NUM", rxRecordNum.ToString() ?? string.Empty},
                { ":FILL_STATE_KEY",  fillStateKey.ToString() ?? string.Empty}
            };

            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithLiteralsToDataSetAsync(
                CommandType.Text,
                "SelectThirdPartyClaimsLookupAcquisitionCost",
                queryLiterals,
                c);

            IEnumerable<decimal?> updatedData = _dataSetMapper.MapThirdPartyClaimsAcquisitionCost(ds);

            return updatedData.FirstOrDefault();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SureScriptsMedicalHistoryRawDataRow>> GetSureScriptsMedicalHistoryRawDataAsync(DateOnly runFor, CancellationToken c)
        {
            Dictionary<string, string> queryLiterals = new Dictionary<string, string>();
            queryLiterals.Add(":RunDate", runFor.ToSqlDateLiteral());

            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithLiteralsToDataSetAsync(
                CommandType.Text,
                "SelectSureScriptsMedicalHistory",
                queryLiterals,
                c);

            return _dataSetMapper.MapSureScriptsMedicalHistoryRawData(ds);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SureScriptsPhysicianNotificationLetterRawDataRow>> GetSureScriptsPhysicianNotificationLettersRawDataAsync(DateOnly runFor, CancellationToken c)
        {
            Dictionary<string, string> queryLiterals = new Dictionary<string, string>();
            queryLiterals.Add(":RunDate", runFor.ToSqlDateLiteral());

            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithLiteralsToDataSetAsync(
                CommandType.Text,
                "SelectSureScriptsPhysicianNotificationLetters",
                queryLiterals,
                c);

            return _dataSetMapper.MapSureScriptsPhysicianNotificationLettersRawData(ds);
        }

        /// <inheritdoc />
        public IEnumerable<SureScriptsMedicalHistoryReportRow> DeriveSureScriptsMedicalHistoryReport(IEnumerable<SureScriptsMedicalHistoryRawDataRow> medicalHistoryRawDataRows)
        {
            return _dataSetMapper.MapSureScriptsMedicalHistoryRawDataToFinalReport(medicalHistoryRawDataRows);
        }

        /// <inheritdoc />
        public IEnumerable<SureScriptsPhysicianNotificationLetterReportRow> DeriveSureScriptsPhysicianNotificationLetterReport(IEnumerable<SureScriptsPhysicianNotificationLetterRawDataRow> pnlRawDataRows, string storeState)
        {
            return _dataSetMapper.MapSureScriptsPhysicianNotificationLetterRawDataToFinalReport(pnlRawDataRows, storeState);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WorkloadBalanceRow>> GetWorkloadBalanceRowsAsync(DateOnly runDate, CancellationToken c)
        { 
            Dictionary<string, string> queryLiterals = new()
            {
                { ":RunDate", runDate.ToSqlDateLiteral() }
            };

            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithLiteralsToDataSetAsync(
                CommandType.Text,
                "SelectWorkloadBalance",
                queryLiterals,
                c);

            return _dataSetMapper.MapWorkloadBalance(ds);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AstuteAdherenceDispenseRawDataRow>> GetAstuteAdherenceDispensesAsync(DateOnly startDate, DateOnly endDate, CancellationToken c)
        {
            Dictionary<string, string> queryLiterals = new()
            {
                { ":StartDate", startDate.ToSqlDateLiteral() },
                { ":EndDate", endDate.ToSqlDateLiteral() }
            };
            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithLiteralsToDataSetAsync(
                CommandType.Text,
                "SelectAstuteAdherenceDispenses",
                queryLiterals,
                c);

            return _dataSetMapper.MapAstuteAdherenceDispenses(ds);
        }

        /// <inheritdoc />
        public IEnumerable<AstuteAdherenceDispenseReportRow> GetAstuteAdherenceDispensesReport(
            IEnumerable<AstuteAdherenceDispenseRawDataRow> dataRows,
            IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows,
            IEnumerable<SpecialtyDispenseExclusionRow> specialtyDispenseExclusionRows,
            out Dictionary<decimal, List<string>> constraintViolations)
        {
            return _dataSetMapper.PopulateAstuteAdherenceDispenseDerivedDetails(dataRows, completeSpecialtyItemRows, specialtyDispenseExclusionRows, out constraintViolations);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ImmunizationRow>> GetImmunizationRowsAsync(DateOnly runDate, CancellationToken c)
        {
            Dictionary<string, string> queryLiterals = new()
            {
                { ":RunDate", runDate.ToSqlDateLiteral() }
            };

            DataSet ds = await _mcKessonOracleInterface.RunQueryFileWithLiteralsToDataSetAsync(
                CommandType.Text,
                "SelectImmunizations",
                queryLiterals,
                c);

            return _dataSetMapper.MapImmunizations(ds);
        }
    }
}
