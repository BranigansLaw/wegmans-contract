namespace Library.TenTenInterface.UploadsToTenTen
{
    public class TenTenTableSpecifications
    {
        public TenTenTableSpecifications(string uploadTableNameAndPath, bool doesUploadReplaceAllData, string queryableTableNameAndPath, string tenTenTableDefinition)
        {
            UploadTableNameAndPath = uploadTableNameAndPath;
            DoesUploadReplaceAllData = doesUploadReplaceAllData;
            QueryableTableNameAndPath = queryableTableNameAndPath;
            TenTenTableDefinition = tenTenTableDefinition;
        }

        /// <summary>
        /// Some uploads go to back-end tables nobody has access to, and back-end processes may sort and segment the data and merge it into final queryable tables.
        /// </summary>
        public string UploadTableNameAndPath { get; set; }

        /// <summary>
        /// If set to true, then the upload will replace all data in the table. If set to false, then the upload will append to the table.
        /// </summary>
        public bool DoesUploadReplaceAllData { get; set; }

        /// <summary>
        /// The table name and path that is queryable by the user.
        /// </summary>
        public string QueryableTableNameAndPath { get; set; }

        /// <summary>
        /// The column name that contains the date of the data.
        /// Unfortunately, most tables were created by non-programmers and do not contain a column for Uploaded Date, or something that relates to RunFor Date.
        /// So, this property is meant to identify the next best table column containing a date value that can be associated with an Uploaded Date, or something that relates to RunFor Date.
        /// If this table gets fully replaced with every upload, then this property is not needed.But, it is needed if the table gets appended to with every upload.
        /// </summary>
        public string? ColumnNameContainingDataDate { get; set; }

        /// <summary>
        /// Some data dates might be N number of days behind the Run For Date, and this property will provide the value the data date field will have for a given Run For Date.
        /// </summary>
        public DateOnly? DataDateAssociatedWithRunForDate { get; set; }

        /// <summary>
        /// Contains the TenTen table definition in XML format otherwise known as a Table Tree.
        /// </summary>
        public string TenTenTableDefinition { get; set; }
    }
}
