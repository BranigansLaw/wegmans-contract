using CaseServiceWrapper;
using INN.JobRunner.Commands.CommandHelpers.ExportDelayAndDenialStatusFromEmplifiHelper.EmplifyFieldConverter;
using INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper.EmplifiFieldConverter;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Exceptions;
using System.Collections.ObjectModel;

namespace INN.JobRunner.Commands.CommandHelpers.ExportJpapStatusFromEmplifiHelper.EmplifiMapping;

public class EmplifiMapperImp : IEmplifiMapper
{
    private const string _currentPharmCode = "WEG";
    private const int _currentPharmNpi = 1902172612;
    private const string _currentProgId = "PDE";
    private readonly IEmplifiFieldConverter _emplifiFieldConverter;

    public EmplifiMapperImp(IEmplifiFieldConverter emplifiFieldConverter)
    {
        _emplifiFieldConverter = emplifiFieldConverter ?? throw new ArgumentNullException(nameof(emplifiFieldConverter));
    }

    public JpapStatusRow MappingForEmplifi(Case emplifiCase, Address address, Issue issue, DateTime startDateTime, DateTime endDateTime)
    {
        var constraintViolations = new Collection<string>();
        constraintViolations.Clear();

        JpapStatusRow returnRow = new JpapStatusRow
        {
            PharmacyCode = _emplifiFieldConverter.GetPharmCode(_currentPharmCode, constraintViolations),
            PharmacyNpi = _emplifiFieldConverter.GetPharmNpi(_currentPharmNpi, constraintViolations),
            SpTransactionId = _emplifiFieldConverter.FormatTransactionID(address.address_id, emplifiCase.case_id, issue.issue_seq, constraintViolations),
            ProgramId = _emplifiFieldConverter.GetProgId(_currentProgId, constraintViolations),
            PatientId = _emplifiFieldConverter.GetPatientId(address, constraintViolations),
            PatientLastName = _emplifiFieldConverter.GetPatientLastName(address.last_name, constraintViolations),
            PatientFirstName = _emplifiFieldConverter.GetPatientFirstName(address.given_names, constraintViolations),
            PatientDob = _emplifiFieldConverter.FormatDateToYYYYMMDDDob(address.a05_code, constraintViolations),
            Brand = _emplifiFieldConverter.GetBrand(issue.c47_code, constraintViolations),
            NdcNumber = _emplifiFieldConverter.GetNdc(issue.c39_code, constraintViolations),
            ShipDate = _emplifiFieldConverter.FormatDateToYYYYMMDDShipDate(issue.c16_code, issue.c12_code, constraintViolations),
            Carrier = _emplifiFieldConverter.GetCarrier(issue.c16_code, issue.c84_code, constraintViolations),
            TrackingNumber = _emplifiFieldConverter.GetTrackingNumber(issue.c16_code, issue.c83_code, constraintViolations),
            Quantity = _emplifiFieldConverter.GetQuantity(issue.c16_code, issue.c07_code, constraintViolations),
            DaySupply = _emplifiFieldConverter.GetDaySupply(issue.c16_code, issue.c21_code, constraintViolations),
            RefillsRemaining = _emplifiFieldConverter.GetRefillsRemaining(issue.c16_code, issue.c06_code, constraintViolations),
            PresLastName = _emplifiFieldConverter.GetPresLastName(address.a43_code, address.a23_code, constraintViolations),
            PresFirstName = _emplifiFieldConverter.GetPresFirstName(address.a42_code, address.a22_code, constraintViolations),
            PresNpi = _emplifiFieldConverter.GetPresLong(address.a56_code, address.a36_code, constraintViolations),
            PresDea = _emplifiFieldConverter.GetPresDea(address.a54_code, address.a34_code, constraintViolations),
            PresAddr1 = _emplifiFieldConverter.GetPresAddr1(address.a44_code, address.a24_code, constraintViolations),
            PresAddr2 = _emplifiFieldConverter.GetPresAddr2(address.a45_code, address.a25_code, constraintViolations),
            PresCity = _emplifiFieldConverter.GetPresCity(address.a47_code, address.a27_code, constraintViolations),
            PresState = _emplifiFieldConverter.GetPresState(address.a48_code, address.a28_code, constraintViolations),
            PresZip = _emplifiFieldConverter.GetPresZip(address.a49_code, address.a29_code, constraintViolations),
            PresPhone = _emplifiFieldConverter.FormatPresPhoneNumber(_emplifiFieldConverter.GetPresString(address.a50_code, address.a30_code, constraintViolations), constraintViolations),
            PresFax = _emplifiFieldConverter.FormatPresFaxNumber(address.a31_code, constraintViolations),
            DemographicId = _emplifiFieldConverter.GetDemographicId(address, constraintViolations),
            CarePathTransactionId = _emplifiFieldConverter.GetCarePathTransactionId(issue.c97_code, emplifiCase.b47_code, constraintViolations),
            ShipToLocation = _emplifiFieldConverter.GetShipToLocation(address.a53_code, issue.c93_code, constraintViolations),
            ShipToAddress1 = _emplifiFieldConverter.GetShipToAddr1(emplifiCase.b20_code, issue.c16_code, constraintViolations),
            ShipToAddress2 = _emplifiFieldConverter.GetShipToAddr2(emplifiCase.b19_code, issue.c16_code, constraintViolations),
            ShipToCity = _emplifiFieldConverter.GetShipToCity(emplifiCase.b06_code, issue.c16_code, constraintViolations),
            ShipToState = _emplifiFieldConverter.GetShipToState(emplifiCase.b59_code, issue.c16_code, constraintViolations),
            ShipToZip = _emplifiFieldConverter.GetShipToZip(emplifiCase.b60_code, issue.c16_code, constraintViolations),
            ReshipmentFlag = _emplifiFieldConverter.GetReshipmentFlag(issue.c53_code, issue.c16_code, constraintViolations),
            StatusDate = _emplifiFieldConverter.FormatDateToYYYYMMDDHHMMSS(issue.c26_code, constraintViolations),
            Status = _emplifiFieldConverter.GetStatus(issue.c16_code, constraintViolations),
            SubStatus = _emplifiFieldConverter.GetSubStatus(issue.c16_code, constraintViolations)
        };

        if (constraintViolations.ToArray().Length > 0)
            throw new DataIntegrityException(constraintViolations);

        return returnRow;
    }
}
