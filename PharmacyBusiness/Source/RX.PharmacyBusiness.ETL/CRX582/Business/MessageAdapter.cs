namespace RX.PharmacyBusiness.ETL.CRX582.Business
{
    using System;

    public class MessageAdapter
    {
        private readonly string message;

        public MessageAdapter(string message)
        {
            this.message = message;
        }

        public string D5 { get { return RequestMessageParser.GetValue(this.message, "D5"); } }

        public decimal D9 { get { return RequestMessageParser.ToMoney(RequestMessageParser.GetValue(this.message, "D9")); } }
        
        public string DO { get { return RequestMessageParser.GetValue(this.message, "DO"); } }

        public decimal DQ { get { return RequestMessageParser.ToMoney(RequestMessageParser.GetValue(this.message, "DQ")); } }
    }
}