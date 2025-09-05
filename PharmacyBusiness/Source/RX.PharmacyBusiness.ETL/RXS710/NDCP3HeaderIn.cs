namespace RX.PharmacyBusiness.ETL.RXS710
{
    using System;
    using System.Collections.Generic;

    public class NDCP3HeaderIn
    {
        public string NcpdpNumber { get; set; }
        public string PharmacyAddress1 { get; set; }
        public string PharmacyCity { get; set; }
        public string PharmacyState { get; set; }
        public string PharmacyZip { get; set; }
        public string PharmacyPhoneNumber { get; set; }
    }
}
