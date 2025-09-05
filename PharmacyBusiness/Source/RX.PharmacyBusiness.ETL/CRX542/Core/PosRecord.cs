namespace RX.PharmacyBusiness.ETL.CRX542.Core
{
    using System;

    public class PosRecord
    {
        public int Store_Num { get; set; }
        public int Terminal_Num { get; set; }
        public DateTime Transaction_Date_Time { get; set; }
        public int Operator { get; set; }
        public int Transaction_Num { get; set; }
        public int Rx_Transaction_Num { get; set; }
        public int Refill_Num { get; set; }
        public char POS_TxType { get; set; }
        public decimal Total_Price { get; set; }
        public decimal Insurance_Amt { get; set; }
        public decimal Copay_Amt { get; set; }
        public int Partial_Fill_Sequence_Num { get; set; }
        public string Order_Num { get; set; }
        public string Mail_Flag { get; set; }
    }
}
