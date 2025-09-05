namespace Library.DataFileInterface.DataFileReader
{
    public interface IDataRecord
    {
        /// <summary>
        /// Set the properties of the record from the file row.
        /// NOTE: DO NOT include data in reject data logging which may contain ePHI/HIPAA data, but you can include where in the file the reject data is located.
        /// </summary>
        /// <param name="dataFileColumns">Array of data fields from vendor data file.</param>
        /// <param name="runFor">Run Date (the date the job was intended to be run on.</param>
        void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns, DateOnly runFor);
    }
}
