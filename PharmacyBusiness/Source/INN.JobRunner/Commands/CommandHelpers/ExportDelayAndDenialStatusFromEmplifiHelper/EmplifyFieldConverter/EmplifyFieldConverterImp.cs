using CaseServiceWrapper;
using Library.EmplifiInterface.Extensions;
using System.Text;
using System.Web;

namespace INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.EmplifyFieldConverter
{
    public class EmplifyFieldConverterImp : IEmplifyFieldConverter
    {
        private const string currentTransactionType = "LI";
        private const string SHIPPED = "A1=Shipped";
        private const string fillTypeN = "N";
        private const string fillTypeR = "R";
        private const string _demographicIdType = "JCP ID";
        private const string _patientIdType = "ONC Delay Denial CarePath Patient ID";
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
                _ = DateTime.TryParse(date, out var parsedDate);
                int temp = int.Parse(parsedDate.ToString("yyyyMMdd"));
                return temp != _badDateReturnInt ? temp : throw new Exception(FieldRequiredMessage);
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [PatientDob]: {e.Message}");
                return default;
            }
        }

        public int FormatDateToYYYYMMDDShipDate(string date, ICollection<string> dataConstraints)
        {
            try
            {
                _ = DateTime.TryParse(date, out var parsedDate);
                int temp = int.Parse(parsedDate.ToString("yyyyMMdd"));
                return temp != _badDateReturnInt ? temp : throw new Exception(FieldRequiredMessage);
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
                _ = DateTime.TryParse(date, out var parsedDate);
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

        public int? GetShipDateStatus(string status, string shipDate, ICollection<string> dataConstraints)
        {
            try
            {
                if (IsShipStatusShipped(status))
                {
                    return FormatDateToYYYYMMDDShipDate(shipDate, dataConstraints);
                }
                return null;
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [ShipDate]: {e.Message}");
                return default;
            }
        }

        public string? GetTransactionType(string status, ICollection<string> dataConstraints)
        {
            try
            {

                if (IsShipStatusShipped(status))
                {
                    return currentTransactionType.CleanNullableStringForVendorFileLimitations();
                }
                return null;
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [TransactionType]: {e.Message}");
                return string.Empty;
            }
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

        public int? GetQuantity(string status, string quantity, ICollection<string> dataConstraints)
        {
            try
            {
                if (IsShipStatusShipped(status))
                {
                    return int.Parse(quantity);
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
                if (IsShipStatusShipped(status))
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

        public string? GetFillType(string status, string fillNumString, ICollection<string> dataConstraints)
        {
            try
            {
                if (IsShipStatusShipped(status))
                {
                    if (int.TryParse(fillNumString, out int fillNum) && fillNum == 1)
                        return fillTypeN.CleanNullableStringForVendorFileLimitations();

                    return fillTypeR.CleanNullableStringForVendorFileLimitations();
                }

                return null;
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [FillType]: {e.Message}");
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

        public int GetPresInt(string healthCare, string other, ICollection<string> dataConstraints)
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

        public long? FormatPhoneNumber(string phoneNumber, ICollection<string> dataConstraints)
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

        public long? FormatFaxNumber(string phoneNumber, ICollection<string> dataConstraints)
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

        public string GetStatus(string status, ICollection<string> dataConstraints)
        {
            try
            {
                return (status switch
                {
                    "A1=Shipped" => "ACTIVE",
                    "P1=New Referral" or "P4=Future Shipment" or "P6=Patient Decision - Pending Financial" => "PENDING",
                    "C2=Unable To Contact" or "C3=Patient Decision - Clinical" or "C4=Patient Decision - Financial" or "C9=Alt Therapy" or "C14=Other" => "CANCELLED",
                    "D2=Unable To Contact" or "D3=Patient Decision - Clinical" or "D4=Patient Decision - Financial" or "D5=Patient Decision - Other" or "D8=Alt Therapy" or "D9=Completed"
                        or "D11=Coverage Determination Not Avail." or "D12=Patient Deceased" or "D14=Therapy End" or "D16=Transferred Rx To New Sp" => "DISCONTINUED",
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
                return (status switch
                {
                    "A1=Shipped" => "SHIPMENT",
                    "P1=New Referral" => "NEW",
                    "P4=Future Shipment" => "FUTURE_SHIPMENT",
                    "P6=Patient Decision - Pending Financial" or "C4=Patient Decision - Financial" or "D4=Patient Decision - Financial" => "PATIENT_FINA",
                    "C2=Unable To Contact" or "D2=Unable To Contact" => "PATIENT_RESP",
                    "C3=Patient Decision - Clinical" or "D3=Patient Decision - Clinical" or "D5=Patient Decision - Other" => "PATIENT_END",
                    "C9=Alt Therapy" or "D8=Alt Therapy" => "ALT_THERAPY",
                    "C14=Other" => "OTHER",
                    "D9=Completed" => "COMPLETED",
                    "D11=Coverage Determination Not Avail." => "COVERAGE_UNKNOWN",
                    "D12=Patient Deceased" => "PATIENT_DECD",
                    "D14=Therapy End" => "THERAPY_END",
                    "D16=Transferred Rx To New Sp" => "TRANSFER_SP",
                    _ => string.Empty,
                }).CleanNonNullableStringForVendorFileLimitations();
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [SubStatus]: {e.Message}");
                return string.Empty;
            }
        }

        public string GetEnrollmentStatus(string status, ICollection<string> dataConstraints)
        {
            try
            {
                return (status switch
                {
                    "Yes" => "Y",
                    _ => "H"
                }).CleanNonNullableStringForVendorFileLimitations();
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [EnrollmentStatus]: {e.Message}");
                return string.Empty;
            }
        }

        public string? GetTransferPharmacyName(string status, string? transferPharmacyConfirmed, string? transferPharmacy, ICollection<string> dataConstraints)
        {
            try
            {
                if (!string.Equals(status, "D16=Transferred Rx To New Sp", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }

                if (string.IsNullOrWhiteSpace(transferPharmacyConfirmed))
                {
                    return transferPharmacy.CleanNullableStringForVendorFileLimitations();
                }

                return transferPharmacyConfirmed.CleanNullableStringForVendorFileLimitations();
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [TransferPharmacyName]: {e.Message}");
                return string.Empty;
            }
        }

        public string? GetUpdateType(string? addressDateChangedString, DateTime startDateTime, DateTime endDateTime, ICollection<string> dataConstraints)
        {
            try
            {
                var addressDateChanged = DateTime.TryParse(addressDateChangedString, out var parsedAddressDateChanged) ? parsedAddressDateChanged : (DateTime?)null;

                if (startDateTime <= addressDateChanged && addressDateChanged < endDateTime)
                {
                    return "Insurance Change".CleanNullableStringForVendorFileLimitations();
                }

                return null;
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [UpdateType]: {e.Message}");
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

        public string GetDemographicId(Address address, ICollection<string> dataConstraints)
        {
            try
            {
                return address.PhoneList?.Phone?.FirstOrDefault(x => string.Equals(x.phone_type_code, _demographicIdType, StringComparison.OrdinalIgnoreCase))?.phone.CleanNonNullableStringForVendorFileLimitations() ?? string.Empty;
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [DemographicId]: {e.Message}");
                return string.Empty;
            }
        }

        public string GetCaseId(string CaseId, ICollection<string> dataConstraints)
        {
            try
            {
                return CaseId.CleanNonNullableStringForVendorFileLimitations();
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [CaseId]: {e.Message}");
                return string.Empty;
            }
        }

        public string GetInsuranceName(string insuranceName, ICollection<string> dataConstraints)
        {
            try
            {
                return insuranceName.CleanNonNullableStringForVendorFileLimitations();
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [InsuranceName]: {e.Message}");
                return string.Empty;
            }
        }

        public string GetInsuranceBin(string insuranceBin, ICollection<string> dataConstraints)
        {
            try
            {
                return insuranceBin.CleanNonNullableStringForVendorFileLimitations();
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [InsuranceBin]: {e.Message}");
                return string.Empty;
            }
        }

        public string GetInsurancePcn(string insurancePcn, ICollection<string> dataConstraints)
        {
            try
            {
                return insurancePcn.CleanNonNullableStringForVendorFileLimitations();
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [InsurancePcn]: {e.Message}");
                return string.Empty;
            }
        }

        public string GetInsuranceGroup(string insuranceGroup, ICollection<string> dataConstraints)
        {
            try
            {
                return insuranceGroup.CleanNonNullableStringForVendorFileLimitations();
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [InsuranceGroup]: {e.Message}");
                return string.Empty;
            }
        }

        public string GetInsuranceId(string insuranceId, ICollection<string> dataConstraints)
        {
            try
            {
                return insuranceId.CleanNonNullableStringForVendorFileLimitations();
            }
            catch (Exception e)
            {
                dataConstraints.Add($"{ErrorAssigningValueMessage} [InsuranceId]: {e.Message}");
                return string.Empty;
            }
        }
    }
}
