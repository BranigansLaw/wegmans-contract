namespace RX.PharmacyBusiness.ETL.RX_Data_Transfers.Core
{
    public class OracleSchemaRowCount
    {
        public string TABLE_OWNER { get; set; }
        public string TABLE_NAME { get; set; }
        public string DATE_QUERIED { get; set; }
        public string ROW_COUNT { get; set; }
    }
}
