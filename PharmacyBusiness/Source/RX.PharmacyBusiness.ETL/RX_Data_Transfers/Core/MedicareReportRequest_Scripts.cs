namespace RX.PharmacyBusiness.ETL.RX_Data_Transfers.Core
{
    using System;

    public class MedicareReportRequest_Scripts
    {
        public string USER_ID { get; set; }
        public string REPORT_SERVER_URL { get; set; }
        public string REPORT_FOLDER { get; set; }
        public string REPORT_NAME { get; set; }
        public string REPORT_PARAMETERS { get; set; }
        public string DATABASE_PROCEDURE_NAME { get; set; }
        public DateTime REPORT_ACTIVITY_DATE { get; set; }
    }
}
