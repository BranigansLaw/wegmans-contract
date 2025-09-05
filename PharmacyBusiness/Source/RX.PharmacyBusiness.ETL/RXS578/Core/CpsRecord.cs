namespace RX.PharmacyBusiness.ETL.RXS578.Core
{
    using System;

    public class CpsRecord
    {
        public string StoreNbr { get; set; }
        public string ProgramName { get; set; }
        public string KeyDesc { get; set; }
        public string KeyValue { get; set; }
        public string StatusName { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
