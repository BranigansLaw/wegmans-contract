namespace RX.PharmacyBusiness.ETL.RXS527.Core
{
    using System;

    public class McKessonRxRecord
    {
        public string PhoneNbr { get; set; }
        public string JobCode { get; set; }
        public string StoreNbr { get; set; }
        public string RxNbr { get; set; }
        public int RefillNum { get; set; }
        public int PartialFillSeq { get; set; }
        public string PatientNum { get; set; }
        public DateTime? PosSoldDate { get; set; }
        public string IsFluShot { get; set; }
        public int ready_dt { get; set; }

        public int StoreInteger
        {
            get
            {
                return Convert.ToInt32(this.StoreNbr);
            }
        }

        public int RxInteger
        {
            get
            {
                return Convert.ToInt32(this.RxNbr);
            }
        }
    }
}
