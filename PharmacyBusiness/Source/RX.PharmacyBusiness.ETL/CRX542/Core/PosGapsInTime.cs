using System;

namespace RX.PharmacyBusiness.ETL.CRX542.Core
{
    public class PosGapsInTime
    {
        public DateTime RunDate { get; set; }
        public int MinutesGap { get; set; }
        public DateTime StartOfMissingPosRecords { get; set; }
        public DateTime EndOfMissingPosRecords { get; set; }
    }
}
