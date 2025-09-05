namespace RX.PharmacyBusiness.ETL.RX_Data_Transfers.Core
{
    using System.Text.RegularExpressions;

    public class MedicareReportRequest_1010
    {
        public string report_request_datetime { get; set; }
        public string report_request_parameters_json { get; set; }
        public string pd_patient_num { get; set; }
        public string ndc_quantity_dayssupply_acuterefilllimit_array { get; set; }
        public string store_num { get; set; }
        public string plan_service_area_array { get; set; }
        public string plan_type_comparison_filter_array { get; set; }
        public string report_user_info { get; set; }
        public string report_server_url { get; set; }
        public string report_folder { get; set; }
        public string report_name { get; set; }
        public string database_procedure_name { get; set; }

        public MedicareReportRequest_1010(MedicareReportRequest_Scripts record)
        {
            //NOTE: Formatting for 1010data depends on if file will be SFTP'd or TenUp'd, and this file gets TenUp'd.
            this.report_request_datetime = TenTenHelper.FormatDateWithTimeForTenUp(record.REPORT_ACTIVITY_DATE);
            this.report_request_parameters_json = record.REPORT_PARAMETERS;
            this.pd_patient_num = string.Empty;
            this.ndc_quantity_dayssupply_acuterefilllimit_array = string.Empty;
            this.store_num = string.Empty;
            this.plan_service_area_array = string.Empty;
            this.plan_type_comparison_filter_array = string.Empty;
            this.report_user_info = record.USER_ID;
            this.report_server_url = record.REPORT_SERVER_URL;
            this.report_folder = record.REPORT_FOLDER;
            this.report_name = record.REPORT_NAME;
            this.database_procedure_name = record.DATABASE_PROCEDURE_NAME;

            //var jsonProperties = record.REPORT_PARAMETERS.Split(',');
            var jsonProperties = Regex.Split(record.REPORT_PARAMETERS, ",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");

            foreach (var jsonProperty in jsonProperties)
            {
                var jsonPropertyNameValuePairs = jsonProperty.Split(':');

                switch (jsonPropertyNameValuePairs[0].Trim())
                {
                    case "MyPatientId":
                        this.pd_patient_num = jsonPropertyNameValuePairs[1];
                        break;
                    case "MyNDCs":
                        this.ndc_quantity_dayssupply_acuterefilllimit_array = jsonPropertyNameValuePairs[1].Replace("\"", string.Empty);
                        break;
                    case "MyStoreId":
                        this.store_num = jsonPropertyNameValuePairs[1];
                        break;
                    case "MyPlanServiceArea":
                        this.plan_service_area_array = jsonPropertyNameValuePairs[1].Replace("\"", string.Empty);
                        break;
                    case "MyPlanType":
                        this.plan_type_comparison_filter_array = jsonPropertyNameValuePairs[1].Replace("\"", string.Empty);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
