namespace RX.PharmacyBusiness.ETL.CRX542.Core
{
    /// <summary>
    /// The output file for a TenUp to 1010data requires special formatting.
    /// </summary>
    public class PosFor1010dataRecord_TENUP
    {
        //Header row for file that gets TenUp'd to 1010data, which we can get by using those column names as class property names in the order listed here.
        public string store_num { get; set; }
        public string rx_num { get; set; }
        public string refill_num { get; set; }
        public string part_seq_num { get; set; }
        public string id_posterminal { get; set; }
        public string dtm_possold { get; set; }
        public string dtm_possold_time { get; set; }
        public string id_posoperator { get; set; }
        public string id_postxnbr { get; set; }
        public string c_postranstype { get; set; }
        public string mny_rxprice { get; set; }
        public string mny_inspays { get; set; }
        public string mny_custpays { get; set; }

        public PosFor1010dataRecord_TENUP(PosRecord record)
        {
            //NOTE: Formatting for 1010data depends on if file will be SFTP'd or TenUp'd, and this file gets TenUp'd.
            this.store_num = record.Store_Num.ToString();
            this.rx_num = record.Rx_Transaction_Num.ToString();
            this.refill_num = record.Refill_Num.ToString();
            this.part_seq_num = record.Partial_Fill_Sequence_Num.ToString();
            this.id_posterminal = record.Terminal_Num.ToString();
            this.dtm_possold = TenTenHelper.FormatDateWithoutTimeForTenUp(record.Transaction_Date_Time);
            this.dtm_possold_time = TenTenHelper.FormatDateWithTimeForTenUp(record.Transaction_Date_Time);
            this.id_posoperator = record.Operator.ToString();
            this.id_postxnbr = record.Transaction_Num.ToString();
            this.c_postranstype = record.POS_TxType.ToString();
            this.mny_rxprice = record.Total_Price.ToString("0.00");
            this.mny_inspays = record.Insurance_Amt.ToString("0.00");
            this.mny_custpays = record.Copay_Amt.ToString("0.00");
        }
    }
}
