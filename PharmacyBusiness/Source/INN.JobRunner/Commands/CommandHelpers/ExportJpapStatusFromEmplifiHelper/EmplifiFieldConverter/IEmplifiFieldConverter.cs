using CaseServiceWrapper;

namespace INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper.EmplifiFieldConverter;

public interface IEmplifiFieldConverter
{
    string FormatTransactionID(string addressId, string caseId, string issueSeq, ICollection<string> dataConstraints);

    int FormatDateToYYYYMMDDDob(string date, ICollection<string> dataConstraints);

    int? FormatDateToYYYYMMDDShipDate(string status, string date, ICollection<string> dataConstraints);

    long FormatDateToYYYYMMDDHHMMSS(string date, ICollection<string> dataConstraints);

    bool IsShipStatusShipped(string status);

    string? GetCarrier(string status, string carrier, ICollection<string> dataConstraints);

    string? GetTrackingNumber(string status, string tracking, ICollection<string> dataConstraints);

    decimal? GetQuantity(string status, string quantity, ICollection<string> dataConstraints);

    int? GetDaySupply(string status, string daySupply, ICollection<string> dataConstraints);

    string? GetRefillsRemaining(string status, string refillsRemaining, ICollection<string> dataConstraints);

    string GetPresLastName(string? pres, string healthCare, ICollection<string> dataConstraints);

    string GetPresFirstName(string? pres, string healthCare, ICollection<string> dataConstraints);

    long GetPresLong(string? pres, string healthCare, ICollection<string> dataConstraints);

    string GetPresDea(string? pres, string healthCare, ICollection<string> dataConstraints);

    string GetPresAddr1(string? pres, string healthCare, ICollection<string> dataConstraints);

    string GetPresAddr2(string? pres, string healthCare, ICollection<string> dataConstraints);

    string GetPresCity(string healthCare, string other, ICollection<string> dataConstraints);

    string GetPresState(string healthCare, string other, ICollection<string> dataConstraints);

    int GetPresZip(string healthCare, string other, ICollection<string> dataConstraints);

    long? FormatPresPhoneNumber(string phoneNumber, ICollection<string> dataConstraints);

    long? FormatPresFaxNumber(string phoneNumber, ICollection<string> dataConstraints);

    string GetDemographicId(Address address, ICollection<string> dataConstraints);

    string GetCarePathTransactionId(string CarePathTransactionIdIssue, string CarePathTransactionIdCase, ICollection<string> dataConstraints);

    string GetShipToLocation(string shipToLocation_a53, string shipToLocation_c93, ICollection<string> dataConstraints);

    string? GetShipToAddr1(string shipToAddress1, string status, ICollection<string> dataConstraints);

    string? GetShipToAddr2(string shipToAddress2, string status, ICollection<string> dataConstraints);

    string? GetShipToCity(string shipToCity, string status, ICollection<string> dataConstraints);

    string? GetShipToState(string shipToState, string status, ICollection<string> dataConstraints);

    int? GetShipToZip(string shipToZip, string status, ICollection<string> dataConstraints);

    string GetStatus(string status, ICollection<string> dataConstraints);

    string GetSubStatus(string status, ICollection<string> dataConstraints);

    string GetPharmCode(string pharmCode, ICollection<string> dataConstraints);

    int GetPharmNpi(int pharmNpi, ICollection<string> dataConstraints);

    string GetProgId(string progId, ICollection<string> dataConstraints);

    string GetPatientId(Address address, ICollection<string> dataConstraints);

    string GetPatientFirstName(string firstName, ICollection<string> dataConstraints);

    string GetPatientLastName(string LastName, ICollection<string> dataConstraints);

    long? GetNdc(string? ndc, ICollection<string> dataConstraints);

    string GetBrand(string Brand, ICollection<string> dataConstraints);

    string GetPresString(string? pres, string healthCare, ICollection<string> dataConstraints);

    string? GetReshipmentFlag(string reshipDate, string status, ICollection<string> dataConstraints);
}

