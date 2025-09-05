namespace RX.PharmacyBusiness.ETL.RXS516.Core
{
    using System;

    public class APICallFailures : UnresolvedPosExceptionRecord
    {
        public DateTime APICallMadeDate { get; set; }
        public string APIName { get; set; }
        public string APIExceptionMessage { get; set; }
    }
}
