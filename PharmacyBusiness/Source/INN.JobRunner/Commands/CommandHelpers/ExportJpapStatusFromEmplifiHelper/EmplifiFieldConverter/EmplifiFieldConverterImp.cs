using CaseServiceWrapper;
using Library.EmplifiInterface.Extensions;
using Microsoft.IdentityModel.Tokens;

namespace INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper.EmplifiFieldConverter;

public class EmplifiFieldConverterImp : IEmplifiFieldConverter
{
    private const string SHIPPED = "A1=Shipped";
    private const string _demographicIdType = "JCP ID";
    private const string _patientIdType = "JPAP CarePath Patient ID";
    private const int _badDateReturnInt = 10101;
    private const long _badDateReturnLong = 10101000000;

    public const string ErrorAssigningValueMessage = "Error assigning value to field";
    public const string FieldRequiredMessage = "Value cannot be blank";

    public string FormatTransactionID(string addressId, string caseId, string issueSeq, ICollection<string> dataConstraints)
    {
        try
        {
            string tempString = issueSeq.PadLeft(3, '0');
            return (addressId + "0" + caseId + tempString).CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [SpTransactionId]: {e.Message}");
            return string.Empty;
        }
    }

    public int FormatDateToYYYYMMDDDob(string date, ICollection<string> dataConstraints)
    {
        try
        {
            var parsedDate = DateTime.Parse(date);
            int temp = int.Parse(parsedDate.ToString("yyyyMMdd"));
            return temp != _badDateReturnInt ? temp : throw new Exception(FieldRequiredMessage);
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PatientDob]: {e.Message}");
            return default;
        }
    }

    public int? FormatDateToYYYYMMDDShipDate(string status, string date, ICollection<string> dataConstraints)
    {
        try
        {
            if (IsShipStatusShipped(status))
            {
                _ = DateTime.TryParse(date, out var parsedDate);
                int temp = int.Parse(parsedDate.ToString("yyyyMMdd"));
                return temp != _badDateReturnInt ? temp : null;
            }
            return null;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [ShipDate]: {e.Message}");
            return default;
        }
    }

    public long FormatDateToYYYYMMDDHHMMSS(string date, ICollection<string> dataConstraints)
    {
        try
        {
            var parsedDate = DateTime.Parse(date);
            long temp = long.Parse(parsedDate.ToString("yyyyMMddHHmmss"));
            return temp != _badDateReturnLong ? temp : throw new Exception(FieldRequiredMessage);
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [StatusDate]: {e.Message}");
            return default;
        }
    }

    public bool IsShipStatusShipped(string status)
    {
        return status.Equals(SHIPPED);
    }

    public string? GetCarrier(string status, string carrier, ICollection<string> dataConstraints)
    {
        try
        {
            if (IsShipStatusShipped(status))
            {
                return carrier.CleanNullableStringForVendorFileLimitations();
            }
            return null;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [Carrier]: {e.Message}");
            return string.Empty;
        }
    }

    public string? GetTrackingNumber(string status, string tracking, ICollection<string> dataConstraints)
    {
        try
        {
            if (IsShipStatusShipped(status))
            {
                return tracking.CleanNullableStringForVendorFileLimitations();
            }
            return null;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [TrackingNumber]: {e.Message}");
            return string.Empty;
        }
    }

    public decimal? GetQuantity(string status, string quantity, ICollection<string> dataConstraints)
    {
        try
        {
            if (IsShipStatusShipped(status) && quantity is not null)
            {
                return decimal.Parse(quantity);
            }
            return null;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [Quantity]: {e.Message}");
            return default;
        }
    }

    public int? GetDaySupply(string status, string daySupply, ICollection<string> dataConstraints)
    {
        try
        {
            if (IsShipStatusShipped(status) && daySupply is not null)
            {
                return int.Parse(daySupply);
            }
            return null;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [DaySupply]: {e.Message}");
            return default;
        }
    }

    public string? GetRefillsRemaining(string status, string refillsRemaining, ICollection<string> dataConstraints)
    {
        try
        {
            if (IsShipStatusShipped(status))
            {
                return refillsRemaining;
            }
            return null;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [RefillsRemaining]: {e.Message}");
            return default;
        }
    }

    public string GetPresLastName(string? pres, string healthCare, ICollection<string> dataConstraints)
    {
        try
        {
            if (pres is null)
            {
                return healthCare.CleanNonNullableStringForVendorFileLimitations();
            }
            return pres.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PresLastName]: {e.Message}");
            return string.Empty;
        }
    }

    public string GetPresFirstName(string? pres, string healthCare, ICollection<string> dataConstraints)
    {
        try
        {
            if (pres is null)
            {
                return healthCare.CleanNonNullableStringForVendorFileLimitations();
            }
            return pres.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PresFirstName]: {e.Message}");
            return string.Empty;
        }
    }

    public long GetPresLong(string? pres, string healthCare, ICollection<string> dataConstraints)
    {
        try
        {
            if (pres is null)
            {
                return long.Parse(healthCare);
            }
            return long.Parse(pres);
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PresNpi]: {e.Message}");
            return default;
        }
    }

    public string GetPresDea(string? pres, string healthCare, ICollection<string> dataConstraints)
    {
        try
        {
            if (pres is null)
            {
                return healthCare.CleanNonNullableStringForVendorFileLimitations();
            }
            return pres.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PresDea]: {e.Message}");
            return string.Empty;
        }
    }

    public string GetPresAddr1(string? pres, string healthCare, ICollection<string> dataConstraints)
    {
        try
        {
            if (pres is null)
            {
                return healthCare.CleanNonNullableStringForVendorFileLimitations();
            }
            return pres.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PresAddr1]: {e.Message}");
            return string.Empty;
        }
    }

    public string GetPresAddr2(string? pres, string healthCare, ICollection<string> dataConstraints)
    {
        try
        {
            if (pres is null)
            {
                return healthCare.CleanNonNullableStringForVendorFileLimitations();
            }

            return pres.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PresAddr2]: {e.Message}");
            return string.Empty;
        }
    }

    public string GetPresCity(string healthCare, string other, ICollection<string> dataConstraints)
    {
        try
        {
            return string.IsNullOrEmpty(healthCare) ? other.CleanNonNullableStringForVendorFileLimitations() : healthCare.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PresCity]: {e.Message}");
            return string.Empty;
        }
    }

    public string GetPresState(string healthCare, string other, ICollection<string> dataConstraints)
    {
        try
        {
            return string.IsNullOrEmpty(healthCare) ? other.CleanNonNullableStringForVendorFileLimitations() : healthCare.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PresState]: {e.Message}");
            return string.Empty;
        }
    }

    public int GetPresZip(string healthCare, string other, ICollection<string> dataConstraints)
    {
        try
        {
            return int.Parse(string.IsNullOrEmpty(healthCare) ? other : healthCare);
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PresZip]: {e.Message}");
            return default;
        }
    }

    public long? FormatPresPhoneNumber(string phoneNumber, ICollection<string> dataConstraints)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return null;
            }

            return long.Parse(phoneNumber.Where(char.IsDigit).ToArray());
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PresPhone]: {e.Message}");
            return default;
        }
    }

    public long? FormatPresFaxNumber(string phoneNumber, ICollection<string> dataConstraints)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
            {
                return null;
            }

            return long.Parse(phoneNumber.Where(char.IsDigit).ToArray());
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PresFax]: {e.Message}");
            return default;
        }
    }

    public string GetDemographicId(Address address, ICollection<string> dataConstraints)
    {
        try
        {
            return address.PhoneList?.Phone?.FirstOrDefault(x => string.Equals(x.phone_type_code, _demographicIdType, StringComparison.OrdinalIgnoreCase))?.phone.CleanNonNullableStringForVendorFileLimitations() ?? throw new Exception(FieldRequiredMessage);
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [DemographicId]: {e.Message}");
            return string.Empty;
        }
    }

    public string GetCarePathTransactionId(string CarePathTransactionIdIssue, string CarePathTransactionIdCase, ICollection<string> dataConstraints)
    {
        try
        {
            if (!CarePathTransactionIdCase.IsNullOrEmpty() || !CarePathTransactionIdIssue.IsNullOrEmpty())
            {
                return CarePathTransactionIdIssue ?? CarePathTransactionIdCase;
            }
            throw new Exception(FieldRequiredMessage);
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [CarePathTransactionId]: {e.Message}");
            return string.Empty;
        }
    }

    public string GetShipToLocation(string shipToLocation_a53, string shipToLocation_c93, ICollection<string> dataConstraints)
    {
        try
        { 
            if (!shipToLocation_a53.IsNullOrEmpty() || !shipToLocation_c93.IsNullOrEmpty())
            {
                return shipToLocation_a53 ?? shipToLocation_c93;
            }
            throw new Exception(FieldRequiredMessage);
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [ShipToLocation]: {e.Message}");
            return string.Empty;
        }
    }

    public string? GetShipToAddr1(string shipToAddress1, string status, ICollection<string> dataConstraints)
    {
        try
        {
            if (IsShipStatusShipped(status))
            {
                return shipToAddress1.CleanNullableStringForVendorFileLimitations();
            }
            return null;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [ShipToAddr1]: {e.Message}");
            return default;
        }
    }

    public string? GetShipToAddr2(string shipToAddress2, string status, ICollection<string> dataConstraints)
    {
        try
        {
            if (IsShipStatusShipped(status))
            {
                return shipToAddress2.CleanNullableStringForVendorFileLimitations();
            }
            return null;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [ShipToAddr2]: {e.Message}");
            return default;
        }
    }

    public string? GetShipToCity(string shipToCity, string status, ICollection<string> dataConstraints)
    {
        try
        {
            if (IsShipStatusShipped(status))
            {
                return shipToCity.CleanNullableStringForVendorFileLimitations();
            }
            return null;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [ShipToCity]: {e.Message}");
            return default;
        }
    }

    public string? GetShipToState(string shipToState, string status, ICollection<string> dataConstraints)
    {
        try
        {
            if (IsShipStatusShipped(status))
            {
                return shipToState.CleanNullableStringForVendorFileLimitations();
            }
            return null;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [ShipToState]: {e.Message}");
            return default;
        }
    }

    public int? GetShipToZip(string shipToZip, string status, ICollection<string> dataConstraints)
    {
        try
        {
            if (IsShipStatusShipped(status))
            {
                return int.Parse(shipToZip);
            }
            return null;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [ShipToZip]: {e.Message}");
            return default;
        }
    }

    public string GetStatus(string status, ICollection<string> dataConstraints)
    {
        try
        {
            return ((status.ToUpper()) switch
            {
                "A1=SHIPPED" or "P5=FULFILLMENT IN PROGRESS" => "ACTIVE",
                "P1=NEW REFERRAL" or "P4=FUTURE SHIPMENT" or "P5=FUTURE SHIPMENT" or "P7=INVENTORY HOLD" or "P8=PENDING PATIENT RESPONSE" or "P9=PENDING HCP RESPONSE" => "PENDING",
                "C2=UNABLE TO CONTACT" or "C3=PATIENT DECISION - CLINICAL" or "C5=UNABLE TO CONTACT PRESCRIBER" or "C6=HCP DECISION - CLINICAL" 
                    or "C7=INELIGIBLE For PROGRAM" or "C10=DUPLICATE" or "C12=PATIENT DECEASED" or "C14=OTHER" => "CANCELLED",
                _ => string.Empty,
            }).CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [Status]: {e.Message}");
            return string.Empty;
        }
    }

    public string GetSubStatus(string status, ICollection<string> dataConstraints)
    {
        try
        {
            return ((status.ToUpper()) switch
            {
                "A1=SHIPPED" => "SHIPMENT",
                "P1=NEW REFERRAL" => "NEW",
                "P4=FUTURE SHIPMENT" => "FUTURE_SHIPMENT",
                "P5=FULFILLMENT IN PROGRESS" => "IN PROGRESS",
                "P7=INVENTORY HOLD" => "INVENTORY HOLD",
                "P8=PENDING PATIENT RESPONSE" or "C2=UNABLE TO CONTACT" => "PATIENT_RESP",
                "P9=PENDING HCP RESPONSE" or "C5=UNABLE TO CONTACT PRESCRIBER" => "PRESCRIBER",
                "C3=PATIENT DECISION - CLINICAL" => "PATIENT_END",
                "C6=HCP DECISION - CLINICAL" => "PRESCRIBER_END",
                "C7=INELIGIBLE FOR PROGRAM" => "PATIENT_INELIGIBLE",
                "C10=DUPLICATE" => "DUPLICATE",
                "C12=PATIENT DECEASED" => "PATIENT_DECD",
                "C14=OTHER" => "OTHER",
                _ => string.Empty,
            }).CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [SubStatus]: {e.Message}");
            return string.Empty;
        }
    }

    public string GetPharmCode(string pharmCode, ICollection<string> dataConstraints)
    {
        try
        {
            return pharmCode.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PharmCode]: {e.Message}");
            return string.Empty;
        }
    }

    public int GetPharmNpi(int pharmNpi, ICollection<string> dataConstraints)
    {
        try
        {
            return pharmNpi;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PharmCode]: {e.Message}");
            return default;
        }
    }

    public string GetProgId(string progId, ICollection<string> dataConstraints)
    {
        try
        {
            return progId.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [ProgramId]: {e.Message}");
            return string.Empty;
        }

    }

    public string GetPatientId(Address address, ICollection<string> dataConstraints)
    {
        try
        {
            return address.PhoneList?.Phone?.FirstOrDefault(x => string.Equals(x.phone_type_code, _patientIdType, StringComparison.OrdinalIgnoreCase))?.phone.CleanNonNullableStringForVendorFileLimitations() ?? string.Empty;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PatientId]: {e.Message}");
            return string.Empty;
        }
    }

    public string GetPatientFirstName(string firstName, ICollection<string> dataConstraints)
    {
        try
        {
            return firstName.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PatientFirstName]: {e.Message}");
            return string.Empty;
        }
    }

    public string GetPatientLastName(string LastName, ICollection<string> dataConstraints)
    {
        try
        {
            return LastName.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PatientLastName]: {e.Message}");
            return string.Empty;
        }
    }

    public long? GetNdc(string? ndc, ICollection<string> dataConstraints)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(ndc))
            {
                return null;
            }

            return long.Parse(ndc.Where(char.IsDigit).ToArray());
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [NdcNumber]: {e.Message}");
            return default;
        }
    }

    public string GetBrand(string Brand, ICollection<string> dataConstraints)
    {
        try
        {
            return Brand.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [Brand]: {e.Message}");
            return string.Empty;
        }
    }

    public string GetPresString(string? pres, string healthCare, ICollection<string> dataConstraints)
    {
        try
        {
            if (pres is null)
            {
                return healthCare.CleanNonNullableStringForVendorFileLimitations();
            }
            return pres.CleanNonNullableStringForVendorFileLimitations();
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [PresPhone]: {e.Message}");
            return string.Empty;
        }
    }

    public string? GetReshipmentFlag(string reshipDate, string status, ICollection<string> dataConstraints)
    {
        try
        {
            if (IsShipStatusShipped(status))
            {
                return (reshipDate is not null) ? "Y" : "N";
            }
            return null;
        }
        catch (Exception e)
        {
            dataConstraints.Add($"{ErrorAssigningValueMessage} [ReshipmentFlag]: {e.Message}");
            return string.Empty;
        }
    }
}
