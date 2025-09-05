namespace RX.PharmacyBusiness.ETL.CRX582.Core
{
    using System;

    public class HipaaPortionOfClaim
    {
        private string prescriberIdQualifier = "01";

        //Claim keys:
        public DateTime ClaimDate { get; set; }
        public string OriginatingStoreId { get; set; }
        public string PrescriptionNumber { get; set; }
        public int RefillNumber { get; set; }
        public string ERxSplitFillNumber { get; set; }
        public string FillFactFillStateCode { get; set; }

        //HIPAA elements:
        public string CardholderAddress1 { get; set; }
        public string CardholderAddress2 { get; set; }
        public string CardholderCity { get; set; }
        public DateTime? CardholderDateOfBirth { get; set; }
        public string CardholderFirstName { get; set; }
        public string CardholderLastName { get; set; }
        public string CardholderMiddleInitial { get; set; }
        public string CardholderPhoneNumber { get; set; }
        public DateTime? OtherPayerDateOfBirth { get; set; }
        public string OtherPayerFirstName { get; set; }
        public string OtherPayerLastName { get; set; }
        public string OtherPayerMiddleInitial { get; set; }
        public string PatientAddress1 { get; set; }
        public string PatientAddress2 { get; set; }
        public string PatientCity { get; set; }
        public DateTime? PatientDateOfBirth { get; set; }
        public string PatientFirstName { get; set; }
        public string PatientLastName { get; set; }
        public string PatientMiddleInitial { get; set; }
        public string PatientPhoneNumber { get; set; }
        public string PrescriberAddress1 { get; set; }
        public string PrescriberAddress2 { get; set; }
        public string PrescriberCity { get; set; }
        public string PrescriberDEA { get; set; }
        public string PrescriberFirstName { get; set; }
        public long? PrescriberId { get; set; }
        public string PrescriberIdQualifier { get { return this.prescriberIdQualifier; } }
        public string PrescriberLastName { get; set; }

        public bool IsReversalTransaction()
        {
            return this.FillFactFillStateCode == FillFactStatus.NegativeAdjudication;
        }
    }
}
