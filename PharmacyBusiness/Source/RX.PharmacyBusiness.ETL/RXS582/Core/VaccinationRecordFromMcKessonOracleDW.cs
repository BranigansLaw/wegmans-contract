namespace RX.PharmacyBusiness.ETL.RXS582.Core
{
    using System;

    public class VaccinationRecordFromMcKessonOracleDW : VaccinationRecordForSTC
    {
        public int refill_num { get; set; }
    }
}
