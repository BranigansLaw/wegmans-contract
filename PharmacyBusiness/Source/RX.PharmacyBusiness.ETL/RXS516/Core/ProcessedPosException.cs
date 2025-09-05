namespace RX.PharmacyBusiness.ETL.RXS516.Core
{
    public class ProcessedPosException
    {
        public string resolved_datetime { get; set; }
        public string userdata_uid { get; set; }
        public int exception_date { get; set; }
        public int store_num { get; set; }
        public int rx_num { get; set; }
        public int refill_num { get; set; }
        public int part_seq_num { get; set; }
        public int classification_code { get; set; }
        public string in_wcb { get; set; }
        public int is_resolved { get; set; }
    }
}
