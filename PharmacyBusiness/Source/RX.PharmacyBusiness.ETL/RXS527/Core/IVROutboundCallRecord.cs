namespace RX.PharmacyBusiness.ETL.RXS527.Core
{
    public class IVROutboundCallRecord
    {
        public string PhoneNbr { get; set; }
        public string ContactType { get; set; }
        public string JobCode { get; set; }
        public string StoreNbr { get; set; }
        public string RxNbr { get; set; }
        public string DaysInBin { get; set; }
        public string Field01 { get; set; }
        public string Field02 { get; set; }
        public string Field03 { get; set; }
        public string ChainFlag { get; set; }
        public string Field04 { get; set; }
        public string Field05 { get; set; }
        public string Field06 { get; set; }
        public string Field07 { get; set; }
        public string Field08 { get; set; }
        public string Field09 { get; set; }
        public string Field10 { get; set; }
        public string Field11 { get; set; }
        public string Field12 { get; set; }
        public string Field13 { get; set; }
        public string Field14 { get; set; }
        public string Field15 { get; set; }
        public string Field16 { get; set; }
        public string Field17 { get; set; }
        public string Field18 { get; set; }
        public string Field19 { get; set; }
        public string PatientNum { get; set; }

        public IVROutboundCallRecord()
        {
        }

        public IVROutboundCallRecord(McKessonRxRecord mcKessonRxRecord)
        {
            this.PhoneNbr = mcKessonRxRecord.PhoneNbr;
            this.ContactType = "2";
            this.JobCode = mcKessonRxRecord.JobCode;
            this.StoreNbr = mcKessonRxRecord.StoreNbr;
            this.RxNbr = mcKessonRxRecord.RxNbr;
            this.DaysInBin = "0";
            this.Field01 = string.Empty;
            this.Field02 = string.Empty;
            this.Field03 = string.Empty;
            this.ChainFlag = "WEG";
            this.Field04 = string.Empty;
            this.Field05 = string.Empty;
            this.Field06 = string.Empty;
            this.Field07 = string.Empty;
            this.Field08 = string.Empty;
            this.Field09 = string.Empty;
            this.Field10 = string.Empty;
            this.Field11 = string.Empty;
            this.Field12 = string.Empty;
            this.Field13 = string.Empty;
            this.Field14 = string.Empty;
            this.Field15 = string.Empty;
            this.Field16 = string.Empty;
            this.Field17 = string.Empty;
            this.Field18 = string.Empty;
            this.Field19 = string.Empty;
            this.PatientNum = mcKessonRxRecord.PatientNum;
        }
    }
}
