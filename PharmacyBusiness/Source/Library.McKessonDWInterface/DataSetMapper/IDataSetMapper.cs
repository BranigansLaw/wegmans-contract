using Library.McKessonDWInterface.DataModel;
using Library.TenTenInterface.DataModel;
using System.Collections.ObjectModel;
using System.Data;

namespace Library.McKessonDWInterface.DataSetMapper
{
    public interface IDataSetMapper
    {
        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="OmnisysClaimRow"/>
        /// </summary>
        IEnumerable<OmnisysClaimRow> MapOmnisysClaim(DataSet ds);

        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="NewTagPatientGroupsRow"/>
        /// </summary>
        IEnumerable<NewTagPatientGroupsRow> MapNewTagPatientGroups(DataSet ds);

        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="RxErpRow"/>
        /// </summary>
        IEnumerable<RxErpRow> MapRxErp(DataSet ds);

        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="SoldDetailRow"/>
        /// </summary>
        IEnumerable<SoldDetailRow> MapSoldDetail(DataSet ds);

        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="StoreInventoryHistoryRow"/>
        /// </summary>
        IEnumerable<StoreInventoryHistoryRow> MapStoreInventoryHistory(DataSet ds);

        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="TatDetailsRow"/>
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        IEnumerable<TatRawDataRow> MapTatRawData(DataSet ds);

        /// <summary>
        /// Applies business rules to the raw data to derive additional record details.
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="tatTarget"></param>
        /// <returns></returns>
        IEnumerable<TatDetailsRow> PopulateTatDerivedDetails(IEnumerable<TatRawDataRow> dataRows, string tatTarget);

        /// <summary>
        /// Populates derived summary data from the TAT data.
        /// </summary>
        /// <param name="rows"></param>
        /// <returns></returns>
        IEnumerable<TatSummaryRow> PopulateTatDerivedSummary(IEnumerable<TatDetailsRow> dataRows, string tatTarget);

        /// <summary>
        /// Original requirements that one Rx within an Order that has the greatest DaysNetTat represent the time metrics for the entire Order..
        /// Populates derived summary max rx data from the TAT summary data.
        /// </summary>
        /// <param name="summaryRows"></param>
        /// <returns></returns>
        IEnumerable<TatSummaryMaxRxRow> GetOneRxWithinAnOrderHavingLargestDaysNetTat(IEnumerable<TatSummaryRow> summaryRows);

        /// <summary>
        /// Populates data for TpcDataRow
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        IEnumerable<TpcDataRow> MapThirdPartyClaimBase(DataSet ds);

        /// <summary>
        /// Populates only the acquisition cost data field for TpcDataRow. 
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        IEnumerable<decimal?> MapThirdPartyClaimsAcquisitionCost(DataSet ds);

        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="SureScriptsMedicalHistoryRawDataRow"/>
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        IEnumerable<SureScriptsMedicalHistoryRawDataRow> MapSureScriptsMedicalHistoryRawData(DataSet ds);

        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="SureScriptsPhysicianNotificationLetterRawDataRow"/>
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        IEnumerable<SureScriptsPhysicianNotificationLetterRawDataRow> MapSureScriptsPhysicianNotificationLettersRawData(DataSet ds);

        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="SureScriptsPharmacyNotificationLetterRawDataRow"/>
        /// </summary>
        /// <param name="dataRows"></param>
        /// <returns></returns>
        IEnumerable<SureScriptsMedicalHistoryReportRow> MapSureScriptsMedicalHistoryRawDataToFinalReport(IEnumerable<SureScriptsMedicalHistoryRawDataRow> dataRows);

        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="SureScriptsPharmacyNotificationLetterReportRow"/>
        /// </summary>
        /// <param name="dataRows"></param>
        /// <param name="storeState"></param>
        /// <returns></returns>
        IEnumerable<SureScriptsPhysicianNotificationLetterReportRow> MapSureScriptsPhysicianNotificationLetterRawDataToFinalReport(IEnumerable<SureScriptsPhysicianNotificationLetterRawDataRow> dataRows, string storeState);

        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="WorkloadBalanceRow"/>
        /// </summary>
        IEnumerable<WorkloadBalanceRow> MapWorkloadBalance(DataSet ds);

        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="AstuteAdherenceDispenseRawDataRow"/>
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        IEnumerable<AstuteAdherenceDispenseRawDataRow> MapAstuteAdherenceDispenses(DataSet ds);

        /// <summary>
        /// Populates derived data properties for reporting.
        /// </summary>
        /// <param name="dataRows"></param>
        /// <returns></returns>
        IEnumerable<AstuteAdherenceDispenseReportRow> PopulateAstuteAdherenceDispenseDerivedDetails(
            IEnumerable<AstuteAdherenceDispenseRawDataRow> rawDataRows,
            IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows,
            IEnumerable<SpecialtyDispenseExclusionRow> specialtyDispenseExclusionRows,
            out Dictionary<decimal, List<string>> constraintViolations);

        /// <summary>
        /// Maps a <see cref="DataSet"/> to a enumerable of <see cref="ImmunizationRow"/>
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        IEnumerable<ImmunizationRow> MapImmunizations(DataSet ds);
    }
}
