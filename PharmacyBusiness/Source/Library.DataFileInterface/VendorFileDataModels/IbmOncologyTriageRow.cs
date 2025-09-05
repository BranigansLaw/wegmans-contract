using Library.DataFileInterface.DataFileReader;
using Library.DataFileInterface.Exceptions;

namespace Library.DataFileInterface.VendorFileDataModels
{
    public class IbmOncologyTriageRow : IDataRecord
    {
        public string? RecordTimestamp { get; set; }
        public string? CarePathSpecialtyPharmacyName { get; set; }
        public string? CarePathPatientId { get; set; }
        public string? PatientLastName { get; set; }
        public string? PatientFirstName { get; set; }
        public string? PatientDateOfBirth { get; set; }
        public string? PatientGender { get; set; }
        public string? PatientAddress1 { get; set; }
        public string? PatientAddress2 { get; set; }
        public string? PatientCity { get; set; }
        public string? PatientState { get; set; }
        public string? PatientZipCode { get; set; }
        public string? PatientPhoneNumber { get; set; }
        public string? ProductName { get; set; }
        public string? PrescriberLastName { get; set; }
        public string? PrescriberFirstName { get; set; }
        public string? PrescriberNpi { get; set; }
        public string? CarePathPrescriberId { get; set; }
        public string? PrescriberAddress1 { get; set; }
        public string? PrescriberAddress2 { get; set; }
        public string? PrescriberCity { get; set; }
        public string? PrescriberState { get; set; }
        public string? PrescriberZipCode { get; set; }
        public string? PrescriberPhoneNumber { get; set; }
        public string? PriorAuthReceivedFromPayerDate { get; set; }
        public string? ShipToLocation { get; set; }
        public string? TreatmentCenterName { get; set; }
        public string? TreatmentCenterContactLastName { get; set; }
        public string? TreatmentCenterContactFirstName { get; set; }
        public string? TreatmentCenterAddress1 { get; set; }
        public string? TreatmentCenterAddress2 { get; set; }
        public string? TreatmentCenterCity { get; set; }
        public string? TreatmentCenterState { get; set; }
        public string? TreatmentCenterZipCode { get; set; }
        public string? TreatmentCenterPhoneNumber { get; set; }
        public string? TreatmentCenterFaxNumber { get; set; }
        public string? TreatmentCenterNpi { get; set; }
        public string? TreatmentCenterDea { get; set; }
        public string? DoseType { get; set; }
        public string? ImageAvailable { get; set; }
        public string? CaseId { get; set; }
        public string? NdcCode { get; set; }
        public string? PrimaryDiagnosisCode { get; set; }
        public string? PatientDemographicId { get; set; }

        /// <inheritdoc />
        public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns, DateOnly runFor)
        {
            Type classType = typeof(IbmOncologyTriageRow);
            int numberOfDataFileProperties = classType.GetProperties().Where(r => r.Name.IndexOf("Derived", 0) == -1).ToArray().Length;
            if (dataFileColumns.Count() != numberOfDataFileProperties)
            {
                throw new DataIntegrityException($"Number of columns [{dataFileColumns.Count()}] in the data file does not match number the expected number [{numberOfDataFileProperties}].");
            }

            RecordTimestamp = dataFileColumns.ElementAt(0);
            CarePathSpecialtyPharmacyName = dataFileColumns.ElementAt(1);
            CarePathPatientId = dataFileColumns.ElementAt(2);
            PatientLastName = dataFileColumns.ElementAt(3);
            PatientFirstName = dataFileColumns.ElementAt(4);
            PatientDateOfBirth = dataFileColumns.ElementAt(5);
            PatientGender = dataFileColumns.ElementAt(6);
            PatientAddress1 = dataFileColumns.ElementAt(7);
            PatientAddress2 = dataFileColumns.ElementAt(8);
            PatientCity = dataFileColumns.ElementAt(9);
            PatientState = dataFileColumns.ElementAt(10);
            PatientZipCode = dataFileColumns.ElementAt(11);
            PatientPhoneNumber = dataFileColumns.ElementAt(12);
            ProductName = dataFileColumns.ElementAt(13);
            PrescriberLastName = dataFileColumns.ElementAt(14);
            PrescriberFirstName = dataFileColumns.ElementAt(15);
            PrescriberNpi = dataFileColumns.ElementAt(16);
            CarePathPrescriberId = dataFileColumns.ElementAt(17);
            PrescriberAddress1 = dataFileColumns.ElementAt(18);
            PrescriberAddress2 = dataFileColumns.ElementAt(19);
            PrescriberCity = dataFileColumns.ElementAt(20);
            PrescriberState = dataFileColumns.ElementAt(21);
            PrescriberZipCode = dataFileColumns.ElementAt(22);
            PrescriberPhoneNumber = dataFileColumns.ElementAt(23);
            PriorAuthReceivedFromPayerDate = dataFileColumns.ElementAt(24);
            ShipToLocation = dataFileColumns.ElementAt(25);
            TreatmentCenterName = dataFileColumns.ElementAt(26);
            TreatmentCenterContactLastName = dataFileColumns.ElementAt(27);
            TreatmentCenterContactFirstName = dataFileColumns.ElementAt(28);
            TreatmentCenterAddress1 = dataFileColumns.ElementAt(29);
            TreatmentCenterAddress2 = dataFileColumns.ElementAt(30);
            TreatmentCenterCity = dataFileColumns.ElementAt(31);
            TreatmentCenterState = dataFileColumns.ElementAt(32);
            TreatmentCenterZipCode = dataFileColumns.ElementAt(33);
            TreatmentCenterPhoneNumber = dataFileColumns.ElementAt(34);
            TreatmentCenterFaxNumber = dataFileColumns.ElementAt(35);
            TreatmentCenterNpi = dataFileColumns.ElementAt(36);
            TreatmentCenterDea = dataFileColumns.ElementAt(37);
            DoseType = dataFileColumns.ElementAt(38);
            ImageAvailable = dataFileColumns.ElementAt(39);
            CaseId = dataFileColumns.ElementAt(40);
            NdcCode = dataFileColumns.ElementAt(41);
            PrimaryDiagnosisCode = dataFileColumns.ElementAt(42);
            PatientDemographicId = dataFileColumns.ElementAt(43);
        }
    }
}
