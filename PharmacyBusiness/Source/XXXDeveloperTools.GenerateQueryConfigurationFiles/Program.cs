using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        // Create list of scripts that already have files
        ISet<string> filesToIgnore = new HashSet<string>
        {
            "Sold_Detail_YYYYMMDD.sql",
            "SelectSoldDetail.sql",
            "PetPtNums_YYYYMMDD.sql",
            "Omnisys_Claim_YYYYMMDD.sql",
            "InvAdj_YYYYMMDD.sql",
            "Wegmans_HPOne_Pharmacies_YYYYMMDD.sql",
            "GetSmartOrderPointsMinMax.sql",
            "FillFactTest.sql",
            "FDS_Pharmacies.sql",
            "DurConflict_YYYYMMDD.sql",
            "Deceased_YYYYMMDD.sql",
            "CustomerQuery.sql",
        };

        foreach (string dir in Directory.GetFiles("../../../../Library.SnowflakeInterface/SnowflakeSQL"))
        {
            Console.WriteLine(dir);
            string originalQueryFileName = Path.GetFileName(dir);

            if (!filesToIgnore.Contains(originalQueryFileName))
            {
                string fileContents = "";
                using (StreamReader reader = File.OpenText(dir))
                {
                    fileContents = reader.ReadToEnd();
                }

                // Extract file name for QueryConfigration file name
                string dataFileClassName = originalQueryFileName
                    .Replace(".sql", "")
                    .Replace("_YYYYMMDD", "")
                    .Replace("_", "");

                // Read SQL file contents and extract all arguments
                string argumentProperties = "";
                string argumentPropertySetups = "";
                ISet<string> addedParamters = new HashSet<string>();
                foreach (Match match in Regex.Matches(fileContents, @"[^:](:[a-zA-Z0-9_]+)"))
                {
                    string argumentName = match.Value.Substring(2);

                    if (!addedParamters.Contains(argumentName.ToLower()))
                    {
                        addedParamters.Add(argumentName.ToLower());

                        string propertyName = $"{char.ToUpperInvariant(argumentName[0])}{argumentName[1..].ToLowerInvariant()}";
                        if (string.Equals(argumentName, "rundate", StringComparison.OrdinalIgnoreCase))
                        {
                            propertyName = "RunDate";
                        }

                        if (propertyName == "RunDate")
                        {
                            argumentProperties += string.Format(@"        public required DateOnly {0} {{ get; set; }}
", propertyName);
                        }
                        else
                        {
                            argumentProperties += string.Format(@"        public required string {0} {{ get; set; }}
", propertyName);
                        }

                        if (propertyName == "RunDate")
                        {
                            argumentPropertySetups += string.Format(@"

            DbParameter param{1} = command.CreateParameter();
            param{1}.ParameterName = ""{0}"";
            param{1}.DbType = DbType.Date;
            param{1}.Value = new DateTime({1}.Year, {1}.Month, {1}.Day);
            command.Parameters.Add(param{1});", argumentName, propertyName);
                        }
                        else
                        {
                            argumentPropertySetups += string.Format(@"

            DbParameter param{1} = command.CreateParameter();
            param{1}.ParameterName = ""{0}"";
            param{1}.DbType = DbType.String;
            param{1}.Value = {1};
            command.Parameters.Add(param{1});", argumentName, propertyName);
                        }
                    }
                }

                string dataFileName = $"{dataFileClassName}.cs";
                string dataFileTemplate = string.Format(@"namespace Library.SnowflakeInterface.Data
{{
    public class {0}
    {{
    }}
}}
", dataFileClassName);

                string queryFileName = $"{dataFileClassName}Query.cs";
                string queryFileTemplate = string.Format(@"using Library.SnowflakeInterface.Data;
using System.Data;
using System.Data.Common;

namespace Library.SnowflakeInterface.QueryConfigurations
{{
    public class {0}Query : AbstractQueryConfiguration<{0}>
    {{
{1}
        /// <inheritdoc />
        public override string QueryFilePath => ""./SnowflakeSQL/{3}"";

        public override void AddParameters(DbCommand command, Action<string> logDebug)
        {{
            logDebug(""Adding parameters"");{2}
        }}

        /// <inheritdoc />
        public override {0} MapFromDataReaderToType(DbDataReader reader, Action<string> logDebug)
        {{
            logDebug(""Creating {0}"");
            throw new NotImplementedException();
        }}
    }}
}}
", dataFileClassName, argumentProperties, argumentPropertySetups, originalQueryFileName);

                using (StreamWriter writer = File.CreateText($"../../../../Library.SnowflakeInterface/Data/{dataFileName}"))
                {
                    writer.Write(dataFileTemplate);
                }

                using (StreamWriter writer = File.CreateText($"../../../../Library.SnowflakeInterface/QueryConfigurations/{queryFileName}"))
                {
                    writer.Write(queryFileTemplate);
                }
            }
        }
    }
}