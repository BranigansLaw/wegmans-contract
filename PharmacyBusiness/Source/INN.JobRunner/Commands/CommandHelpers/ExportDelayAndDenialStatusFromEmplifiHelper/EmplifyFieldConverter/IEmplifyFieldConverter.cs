using CaseServiceWrapper;
namespace INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.EmplifyFieldConverter
{
    public interface IEmplifyFieldConverter
    {
        string FormatTransactionID(string addressId, string caseId, string issueSeq, ICollection<string> dataConstraints);

        /// <summary>
        /// Takes in a string as "10/01/2003"
        /// returns as 20031001
        /// </summary>
        int FormatDateToYYYYMMDDDob(string date, ICollection<string> dataConstraints);

        int FormatDateToYYYYMMDDShipDate(string date, ICollection<string> dataConstraints);

        long FormatDateToYYYYMMDDHHMMSS(string date, ICollection<string> dataConstraints);

        bool IsShipStatusShipped(string status);

        int? GetShipDateStatus(string status, string shipDate, ICollection<string> dataConstraints);

        string? GetTransactionType(string status, ICollection<string> dataConstraints);

        string? GetCarrier(string status, string carrier, ICollection<string> dataConstraints);

        string? GetTrackingNumber(string status, string tracking, ICollection<string> dataConstraints);

        int? GetQuantity(string status, string quantity, ICollection<string> dataConstraints);

        int? GetDaySupply(string status, string daySupply, ICollection<string> dataConstraints);

        string? GetFillType(string status, string fillNumString, ICollection<string> dataConstraints);

        string GetPresString(string? pres, string healthCare, ICollection<string> dataConstraints);

        string GetPresFirstName(string? pres, string healthCare, ICollection<string> dataConstraints);

        string GetPresLastName(string? pres, string healthCare, ICollection<string> dataConstraints);

        string GetPresAddr1(string? pres, string healthCare, ICollection<string> dataConstraints);

        string GetPresAddr2(string? pres, string healthCare, ICollection<string> dataConstraints);

        string GetPresDea(string? pres, string healthCare, ICollection<string> dataConstraints);

        string GetPresState(string healthCare, string other, ICollection<string> dataConstraints);

        string GetPresCity(string healthCare, string other, ICollection<string> dataConstraints);

        int GetPresInt(string healthCare, string other, ICollection<string> dataConstraints);

        long GetPresLong(string? pres, string healthCare, ICollection<string> dataConstraints);

        long? FormatPhoneNumber(string phoneNumber, ICollection<string> dataConstraints);

        long? FormatFaxNumber(string phoneNumber, ICollection<string> dataConstraints);

        string GetStatus(string status, ICollection<string> dataConstraints);

        string GetSubStatus(string status, ICollection<string> dataConstraints);

        string GetEnrollmentStatus(string status, ICollection<string> dataConstraints);

        string? GetTransferPharmacyName(string status, string? transferPharmacyConfirmed, string? transferPharmacy, ICollection<string> dataConstraints);

        string? GetUpdateType(string? addressDateChangedString, DateTime startDateTime, DateTime endDateTime, ICollection<string> dataConstraints);

        long? GetNdc(string? ndc, ICollection<string> dataConstraints);

        string GetPharmCode(string pharmCode, ICollection<string> dataConstraints);

        int GetPharmNpi(int pharmNpi, ICollection<string> dataConstraints);

        string GetProgId(string progId, ICollection<string> dataConstraints);

        string GetPatientId(Address address, ICollection<string> dataConstraints);

        string GetPatientLastName(string LastName, ICollection<string> dataConstraints);

        string GetPatientFirstName(string firstName, ICollection<string> dataConstraints);

        string GetBrand(string Brand, ICollection<string> dataConstraints);

        string GetDemographicId(Address address, ICollection<string> dataConstraints);

        string GetCaseId(string CaseId, ICollection<string> dataConstraints);

        string GetInsuranceName(string insuranceName, ICollection<string> dataConstraints);

        string GetInsuranceBin(string insuranceBin, ICollection<string> dataConstraints);

        string GetInsurancePcn(string insurancePcn, ICollection<string> dataConstraints);

        string GetInsuranceGroup(string insuranceGroup, ICollection<string> dataConstraints);

        string GetInsuranceId(string insuranceId, ICollection<string> dataConstraints);
    }

}
