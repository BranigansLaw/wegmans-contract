using CaseServiceWrapper;
using INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.EmplifyFieldConverter;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Exceptions;
using System.Collections.ObjectModel;

namespace INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.EmplifiMapping
{
    public class EmplifiMapperImp : IEmplifiMapper
    {
        private const string _currentPharmCode = "WEG";
        private const int _currentPharmNpi = 1902172612;
        private const string _currentProgId = "DD";
        private readonly IEmplifyFieldConverter _emplifyFieldConverter;

        public EmplifiMapperImp(IEmplifyFieldConverter emplifyFieldConverter)
        {
            _emplifyFieldConverter = emplifyFieldConverter ?? throw new ArgumentNullException(nameof(emplifyFieldConverter));
        }

        public DelayAndDenialStatusRow MappingForEmplifi(Case emplifiCase, Address address, Issue issue, DateTime startDateTime, DateTime endDateTime)
        {
            var constraintViolations = new Collection<string>();
            constraintViolations.Clear();

            DelayAndDenialStatusRow returnRow = new DelayAndDenialStatusRow
            {
                PharmCode = _emplifyFieldConverter.GetPharmCode(_currentPharmCode, dataConstraints: constraintViolations),
                PharmNpi = _emplifyFieldConverter.GetPharmNpi(_currentPharmNpi, dataConstraints: constraintViolations),
                SpTransactionId = _emplifyFieldConverter.FormatTransactionID(address.address_id, emplifiCase.case_id, issue.issue_seq, dataConstraints: constraintViolations),
                ProgramId = _emplifyFieldConverter.GetProgId(_currentProgId, dataConstraints: constraintViolations),
                PatientId = _emplifyFieldConverter.GetPatientId(address, dataConstraints: constraintViolations),
                PatientLastName = _emplifyFieldConverter.GetPatientLastName(address.last_name, dataConstraints: constraintViolations),
                PatientFirstName = _emplifyFieldConverter.GetPatientFirstName(address.given_names, dataConstraints: constraintViolations),
                PatientDob = _emplifyFieldConverter.FormatDateToYYYYMMDDDob(address.a05_code, dataConstraints: constraintViolations),
                Brand = _emplifyFieldConverter.GetBrand(issue.c47_code, dataConstraints: constraintViolations),
                NdcNumber = _emplifyFieldConverter.GetNdc(issue.c39_code, dataConstraints: constraintViolations),
                ShipDate = _emplifyFieldConverter.GetShipDateStatus(issue.c16_code, issue.c12_code, dataConstraints: constraintViolations),
                TransactionType = _emplifyFieldConverter.GetTransactionType(issue.c16_code, dataConstraints: constraintViolations),
                Carrier = _emplifyFieldConverter.GetCarrier(issue.c16_code, issue.c84_code, dataConstraints: constraintViolations),
                TrackingNumber = _emplifyFieldConverter.GetTrackingNumber(issue.c16_code, issue.c83_code, dataConstraints: constraintViolations),
                Quantity = _emplifyFieldConverter.GetQuantity(issue.c16_code, issue.c07_code, dataConstraints: constraintViolations),
                DaySupply = _emplifyFieldConverter.GetDaySupply(issue.c16_code, issue.c21_code, dataConstraints: constraintViolations),
                FillType = _emplifyFieldConverter.GetFillType(issue.c16_code, issue.c09_code, dataConstraints: constraintViolations),
                PresLastName = _emplifyFieldConverter.GetPresLastName(address.a43_code, address.a23_code, dataConstraints: constraintViolations),
                PresFirstName = _emplifyFieldConverter.GetPresFirstName(address.a42_code, address.a22_code, dataConstraints: constraintViolations),
                PresNpi = _emplifyFieldConverter.GetPresLong(address.a56_code, address.a36_code, dataConstraints: constraintViolations),
                PresDea = _emplifyFieldConverter.GetPresDea(address.a54_code, address.a34_code, dataConstraints: constraintViolations),
                PresAddr1 = _emplifyFieldConverter.GetPresAddr1(address.a44_code, address.a24_code, dataConstraints: constraintViolations),
                PresAddr2 = _emplifyFieldConverter.GetPresAddr2(address.a45_code, address.a25_code, dataConstraints: constraintViolations),
                PresCity = _emplifyFieldConverter.GetPresCity(address.a47_code, address.a27_code, dataConstraints: constraintViolations),
                PresState = _emplifyFieldConverter.GetPresState(address.a48_code, address.a28_code, dataConstraints: constraintViolations),
                PresZip = _emplifyFieldConverter.GetPresInt(address.a49_code, address.a29_code, dataConstraints: constraintViolations),
                PresPhone = _emplifyFieldConverter.FormatPhoneNumber(_emplifyFieldConverter.GetPresString(address.a50_code, address.a30_code, dataConstraints: constraintViolations), dataConstraints: constraintViolations),
                PresFax = _emplifyFieldConverter.FormatFaxNumber(address.a31_code, dataConstraints: constraintViolations),
                DemographicId = _emplifyFieldConverter.GetDemographicId(address, dataConstraints: constraintViolations),
                CaseId = _emplifyFieldConverter.GetCaseId(issue.c97_code, dataConstraints: constraintViolations),
                StatusDate = _emplifyFieldConverter.FormatDateToYYYYMMDDHHMMSS(issue.c26_code, dataConstraints: constraintViolations),
                Status = _emplifyFieldConverter.GetStatus(issue.c16_code, dataConstraints: constraintViolations),
                SubStatus = _emplifyFieldConverter.GetSubStatus(issue.c16_code, dataConstraints: constraintViolations),
                EnrollmentStatus = _emplifyFieldConverter.GetEnrollmentStatus(emplifiCase.b38_code, dataConstraints: constraintViolations),
                TransferPharmacyName = _emplifyFieldConverter.GetTransferPharmacyName(issue.c16_code, emplifiCase.b15_code, emplifiCase.b13_code, dataConstraints: constraintViolations),
                InsuranceName = _emplifyFieldConverter.GetInsuranceName(address.a07_code, dataConstraints: constraintViolations),
                InsuranceBin = _emplifyFieldConverter.GetInsuranceBin(address.a09_code, dataConstraints: constraintViolations),
                InsurancePcn = _emplifyFieldConverter.GetInsurancePcn(address.a10_code, dataConstraints: constraintViolations),
                InsuranceGroup = _emplifyFieldConverter.GetInsuranceGroup(address.a11_code, dataConstraints: constraintViolations),
                InsuranceId = _emplifyFieldConverter.GetInsuranceId(address.a08_code, dataConstraints: constraintViolations),
                UpdateType = _emplifyFieldConverter.GetUpdateType(address.a38_code, startDateTime, endDateTime, dataConstraints: constraintViolations),
            };

            if (constraintViolations.ToArray().Length > 0)
                throw new DataIntegrityException(constraintViolations);

            return returnRow;

        }
    }
}
