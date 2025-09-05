namespace RX.PharmacyBusiness.ETL.RXS532.Core
{
    using System;

    public class VaccinationRecordForCDC
    {
        public string VAX_EVENT_ID { get; set; }
        public string EXT_TYPE { get; set; }
        public string PPRL_ID { get; set; }
        public int RECIP_ID { get; set; }
        public string RECIP_FIRST_NAME { get; set; }
        public string RECIP_MIDDLE_NAME { get; set; }
        public string RECIP_LAST_NAME { get; set; }
        public string RECIP_DOB { get; set; }
        public string RECIP_SEX { get; set; }
        public string RECIP_ADDRESS_STREET { get; set; }
        public string RECIP_ADDRESS_STREET_2 { get; set; }
        public string RECIP_ADDRESS_CITY { get; set; }
        public string RECIP_ADDRESS_COUNTY { get; set; }
        public string RECIP_ADDRESS_STATE { get; set; }
        public string RECIP_ADDRESS_ZIP { get; set; }
        public string RECIP_RACE_1 { get; set; }
        public string RECIP_RACE_2 { get; set; }
        public string RECIP_RACE_3 { get; set; }
        public string RECIP_RACE_4 { get; set; }
        public string RECIP_RACE_5 { get; set; }
        public string RECIP_RACE_6 { get; set; }
        public string RECIP_ETHNICITY { get; set; }
        public string ADMIN_DATE { get; set; }
        public string CVX { get; set; }
        public string NDC { get; set; }
        public string MVX { get; set; }
        public string LOT_NUMBER { get; set; }
        public string VAX_EXPIRATION { get; set; }
        public string VAX_ADMIN_SITE { get; set; }
        public string VAX_ROUTE { get; set; }
        public string DOSE_NUM { get; set; }
        public string VAX_SERIES_COMPLETE { get; set; }
        public string RESPONSIBLE_ORG { get; set; }
        public string ADMIN_NAME { get; set; }
        public string VTRCKS_PROV_PIN { get; set; }
        public string ADMIN_TYPE { get; set; }
        public string ADMIN_ADDRESS_STREET { get; set; }
        public string ADMIN_ADDRESS_STREET_2 { get; set; }
        public string ADMIN_ADDRESS_CITY { get; set; }
        public string ADMIN_ADDRESS_COUNTY { get; set; }
        public string ADMIN_ADDRESS_STATE { get; set; }
        public string ADMIN_ADDRESS_ZIP { get; set; }
        public string VAX_REFUSAL { get; set; }
        public string CMORBID_STATUS { get; set; }
        public string SEROLOGY { get; set; }
    }
}
