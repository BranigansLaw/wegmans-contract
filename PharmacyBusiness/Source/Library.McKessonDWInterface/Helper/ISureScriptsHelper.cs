using Library.McKessonDWInterface.DataModel;

namespace Library.McKessonDWInterface.Helper
{
    public interface ISureScriptsHelper
    {
        string FormatString(string text);

        /// <summary>
        /// Set default date if null.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        DateTime SetDefaultDate(DateTime? date);

        string LPad(string? text, int value, char valueToPadWith);

        string SetDefaultString(string? text);

        string SetDigitsOnly(string? text);

        string SetNullableValue(string? text, string defaultValue);

        decimal SetNullableValue(decimal? value, decimal defaultValue);

        long SetNullableValue(long? value, long defaultValue);

        decimal DispensedQuantityModifier(decimal? value);

        decimal SetRefillsRemaining(decimal? value);

        bool IsStatePA(string? state, string reportState);

        string SetDefaultStringConsideringState(string? text, bool isStatePa);

        string SetRouteOfAdministration(string? vaccineInformationStatementName, string? routeOfAdministrationDescription);

        string SetSiteOfAdministration(string? vaccineInformationStatementName, string? routeOfAdministrationDescription);

        string SetVaccineManufacturerName(string? drugNdc, string? drugLabeler);

        string SetVaccineName(string? drugNdc, string? drugLabelName);

        string SetVaccineCvxCode(string? cvxCode, string? vaccineInformationStatementName, string? drugLabeler, string? drugNdc);

        public SureScriptsMedicalHistoryHeaderRow GetMedHistoryHeaderRecord(DateOnly runfor);

        public SureScriptsMedicalHistoryTrailerRow GetMedHistoryTrailerRecord(int recordCount);

        string GetPNLHeaderRecord(DateOnly runfor, string delimeter);

        string GetPNLTrailerRecord(int? recordCount, string delimeter);

        /// <summary>
        /// Format a date to the required string format.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        string FormatDateString(DateTime date);

        /// <summary>
        /// Set a string to a maximum length.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="maxLength"></param>
        /// <returns></returns>
        string? SetStringToMaxLength(string? input, int maxLength);

        /// <summary>
        /// Remove trailing zero from a decimal.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string? RemoveTrailingZeroFromDecimal(decimal? value);

        /// <summary>
        /// De-duplicate the SureScripts Medical History Raw Data Rows by Patient Address and Prescriber Phone.
        /// </summary>
        /// <param name="rawDataRows"></param>
        /// <returns></returns>
        IEnumerable<SureScriptsMedicalHistoryRawDataRow> DeDuplicateMedHistoryRows(IEnumerable<SureScriptsMedicalHistoryRawDataRow> rawDataRows);

        /// <summary>
        /// Rank and filter the SureScripts Medical History Raw Data Rows by Patient Address,
        /// where addresses with the same values will have the same rank,
        /// and all instances of the top ranked address will be returned.
        /// </summary>
        /// <param name="dataRows"></param>
        /// <returns></returns>
        IEnumerable<SureScriptsMedicalHistoryRawDataRow> RankAndFilterMedHistoryByPatientAddress(IEnumerable<SureScriptsMedicalHistoryRawDataRow> dataRows);

        /// <summary>
        /// Rank and filter the SureScripts Medical History Raw Data Rows by Prescriber Phone,
        /// where phone numbers with the same values will NOT have the same rank, 
        /// and only one instance of the top ranked phone will be returned.
        /// </summary>
        /// <param name="dataRows"></param>
        /// <returns></returns>
        IEnumerable<SureScriptsMedicalHistoryRawDataRow> RankAndFilterMedHistoryByPrescriberPhone(IEnumerable<SureScriptsMedicalHistoryRawDataRow> dataRows);

        /// <summary>
        /// Rank the SureScripts Medical History Raw Data Rows by Prescription Number and Fill Number, for use in calculating the Compound Ingredient Sequence Number.
        /// </summary>
        /// <param name="rawDataRows"></param>
        /// <returns></returns>
        IEnumerable<SureScriptsMedicalHistoryRawDataRow> RankMedHistoryByPrescriptionFill(IEnumerable<SureScriptsMedicalHistoryRawDataRow> rawDataRows);

        /// <summary>
        /// Derive the Compound Ingredient Sequence Number from the NDC and Rank.
        /// </summary>
        /// <param name="ndc"></param>
        /// <param name="rank"></param>
        /// <returns></returns>
        int DeriveCompoundIngredientSequenceNumber(string? ndc, int? rank);
    }
}
