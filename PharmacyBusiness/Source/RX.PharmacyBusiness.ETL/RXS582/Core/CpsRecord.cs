namespace RX.PharmacyBusiness.ETL.RXS582.Core
{
    using System;

    public class CpsRecord
    {
        public string StoreNbr { get; set; }
        public int PatientNum { get; set; }
        public string RxNbr { get; set; }
        public int RefillNbr { get; set; }
        public string KeyDesc { get; set; }
        public string KeyValue { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
