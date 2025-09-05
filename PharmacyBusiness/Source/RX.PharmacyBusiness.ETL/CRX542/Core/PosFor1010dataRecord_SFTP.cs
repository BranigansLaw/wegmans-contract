namespace RX.PharmacyBusiness.ETL.CRX542.Core
{
    /// <summary>
    /// The output file for an SFTP to 1010data requires special formatting.
    /// </summary>
    public class PosFor1010dataRecord_SFTP
    {
        //Header row for file that gets SFTP'd to 1010data, which we can get by using those column names as class property names.
        //ID_STORENBR,ID_POSTXNBR,ID_POSOPERATOR,ID_POSTERMINAL,DTM_POSSOLD,ID_ERXRXNBR,N_ERXREFILLNBR,N_ERXPARTFILLSEQNBR,C_POSTRANSTYPE,MNY_RXPRICE,MNY_INSPAYS,MNY_CUSTPAYS

        public string ID_STORENBR { get; set; }
        public string ID_POSTXNBR { get; set; }
        public string ID_POSOPERATOR { get; set; }
        public string ID_POSTERMINAL { get; set; }
        public string DTM_POSSOLD { get; set; }
        public string ID_ERXRXNBR { get; set; }
        public string N_ERXREFILLNBR { get; set; }
        public string N_ERXPARTFILLSEQNBR { get; set; }
        public string C_POSTRANSTYPE { get; set; }
        public string MNY_RXPRICE { get; set; }
        public string MNY_INSPAYS { get; set; }
        public string MNY_CUSTPAYS { get; set; }

        public PosFor1010dataRecord_SFTP(PosRecord record)
        {
            //NOTE: Formatting for 1010data depends on if file will be SFTP'd or TenUp'd.
            //      This file gets SFTP'd, and formatting was set in place years ago.
            this.ID_STORENBR = record.Store_Num.ToString();
            this.ID_POSTXNBR = record.Transaction_Num.ToString();
            this.ID_POSOPERATOR = record.Operator.ToString();
            this.ID_POSTERMINAL = record.Terminal_Num.ToString();
            this.DTM_POSSOLD = record.Transaction_Date_Time.ToString("yyyy-MM-dd HH:mm:ss");
            this.ID_ERXRXNBR = record.Rx_Transaction_Num.ToString();
            this.N_ERXREFILLNBR = record.Refill_Num.ToString();
            this.N_ERXPARTFILLSEQNBR = record.Partial_Fill_Sequence_Num.ToString();
            this.C_POSTRANSTYPE = record.POS_TxType.ToString();
            this.MNY_RXPRICE = record.Total_Price.ToString("0.00");
            this.MNY_INSPAYS = record.Insurance_Amt.ToString("0.00");
            this.MNY_CUSTPAYS = record.Copay_Amt.ToString("0.00");
        }
    }
}
