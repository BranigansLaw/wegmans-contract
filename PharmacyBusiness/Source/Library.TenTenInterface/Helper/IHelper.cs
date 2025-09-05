using Library.TenTenInterface.DownloadsFromTenTen;
using Library.TenTenInterface.Response;

namespace Library.TenTenInterface.Helper
{
    public interface IHelper
    {
        /// <summary>
        /// Generates the XML for a TenTen upload for the given <paramref name="data"/> making sure it doesn't increase to over <paramref name="maxSizeMb"/> MB
        /// </summary>
        IEnumerable<string> CreateXmlBatches<T>(string body, IEnumerable<T> data, Func<T, string> convertToTableLine, int maxSizeMb);

        /// <summary>
        /// Return the XML file template with name <paramref name="templateName"/> from the file system
        /// </summary>
        Task<string> ReadXmlTemplateAsync(string templateName);

        /// <summary>
        /// Generates a row count query coded in proprietary 1010 XML language to be run within 1010data and when it is run it will return data results in XML format (that resembles HTML).
        /// </summary>
        /// <param name="tableName">Table name without the namespace file path info, which will be set depending on which environment this program runs in.</param>
        /// <param name="tabulationBreakColumnNames">An array of column names to tabulate aggregate data. Typically this will be just one date column.</param>
        /// <returns></returns>
        public string GenerateRowCountQueryReturningXmlResults(string tableName, string[] tabulationBreakColumnNames);

        /// <summary>
        /// Generates a data extract query coded in proprietary 1010 XML language to be run within 1010data and when it is run it will return data results in delimited format easily transformed into collections.
        /// </summary>
        /// <param name="downloadXmlQuery"></param>
        /// <param name="columnNames"></param>
        /// <returns></returns>
        public string GenerateDataExtractQueryReturningDelimitedResultsForTransformingToCollections(string downloadXmlQuery, string[] columnNames);

        /// <summary>
        /// Converts a delimited string into a collection of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="delimitedString"></param>
        /// <returns></returns>
        public IEnumerable<T> ConvertStringToCollection<T>(string delimitedString);

        /// <summary>
        /// Generates a row count query coded in proprietary 1010 XML language to be run within 1010data and when it is run it will return data results in delimited format (surrounded by a CDATA tag).
        /// </summary>
        /// <param name="tableName">Table name without the namespace file path info, which will be set depending on which environment this program runs in.</param>
        /// <param name="tabulationBreakColumnNames">An array of column names to tabulate aggregate data. Typically this will be just one date column.</param>
        /// <returns></returns>
        public string GenerateRowCountQueryReturningCsvResults(string tableName, string[] tabulationBreakColumnNames);

        /// <summary>
        /// Extracts row count from the response of a row count query.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public int GetRowCountFromQueryResponse(ITenTenQueryResponse response, DateOnly runFor);

        /// <summary>
        /// Generates a data extract query coded in proprietary 1010 XML language to be run within 1010data and when it is run it will return data results in XML format (that resembles HTML).
        /// </summary>
        /// <param name="downloadXmlQuery"></param>
        /// <param name="columnNames"></param>
        /// <param name="extractDelimiter"></param>
        /// <returns></returns>
        public string GenerateDataExtractQueryReturningCsvResults(string downloadXmlQuery, string[] columnNames, string extractDelimiter);

        /// <summary>
        /// Generates a rollup query coded in proprietary 1010 XML language to be run within 1010data and when it is run it will return data results in XML format (that resembles HTML).
        /// </summary>
        /// <param name="folderOfIndividualTablesByDates"></param>
        /// <param name="rollupTableFullPath"></param>
        /// <param name="rollupTableTitle"></param>
        /// <returns></returns>
        public string GenerateRollupQuery(
            string folderOfIndividualTablesByDates,
            string rollupTableFullPath,
            string rollupTableTitle);

        /// <summary>
        /// Generates a segment and sort query coded in proprietary 1010 XML language to be run within 1010data and when it is run it will return data results in XML format (that resembles HTML).
        /// </summary>
        /// <param name="tableFullPath"></param>
        /// <param name="tableTitle"></param>
        /// <param name="segmentColumnNames"></param>
        /// <param name="sortColumnNames"></param>
        /// <returns></returns>
        string GenerateSegmentAndSortQuery(
            string tableFullPath,
            string tableTitle,
            string[] segmentColumnNames,
            string[] sortColumnNames);

        /// <summary>
        /// Generates a make directory query coded in proprietary 1010 XML language to be run within 1010data and when it is run it will return data results in XML format (that resembles HTML).
        /// </summary>
        /// <param name="parentDirectoryPath"></param>
        /// <param name="newSubfolderName"></param>
        /// <param name="newSubfolderTitle"></param>
        /// <returns></returns>
        string GenerateMakeDirectoryXml(
            string parentDirectoryPath, 
            string newSubfolderName,
            string newSubfolderTitle);

        /// <summary>
        /// Query results could be CSV or XML. This method writes the results to a file.
        /// </summary>
        /// <param name="response"></param>
        /// <param name="dataExtractFileSpecifications"></param>
        void WriteResultsToFile(ITenTenQueryResponse response, DataExtractFileSpecifications dataExtractFileSpecifications);

        /// <summary>
        /// Transforms XML used to upload to dated tables to a one time initial rollup table with default data.
        /// </summary>
        string TransformUploadXmlForInitialRollupTableWithDefaultData(DateOnly runFor, string uploadDatedTablesXml);
    }
}
