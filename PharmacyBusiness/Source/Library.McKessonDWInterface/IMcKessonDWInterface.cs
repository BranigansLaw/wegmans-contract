using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface.DataModel;
using System.Collections.ObjectModel;

namespace Library.McKessonDWInterface
{
    public interface IMcKessonDWInterface
    {
        /// <summary>
        /// Write a list of <typeparamref name="T"/> to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="downloadList"></param>
        /// <param name="downloadFileName"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        /// <param name="c">CancellationToken</param>
        Task WriteListToFileAsync<T>(
            List<T> downloadList,
            string downloadFileName,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            CancellationToken c);

        /// <summary>
        /// Write a list of <typeparamref name="T"/> to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="downloadList"></param>
        /// <param name="downloadFileName"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        /// <param name="shouldAppendToExistingFile"></param>
        /// <param name="c">CancellationToken</param>
        Task WriteListToFileAsync<T>(
            List<T> downloadList,
            string downloadFileName,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            bool shouldAppendToExistingFile,
            CancellationToken c);

        /// <summary>
        /// Get a list of Omnisys Claim for <paramref name="runDate"/>
        /// </summary>
        Task<IEnumerable<OmnisysClaimRow>> GetOmnisysClaimsAsync(DateOnly runDate, CancellationToken c);

        /// <summary>
        /// Get a list of New Tag Patient Groups for <paramref name="runDate"/>
        /// </summary>
        Task<IEnumerable<NewTagPatientGroupsRow>> GetNewTagPatientGroupsAsync(DateOnly runDate, CancellationToken c);

        /// <summary>
        /// Get a list of Rx Erp for <paramref name="runDate"/>
        /// </summary>
        Task<IEnumerable<RxErpRow>> GetRxErpAsync(DateOnly runDate, CancellationToken c);

        /// <summary>
        /// Get a list of Sold Detail for <paramref name="runDate"/>
        /// </summary>
        Task<IEnumerable<SoldDetailRow>> GetSoldDetailAsync(DateOnly runDate, CancellationToken c);

        /// <summary>
        /// Get a list of Store Inventory History for <paramref name="runDate"/>
        /// </summary>
        Task<IEnumerable<StoreInventoryHistoryRow>> GetStoreInventoryHistoryAsync(DateOnly runDate, CancellationToken c);

        /// <summary>
        /// Get a list of Turnaround Time raw data.
        /// </summary>
        /// <param name="startDate">Typically is previous Sunday</param>
        /// <param name="endDate">Typically is previous Saturday</param>
        /// <param name="tatTarget">One of these: 'SPECIALTY', 'EXCELLUS', 'IHA'</param>
        /// <param name="c">CancellationToken</param>
        /// <returns></returns>
        Task<IEnumerable<TatRawDataRow>> GetTatRawDataAsync(DateOnly startDate, DateOnly endDate, string tatTarget, CancellationToken c);

        /// <summary>
        /// Derive Turnaround Time Details.
        /// </summary>
        /// <param name="dataRows"></param>
        /// <param name="tatTarget"></param>
        /// <returns></returns>
        IEnumerable<TatDetailsRow> GetTatDetailsReport(IEnumerable<TatRawDataRow> dataRows, string tatTarget);

        /// <summary>
        /// 
        /// Derive Tat Summary from <paramref name="tatDataRows"/>
        /// </summary>
        /// <param name="dataRows"></param>
        /// <param name="tatTarget"></param>
        /// <returns></returns>
        IEnumerable<TatSummaryRow> DeriveTatSummaryReport(IEnumerable<TatDetailsRow> dataRows, string tatTarget);

        /// <summary>
        /// Derive Tat Summary Max Rx from <paramref name="tatSummaryRows"/>
        /// </summary>
        /// <param name="summaryRows"></param>
        /// <returns></returns>
        IEnumerable<TatSummaryMaxRxRow> DeriveTatSummaryMaxRxReport(IEnumerable<TatSummaryRow> summaryRows);

        /// <summary>
        /// Returns a list of TpcDataRow from SelectThirdPartyClaimsBase.sql
        /// </summary>
        /// <param name="runDate"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<TpcDataRow>> GetThirdPartyClaimsBaseAsync(DateOnly runDate, CancellationToken c);

        /// <summary>
        /// Returns a decimal to populate AcquisitionCost from SelectThirdPartyClaimsLookupAcquisitionCost.sql
        /// </summary>
        /// <param name="rxFillSeq"></param>
        /// <param name="rxRecordNum"></param>
        /// <param name="fillStateKey"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<decimal?> GetThirdPartyClaimsAcquisitionCostAsync(decimal? rxFillSeq, decimal? rxRecordNum, decimal? fillStateKey, CancellationToken c);

        /// <summary>
        /// Get a list of Sure Scripts Medical History for <paramref name="runDate"/>
        /// </summary>
        /// <param name="runFor"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<SureScriptsMedicalHistoryRawDataRow>> GetSureScriptsMedicalHistoryRawDataAsync(DateOnly runFor, CancellationToken c);

        /// <summary>
        /// Get a list of Sure Scripts Physician Notification Letters for <paramref name="runDate"/>
        /// </summary>
        /// <param name="runFor"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<SureScriptsPhysicianNotificationLetterRawDataRow>> GetSureScriptsPhysicianNotificationLettersRawDataAsync(DateOnly runFor, CancellationToken c);

        /// <summary>
        /// Derive Medical History Report from <paramref name="medicalHistoryRawDataRows"/>
        /// </summary>
        /// <param name="medicalHistoryRawDataRows"></param>
        /// <returns></returns>
        IEnumerable<SureScriptsMedicalHistoryReportRow> DeriveSureScriptsMedicalHistoryReport(IEnumerable<SureScriptsMedicalHistoryRawDataRow> medicalHistoryRawDataRows);

        /// <summary>
        /// Derive Medical History Report from <paramref name="medicalHistoryRawDataRows"/>
        /// </summary>
        /// <param name="pnlRawDataRows"></param>
        /// <param name="storeState"></param>
        /// <returns></returns>
        IEnumerable<SureScriptsPhysicianNotificationLetterReportRow> DeriveSureScriptsPhysicianNotificationLetterReport(IEnumerable<SureScriptsPhysicianNotificationLetterRawDataRow> pnlRawDataRows, string storeState);

        /// <summary>
        /// Returns a list of WorkloadBalanceRow from SelectWorkloadBalance.sql
        /// </summary>
        /// <param name="runDate"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<WorkloadBalanceRow>> GetWorkloadBalanceRowsAsync(DateOnly runDate, CancellationToken c);

        /// <summary>
        /// Returns a list of AstuteAdherenceDispenseRow from SelectAstuteAdherenceDispenses.sql
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<AstuteAdherenceDispenseRawDataRow>> GetAstuteAdherenceDispensesAsync(DateOnly startDate, DateOnly endDate, CancellationToken c);

        /// <summary>
        /// Derive Astute Adherence Dispense Report from <paramref name="dataRows"/>
        /// </summary>
        /// <param name="dataRows"></param>
        /// <returns></returns>
        IEnumerable<AstuteAdherenceDispenseReportRow> GetAstuteAdherenceDispensesReport(
            IEnumerable<AstuteAdherenceDispenseRawDataRow> dataRows,
            IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows,
            IEnumerable<SpecialtyDispenseExclusionRow> specialtyDispenseExclusionRows,
            out Dictionary<decimal, List<string>> constraintViolations);

        /// <summary>
        /// Returns a list of ImmunizationRow from SelectImmunizations.sql
        /// </summary>
        /// <param name="runDate"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<ImmunizationRow>> GetImmunizationRowsAsync(DateOnly runDate, CancellationToken c);
    }
}