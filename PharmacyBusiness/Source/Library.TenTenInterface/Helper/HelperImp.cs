using Library.TenTenInterface.DownloadsFromTenTen;
using Library.TenTenInterface.Response;
using Microsoft.Extensions.Options;
using System.Text;

namespace Library.TenTenInterface.Helper
{
    public class HelperImp : IHelper
    {
        private readonly IOptions<TenTenConfig> _tenTenConfig;
        public const string _queryColumnDelimiter = "|";
        public const string _queryRowDelimiter = "^";

        /// <summary>
        /// For a given table name and path, generates an XML query that will return the row count for the table in XML data format.
        /// </summary>
        private const string apiTemplateRowCountQueryReturningXmlResults = @"<in>
  <name>{0}</name>
	<ops>
	    <tabu breaks=""{1}"">
            <tcol fun=""cnt"" name=""row_count"" label=""Row Count""/>
        </tabu>
        <colord cols=""{2}row_count""/>
	</ops>
    <cols>{3}
        <col>row_count</col>
    </cols>
    <format type=""xml"" values=""formatted""/>
</in>";

        /// <summary>
        /// For a given table name and path, generates an XML query that will return the row count for the table in CSV data format.
        /// </summary>
        private const string apiTemplateRowCountQueryReturningCsvResults = @"<in>
  <name>{0}</name>
	<ops>
	    <tabu breaks=""{1}"">
            <tcol fun=""cnt"" name=""row_count"" format=""type:nocommas"" label=""Row Count""/>
        </tabu>
        <colord cols=""{2}row_count""/>
	</ops>
    <cols>{3}
        <col>row_count</col>
    </cols>
    <format type=""csv"" values=""formatted"">
        <sep>{4}</sep>
    </format>
</in>";

        /// <summary>
        /// For a given table name and path and query, generates an XML query that will return query results in delimited data format intended to be easily transformed into collections.
        /// </summary>
        private const string apiTemplateDataExtractQueryReturningDelimitedResultsForTransformingToCollections = @"<in>
  <name>default.lonely</name>
	<ops>{0}</ops>
    <cols>{1}</cols>
    <format type=""csv"" values=""formatted"">
        <sep>{2}</sep>
        <linesep>{3}</linesep>
    </format>
</in>";

        /// <summary>
        /// Simplified XML to run any query and return the results in CSV format.
        /// </summary>
        private const string apiTemplateDataExtractQueryReturningCsvResults = @"<in>
  <name>default.lonely</name>
	<ops>{0}</ops>
    <cols>{1}</cols>
    <format type=""csv"" values=""formatted"">
        <sep>{2}</sep>
    </format>
</in>";

        /// <summary>
        /// Given a folder containing individual tables by dates, generates an XML query that will roll up the tables into a single table.
        /// </summary>
        private const string apiTemplateRollupQuery = @"<in>
  <name>default.lonely</name>
	<ops>
	  <loop with_=""tables_by_dates"">
        <outer>
          <directory folder=""{0}""/>
          <sel value=""type='tab'""/>
          <sel value=""beginswith(path;'{0}.d')""/>
          <sort cols=""path""/>
          <willbe name=""paths"" value=""g_splice(;;;path;',';)""/>
          <sel value=""g_first1(;;)""/>
          <colord cols=""paths""/>
        </outer>
        <inner>
          <do action_=""combine_tabs"" from=""{3}"" value_=""@paths"" path=""{1}"" title=""{2}"" owner=""{4}"" commit=""1"" inherit_users=""1""/>
        </inner>
      </loop>
	</ops>
    <cols>
        <col>c1</col>
    </cols>
    <format type=""csv"" values=""formatted"">
        <sep>,</sep>
    </format>
</in>";

        /// <summary>
        /// Given a table, generates an XML query that will segment and sort that same table.
        /// </summary>
        private const string apiTemplateSegmentAndSortQuery = @"<in>
  <name>default.lonely</name>
	<ops>
	  <library>
        <block name=""do_seg_and_sort"" saved_path="""">
          <do action_=""savetable"" value_=""@saved_path"" path=""{0}"" title=""{1}"" owner=""{2}"" segby_=""{3}"" replace_=""1"" inherit_=""1"">
            <base table=""{0}""/>{4}
	      </do>
        </block>
      </library>
      <insert block=""do_seg_and_sort"" saved_path=""{0}""/>
	</ops>
    <cols>
        <col>c1</col>
    </cols>
    <format type=""csv"" values=""formatted"">
        <sep>,</sep>
    </format>
</in>";

        /// <summary>
        /// For a given parent folder path, generates an XML that will create a new sub directory.
        /// Useful when first creating a new upload directory.
        /// </summary>
        private const string apiTemplateMakeNewDirectory = @"<in>
  <name>{0}.{1}</name>
  <users type=""inherit"" />
  <upload type=""inherit"" />
  <title>{2}</title>
</in>";

        public HelperImp(IOptions<TenTenConfig> tenTenConfig)
        {
            _tenTenConfig = tenTenConfig ?? throw new ArgumentNullException(nameof(tenTenConfig));
        }

        /// <inheritdoc />
        public IEnumerable<string> CreateXmlBatches<T>(string body, IEnumerable<T> data, Func<T, string> convertToTableLine, int maxSizeMb)
        {
            ICollection<string> toReturn = new List<string>();

            StringWriter stringWriter = new();
            int currXmlBodySize = Encoding.UTF8.GetByteCount(body);
            foreach (T item in data)
            {
                string itemAsString = convertToTableLine(item);
                int xmlDataSize = currXmlBodySize + Encoding.UTF8.GetByteCount(itemAsString);

                if (xmlDataSize > maxSizeMb * 1024 * 1024)
                {
                    toReturn.Add(string.Format(body, stringWriter.ToString()));

                    stringWriter = new StringWriter();
                    currXmlBodySize = Encoding.UTF8.GetByteCount(body);
                }

                stringWriter.Write(itemAsString);
                currXmlBodySize += Encoding.UTF8.GetByteCount(itemAsString);
            }

            toReturn.Add(string.Format(body, stringWriter.ToString()));

            return toReturn;
        }

        /// <inheritdoc />
        public string GenerateRowCountQueryReturningXmlResults(string tableNameAndPath, string[] tabulationBreakColumnNames)
        {
            string tabulationColNamesCsv = string.Join(",", tabulationBreakColumnNames);
            string tabulationColTags = string.Empty;

            foreach (string colName in tabulationBreakColumnNames)
            {
                tabulationColTags += $"<col>{colName}</col>";
            }

            string xmlQueryFor1010 = string.Format(apiTemplateRowCountQueryReturningXmlResults,
                tableNameAndPath,
                tabulationColNamesCsv,
                string.IsNullOrEmpty(tabulationColNamesCsv) ? string.Empty : tabulationColNamesCsv + ",",
                tabulationColTags);

            // Remove newlines and carriage returns.
            return xmlQueryFor1010.Replace("\n", "").Replace("\r", "");
        }

        /// <inheritdoc />
        public string GenerateRowCountQueryReturningCsvResults(string tableNameAndPath, string[] tabulationBreakColumnNames)
        {
            string tabulationColNamesCsv = string.Join(",", tabulationBreakColumnNames);
            string tabulationColTags = string.Empty;

            foreach (string colName in tabulationBreakColumnNames)
            {
                tabulationColTags += $"<col>{colName}</col>";
            }

            string xmlQueryFor1010 = string.Format(apiTemplateRowCountQueryReturningCsvResults,
                tableNameAndPath,
                tabulationColNamesCsv,
                string.IsNullOrEmpty(tabulationColNamesCsv) ? string.Empty : string.Format("{0},", tabulationColNamesCsv),
                tabulationColTags,
                _queryColumnDelimiter);

            // Remove newlines and carriage returns.
            return xmlQueryFor1010.Replace("\n", "").Replace("\r", "");
        }

        /// <inheritdoc />
        public int GetRowCountFromQueryResponse(ITenTenQueryResponse response, DateOnly runFor)
        {
            if (response.Csv is not null)
            {
                string[] csvLines = response.Csv.Replace("<![CDATA[", "").Replace("]]>", "").Split("\n");
                if (Int32.TryParse(csvLines[1], out int returnRowCount))
                    return returnRowCount;
            }
            else if (response.Xml is not null)
            {
                throw new NotImplementedException();
            }

            return 0;
        }

        /// <inheritdoc />
        public string GenerateDataExtractQueryReturningDelimitedResultsForTransformingToCollections(string downloadXmlQuery, string[] columnNames)
        {
            string colTags = string.Empty;

            foreach (string colName in columnNames)
            {
                colTags += $"<col>{colName}</col>";
            }

            string xmlQueryFor1010 = string.Format(apiTemplateDataExtractQueryReturningDelimitedResultsForTransformingToCollections,
                downloadXmlQuery,
                colTags,
                _queryColumnDelimiter,
                _queryRowDelimiter);

            // Remove newlines and carriage returns.
            return xmlQueryFor1010.Replace("\n", "").Replace("\r", "");
        }

        public IEnumerable<T> ConvertStringToCollection<T>(string delimitedString)
        {
            ICollection<T> toReturn = new List<T>();

            string[] rows = delimitedString.Split(_queryRowDelimiter);
            int rowCounter = -1;

            foreach (string row in rows)
            {
                rowCounter++;

                if (rowCounter == 0) //Ignore header row
                    continue;

                string[] columns = row.Split(_queryColumnDelimiter);

                if (columns.Length == 0)
                    continue;

                //TODO: You might want a handler here for when the number of columns is less than or greater than the number you're expecting too. Not sure how often this happens in the CSV export, but something to consider.
#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
                T item = (T)Activator.CreateInstance(typeof(T), columns);
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

                if (item is not null)
                    toReturn.Add(item);
            }
            return toReturn;
        }

        /// <inheritdoc />
        public string GenerateDataExtractQueryReturningCsvResults(string downloadXmlQuery, string[] columnNames, string extractDelimiter)
        {
            string colTags = string.Empty;

            foreach (string colName in columnNames)
            {
                colTags += $"<col>{colName}</col>";
            }

            string xmlQueryFor1010 = string.Format(apiTemplateDataExtractQueryReturningCsvResults,
                downloadXmlQuery,
                colTags,
                extractDelimiter);

            // Remove newlines and carriage returns.
            return xmlQueryFor1010.Replace("\n", "").Replace("\r", "");
        }

        /// <inheritdoc />
        public string GenerateRollupQuery(
            string folderOfIndividualTablesByDates,
            string rollupTableFullPath,
            string rollupTableTitle)
        {
            string xmlQueryFor1010 = string.Format(apiTemplateRollupQuery,
                folderOfIndividualTablesByDates,
                rollupTableFullPath,
                rollupTableTitle,
                @"{str_to_lst(@tables_by_dates.paths;',')}", //The curly braces here are required for 1010data proprietary code execution running within 1010data, but are NOT intended for C# code here in the string.Format to make a substitution.
                _tenTenConfig.Value.Username);

            // Remove newlines and carriage returns.
            return xmlQueryFor1010.Replace("\n", "").Replace("\r", "");
        }

        /// <inheritdoc />
        public string GenerateSegmentAndSortQuery(
            string tableFullPath,
            string tableTitle,
            string[] segmentColumnNames,
            string[] sortColumnNames)
        {
            string sortXml = string.Empty;
            if (sortColumnNames.Length > 0)
            {
                sortXml = string.Format(@"<sort cols=""{0}""/>", string.Join(",", sortColumnNames));
            }
            string xmlQueryFor1010 = string.Format(apiTemplateSegmentAndSortQuery,
                tableFullPath,
                tableTitle,
                _tenTenConfig.Value.Username,
                string.Join(",", segmentColumnNames),
                sortXml
                );
            // Remove newlines and carriage returns.
            return xmlQueryFor1010.Replace("\n", "").Replace("\r", "");
        }

        /// <inheritdoc />
        public string GenerateMakeDirectoryXml(string parentDirectoryPath, string newSubfolderName, string newSubfolderTitle)
        {
            string xmlApiFor1010 = string.Format(apiTemplateMakeNewDirectory,
                parentDirectoryPath,
                newSubfolderName,
                newSubfolderTitle);

            // Remove newlines and carriage returns.
            return xmlApiFor1010.Replace("\n", "").Replace("\r", "");
        }

        /// <inheritdoc />
        public void WriteResultsToFile(ITenTenQueryResponse response, DataExtractFileSpecifications fileSpecifications)
        {
            if (response.Csv is not null)
            {
                var filePath = Path.Combine(_tenTenConfig.Value.OutputFileLocation, fileSpecifications.FileName);
                var output = response.Csv;

                if (fileSpecifications.ReplacementHeader is not null)
                {
                    var csvLines = output.Split(fileSpecifications.LineTerminator);

                    if (csvLines.Length > 0)
                    {
                        csvLines[0] = fileSpecifications.ReplacementHeader;
                    }

                    output = string.Join(fileSpecifications.LineTerminator, csvLines);
                }

                using (StreamWriter sw = File.CreateText(filePath))
                {
                    sw.Write(output);
                }
            }
        }

        /// <inheritdoc />
        public Task<string> ReadXmlTemplateAsync(string templateName)
        {
            return File.ReadAllTextAsync($"{_tenTenConfig.Value.ExecutableBasePath}/UploadXmlTemplateHandlers/XmlTemplates/{templateName}");
        }

        /// <inheritdoc />
        public string TransformUploadXmlForInitialRollupTableWithDefaultData(DateOnly runFor, string uploadDatedTablesXml)
        {
            string xmlApiFor1010 = uploadDatedTablesXml.Replace($".d{runFor.ToString("yyyyMMdd")}", "_rollup");
            xmlApiFor1010 = xmlApiFor1010.Replace($"{runFor.ToString("yyyyMMdd")}", "Rollup");

            // Remove newlines and carriage returns.
            return xmlApiFor1010.Replace("\n", "").Replace("\r", "");
        }
    }
}