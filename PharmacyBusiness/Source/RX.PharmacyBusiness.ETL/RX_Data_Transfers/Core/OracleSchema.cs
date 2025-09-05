namespace RX.PharmacyBusiness.ETL.RX_Data_Transfers.Core
{
    public class OracleSchema
    {
        public string table_owner { get; set; }
        public string table_name { get; set; }
        public string column_name { get; set; }
        public string column_id { get; set; }
        public string column_data_type { get; set; }
        public string column_comments { get; set; }
        public string is_table_key { get; set; }
    }
}
