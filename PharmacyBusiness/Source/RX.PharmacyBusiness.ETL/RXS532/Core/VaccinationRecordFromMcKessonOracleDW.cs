namespace RX.PharmacyBusiness.ETL.RXS532.Core
{
    using System;

    public class VaccinationRecordFromMcKessonOracleDW : VaccinationRecordForCDC
    {
        public string fd_facility_id { get; set; }
        public string rx_number { get; set; }
        public int refill_num { get; set; }
    }
}
