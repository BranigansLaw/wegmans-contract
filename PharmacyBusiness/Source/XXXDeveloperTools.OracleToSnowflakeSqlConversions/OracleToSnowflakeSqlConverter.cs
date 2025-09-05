using System.Linq;
using System.Text;
using XXXDeveloperTools.OracleToSnowflakeSqlConversions.ConversionDataModel;
using XXXDeveloperTools.OracleToSnowflakeSqlConversions.Mapper;

namespace XXXDeveloperTools.OracleToSnowflakeSqlConversions
{
    public class OracleToSnowflakeSqlConverter
    {
        private bool LimitScopeToDW;
        public JobToBeConvertedFromOracleToSnowflake[] JobsToConvert { get; set; }
        public List<JobToBeConvertedFromOracleToSnowflake> ConvertedJobs { get; set; }
        private MappingEngine _mappingEngine { get; set; }

        public OracleToSnowflakeSqlConverter(bool limitScopeToDW)
        {
            LimitScopeToDW = limitScopeToDW;
            ConvertedJobs = new List<JobToBeConvertedFromOracleToSnowflake>();
            JobsToConvert = GetJobsToConvert();
            _mappingEngine = new MappingEngine();
            Console.WriteLine($"JobNumber,SqlFileName,SchemaName,TableName,ColumName,ColumnExistsInSnowflake");
        }

        public void ProcessSqlFiles(string oracleSqlRootDirectory, string snowflakeSqlDirectory)
        {
            List<string> oracleSqlFileNamesAndPaths = Directory.GetFiles(oracleSqlRootDirectory, "*.sql", SearchOption.AllDirectories).ToList();
            oracleSqlFileNamesAndPaths = oracleSqlFileNamesAndPaths.Where(f => f.IndexOf("bin") == -1).ToList();

            foreach (var oracleSqlFileNameAndPath in oracleSqlFileNamesAndPaths)
            {
                string oracleSqlFileName = Path.GetFileName(oracleSqlFileNameAndPath);
                var jobs = JobsToConvert.Where(j => oracleSqlFileNameAndPath.IndexOf($"{j.SqlFileRelativePath}{j.SqlFileName}", StringComparison.CurrentCultureIgnoreCase) > -1);

                if (jobs.Count() == 1)
                {
                    string snowflakeSqlFileName = Path.GetFileName(oracleSqlFileNameAndPath);
                    string snowflakeSqlFilePath = Path.Combine(snowflakeSqlDirectory, snowflakeSqlFileName);
                    string oracleSql = File.ReadAllText(oracleSqlFileNameAndPath);                   
                    string snowflakeSql = Convert(oracleSql);                    

                    if (LimitScopeToDW && snowflakeSql.IndexOf("UAT_MRXTS_DL_US_INT_ERXAUD_DATAINSIGHTS.ERXAUD_PLS_ARCHIVE_VIEW") == -1)
                        File.WriteAllText(snowflakeSqlFilePath, snowflakeSql);
                    else if(!LimitScopeToDW)
                        File.WriteAllText(snowflakeSqlFilePath, snowflakeSql);

                    var job = _mappingEngine.GetColumnMaps(oracleSql, jobs.First());
                    ConvertedJobs.Add(job);
                    //Console.WriteLine($"Job [{jobs?.First()?.JobNumber}] Oracle SQL has [{jobs?.First()?.ColumnMaps?.Where(m => m.IsOracleColumNameDeprecatedInSnowflake == true).Count()}] deprecated fields in file: [{oracleSqlFileNameAndPath}].");
                }
            }

            //TODO: Delete this line after we are done with the initial conversion of just the DW-only SQL files.
            var x = ConvertedJobs.GroupBy(g => g.JobNumber).Count();
            var y = ConvertedJobs.Where(w => w.ColumnMaps is not null && !w.ColumnMaps.Any()).Select(s => s.JobNumber).ToList();
        }

        /// <summary>
        /// This tool is not perfect....you will have to do a lot yourself to verify and make corrections.
        /// Hopefully, this saved you some time.....that's all.
        /// </summary>
        /// <param name="oracleSql"></param>
        /// <returns></returns>
        public string Convert(string oracleSql)
        {
            string snowflakeSql = oracleSql.Trim().ToUpper();

            snowflakeSql = snowflakeSql.Replace("FROM DUAL", "");
            snowflakeSql = snowflakeSql.Replace("INTERVAL '23:59:59' HOUR TO SECOND", "INTERVAL '23 HOURS, 59 MINUTES, 59 SECONDS'");
            snowflakeSql = snowflakeSql.Replace("INTERVAL '15:00:00' HOUR TO SECOND", "INTERVAL '15 hours'");
            snowflakeSql = snowflakeSql.Replace("SYSDATE", "SYSDATE()");
            snowflakeSql = snowflakeSql.Replace("SELECT UNIQUE", "SELECT DISTINCT");
            
            // Special Cases
            snowflakeSql = snowflakeSql.Replace("TO_CHAR(D.DRUG_NDC,'00000G0000G00','NLS_NUMERIC_CHARACTERS=.-')", "LPAD(SUBSTR(DRUG_NDC::TEXT, 1, 5), 5, '0') " +
                                                "|| '-' ||\r\n\t\t\tLPAD(SUBSTR(DRUG_NDC::TEXT, 6, 4), 4, '0') || '-' ||\r\n\t\t\tLPAD(SUBSTR(DRUG_NDC::TEXT, 10, 2), 2, '0'");
            snowflakeSql = snowflakeSql.Replace("CAST(FROM_TZ(CAST(AUD__RX_FILL.DATESTAMP AS TIMESTAMP), 'UTC') AT TIME ZONE 'US/EASTERN' AS DATE)", "CONVERT_TIMEZONE('UTC', 'US/Eastern', CAST(AUD__RX_FILL.DATESTAMP AS TIMESTAMP))");
            snowflakeSql = snowflakeSql.Replace("LISTAGG(DISTINCT TO_CHAR(SEA.SAE_START_TIME::DATE,'HH24:MI:SS'),',')", "LISTAGG(Distinct TO_CHAR(sea.SAE_START_TIME, 'HH24:MI:SS'), ',')");
            snowflakeSql = snowflakeSql.Replace("WITHIN GROUP (ORDER BY SEA.SAE_START_TIME) AS LOGIN_TIMES_CSV", "WITHIN GROUP (ORDER BY TO_CHAR(sea.SAE_START_TIME, 'HH24:MI:SS')) AS LOGIN_TIMES_CSV");
            
            snowflakeSql = snowflakeSql.Replace("TREXONE_DW_DATA", "UAT_MRXTS_DL_US_INT_ERXDW_DATAINSIGHTS.ERXDW_PLS_ARCHIVE_VIEW");
            snowflakeSql = snowflakeSql.Replace("TREXONE_AUD_DATA", "UAT_MRXTS_DL_US_INT_ERXAUD_DATAINSIGHTS.ERXAUD_PLS_ARCHIVE_VIEW");

            snowflakeSql = RepairRowId(snowflakeSql);
            snowflakeSql = RepairRowNum(snowflakeSql);
            snowflakeSql = RemoveOracleHints(snowflakeSql);
            snowflakeSql = ModifyNestedOracleTruncDates(snowflakeSql);
            snowflakeSql = ModifyNestedOracleTocharDates(snowflakeSql);

            return snowflakeSql;
        }

        /// <summary>
        /// When TO_CHAR date values, Snowflake needs a second argument to specify the variable is a date type.
        /// </summary>
        /// <param name="oracleSql"></param>
        /// <returns></returns>
        public string ModifyNestedOracleTocharDates(string oracleSql)
        {
            oracleSql = oracleSql.Trim().ToUpper();
            var snowflakeSql = new StringBuilder();
            var stack = new Stack<int>();
            var startIndices = new List<int>();

            for (int i = 0; i < oracleSql.Length; i++)
            {
                char c = oracleSql[i];
                if (c == '(')
                {
                    stack.Push(i);
                }
                else if (c == ')')
                {
                    if (stack.Count > 0)
                    {
                        int startIndex = stack.Pop();
                        if (startIndex > 6 && oracleSql.Substring(startIndex - 7, 7) == "TO_CHAR")
                        {
                            startIndices.Add(startIndex - 7);
                        }
                    }
                }
            }

            int lastIndex = 0;
            foreach (int i in startIndices)
            {
                int openIdx = i + 7;
                int closeIdx = -1;
                int openCount = 1;

                for (int j = openIdx + 1; j < oracleSql.Length; j++)
                {
                    if (oracleSql[j] == '(')
                        openCount++;
                    if (oracleSql[j] == ')')
                        openCount--;
                    if (openCount == 0)
                    {
                        closeIdx = j;
                        break;
                    }
                }

                if (closeIdx == -1)
                    continue;

                if (i > lastIndex)
                    snowflakeSql.Append(oracleSql.Substring(lastIndex, i - lastIndex));
                else
                    break;
               

                string insideTochar = oracleSql.Substring(openIdx + 1, closeIdx - openIdx - 1);

                if (insideTochar.IndexOf(",'YYYY") > -1)
                    insideTochar = insideTochar.Replace(",'YYYY", "::DATE,'YYYY");

                if (insideTochar.IndexOf(", 'YYYY") > -1)
                    insideTochar = insideTochar.Replace(", 'YYYY", "::DATE, 'YYYY");

                if (insideTochar.IndexOf(",'HH") > -1)
                    insideTochar = insideTochar.Replace(",'HH", "::DATE,'HH");

                if (insideTochar.IndexOf(", 'HH") > -1)
                    insideTochar = insideTochar.Replace(", 'HH", "::DATE, 'HH");

                if (insideTochar.IndexOf(",'DY") > -1)
                    insideTochar = insideTochar.Replace(",'DY", "::DATE,'DY");

                if (insideTochar.IndexOf(", 'DY") > -1)
                    insideTochar = insideTochar.Replace(", 'DY", "::DATE, 'DY");

                if (insideTochar.IndexOf(",'MM") > -1)
                    insideTochar = insideTochar.Replace(",'MM", "::DATE,'MM");

                if (insideTochar.IndexOf(", 'MM") > -1)
                    insideTochar = insideTochar.Replace(", 'MM", "::DATE, 'MM");

                snowflakeSql.Append($"TO_CHAR({insideTochar})");
                lastIndex = closeIdx + 1;
            }

            snowflakeSql.Append(oracleSql.Substring(lastIndex));
            return snowflakeSql.ToString();
        }

        /// <summary>
        /// When truncating date values, Snowflake needs a second argument to specify the date part to truncate to.
        /// </summary>
        /// <param name="oracleSql"></param>
        /// <returns></returns>
        public string ModifyNestedOracleTruncDates(string oracleSql)
        {
            oracleSql = oracleSql.Trim().ToUpper();
            var snowflakeSql = new StringBuilder();
            var stack = new Stack<int>();
            var startIndices = new List<int>();

            for (int i = 0; i < oracleSql.Length; i++)
            {
                char c = oracleSql[i];
                if (c == '(')
                {
                    stack.Push(i);
                }
                else if (c == ')')
                {
                    if (stack.Count > 0)
                    {
                        int startIndex = stack.Pop();
                        if (startIndex > 4 && oracleSql.Substring(startIndex - 5, 5) == "TRUNC")
                        {
                            startIndices.Add(startIndex - 5);
                        }
                    }
                }
            }

            int lastIndex = 0;
            foreach (int i in startIndices)
            {
                int openIdx = i + 5;
                int closeIdx = -1;
                int openCount = 1;

                for (int j = openIdx + 1; j < oracleSql.Length; j++)
                {
                    if (oracleSql[j] == '(')
                        openCount++;
                    if (oracleSql[j] == ')')
                        openCount--;
                    if (openCount == 0)
                    {
                        closeIdx = j;
                        break;
                    }
                }

                if (closeIdx == -1)
                    continue;

                snowflakeSql.Append(oracleSql.Substring(lastIndex, i - lastIndex));
                string insideTrunc = oracleSql.Substring(openIdx + 1, closeIdx - openIdx - 1);

                // There is only one instance of Oracle SQL applying TRUNC to a decimal value in "RX_READY_YYYYMMDD.sql".
                if (insideTrunc == "FILLFACT.TOTAL_REFILLS_REMAINING")
                    snowflakeSql.Append($"TRUNC({insideTrunc})");
                else
                    snowflakeSql.Append($"TRUNC({insideTrunc}, 'day')");

                lastIndex = closeIdx + 1;
            }

            snowflakeSql.Append(oracleSql.Substring(lastIndex));
            return snowflakeSql.ToString();
        }

        /// <summary>
        /// This version does not handle nested TRUNC functions.
        /// </summary>
        /// <param name="oracleSql"></param>
        /// <returns></returns>
        public string ModifySimpleOracleTruncDates(string oracleSql)
        {
            string snowflakeSql = oracleSql;
            string findMe = "TRUNC(";
            int startIndexOf_Trunc;
            int endOfLastFieldIndex = 0;

            do
            {
                startIndexOf_Trunc = snowflakeSql.IndexOf(findMe, endOfLastFieldIndex);
                if (startIndexOf_Trunc == -1)
                {
                    break;
                }
                startIndexOf_Trunc += findMe.Length;
                int endIndexOf_Trunc = snowflakeSql.IndexOf(")", startIndexOf_Trunc);
                string tempField = snowflakeSql.Substring(startIndexOf_Trunc, endIndexOf_Trunc - startIndexOf_Trunc);

                // There is only one instance of Oracle SQL applying TRUNC to a decimal value in "RX_READY_YYYYMMDD.sql".
                if (tempField.Contains("TOTAL_REFILLS_REMAINING"))
                {
                    continue;
                }

                snowflakeSql = snowflakeSql.Replace($"TRUNC({tempField})", $"TRUNC({tempField}, 'DAY')");
                endOfLastFieldIndex = endIndexOf_Trunc;
            } while (snowflakeSql.IndexOf(findMe, startIndexOf_Trunc) > -1);

            return snowflakeSql;
        }

        public string RemoveOracleHints(string oracleSql)
        {
            string snowflakeSql = oracleSql;
            string findMe = "/*";
            string findMe2 = "*/";

            int endOfLastHintIndex = 0;
            do
            {
                int startIndexOf_Hint = snowflakeSql.IndexOf(findMe, endOfLastHintIndex);
                if (startIndexOf_Hint == -1)
                {
                    break;
                }
                int endIndexOf_Hint = snowflakeSql.IndexOf(findMe2, startIndexOf_Hint) + findMe2.Length;
                string tempFirstHalfOfSql = snowflakeSql.Substring(0, startIndexOf_Hint);

                int secondHalfStartIndex = endIndexOf_Hint;
                int secondHalfLength = snowflakeSql.Length - endIndexOf_Hint;
                string tempSecondHalfOfSql = snowflakeSql.Substring(secondHalfStartIndex, secondHalfLength);
                snowflakeSql = tempFirstHalfOfSql + tempSecondHalfOfSql;
            } while (snowflakeSql.Contains(findMe));

            return snowflakeSql;
        }

        /// <summary>
        /// Replace rownum with a snowflake version. You may need to add "as rownum" to run a query in snowflake
        /// </summary>
        /// <param name="oracleSql"></param>
        /// <returns></returns>
        public string RepairRowNum(string oracleSql)
        {

            var snowflakeSql = new StringBuilder(oracleSql);
            if (oracleSql.Contains("ROWNUM"))
            {
                int currentIndex = 0;

                int startOfOrder;
                int rownumIdx;
                int startOfSelect;
                int startOfFrom;

                string str;
                string orderBy;
                string OrderByTxt = "ORDER BY";
                string SelectTxt = "SELECT";

                while ((currentIndex = oracleSql.IndexOf("ROWNUM", currentIndex)) != -1)
                {
                    int lineStart = oracleSql.LastIndexOf('\n', currentIndex);
                    int commentStart = oracleSql.IndexOf("--", lineStart == -1 ? 0 : lineStart);

                    if (commentStart != -1 && commentStart < currentIndex)
                    {
                        currentIndex = oracleSql.IndexOf('\n', currentIndex);
                        if (currentIndex == -1) break;
                        continue;
                    }

                    startOfOrder = oracleSql.LastIndexOf(OrderByTxt, currentIndex);
                    if (startOfOrder != -1 && startOfOrder < currentIndex)
                    {
                        rownumIdx = oracleSql.IndexOf("ROWNUM", startOfOrder);
                        orderBy = oracleSql.Substring(startOfOrder + OrderByTxt.Length, rownumIdx - startOfOrder - OrderByTxt.Length).Trim();
                    }
                    else
                    {
                        startOfSelect = oracleSql.IndexOf(SelectTxt);
                        startOfFrom = oracleSql.IndexOf("FROM", startOfSelect);
                        orderBy = oracleSql.Substring(startOfSelect + SelectTxt.Length, startOfFrom - startOfSelect - SelectTxt.Length).Trim();
                    }
                    string[] parts = orderBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    str = parts.First(p => p.Contains("_"));
                    str = orderBy.Split(" ")[0].Split(new[] { "(", ")", "*", "/*", "+", "," }, StringSplitOptions.RemoveEmptyEntries).Where(p => !string.IsNullOrEmpty(p)).First();

                    string replacement = $"ROW_NUMBER() OVER (ORDER BY {str})";
                    snowflakeSql = snowflakeSql.Replace("ROWNUM", replacement);

                    currentIndex += 6;
                }

            }
            return snowflakeSql.ToString();
        }

        /// <summary>
        /// Replaces rowid with a unique number assigned to a column
        /// </summary>
        /// <param name="oracleSql"></param>
        /// <returns></returns>
        public string RepairRowId(string oracleSql)
        {
            var snowflakeSql = new StringBuilder(oracleSql);
            if (oracleSql.Contains("ROWID"))
            {
                int currentIndex = 0;
                int startOfOrder;
                int rowidIdx;
                int startOfSelect;
                int startOfFrom;

                string str;
                string orderBy;
                string alias = "";
                string OrderByTxt = "ORDER BY";
                string SelectTxt = "SELECT";

                while ((currentIndex = oracleSql.IndexOf("ROWID", currentIndex)) != -1)
                {
                    startOfOrder = oracleSql.LastIndexOf(OrderByTxt, currentIndex);
                    if (startOfOrder != -1 && startOfOrder < currentIndex)
                    {
                        rowidIdx = oracleSql.IndexOf("ROWID", startOfOrder);

                        orderBy = oracleSql.Substring(startOfOrder + OrderByTxt.Length, rowidIdx - startOfOrder - OrderByTxt.Length).Trim();
                    }
                    else
                    {
                        startOfSelect = oracleSql.IndexOf(SelectTxt);
                        startOfFrom = oracleSql.IndexOf("FROM", startOfSelect);

                        orderBy = oracleSql.Substring(startOfOrder + SelectTxt.Length, startOfFrom - startOfSelect - SelectTxt.Length).Trim();
                    }
                    string[] parts = orderBy.Split(' ');
                    str = parts.First(p => p.Contains("."));
                    str = str.Split(' ')[0].Split(['(', ')', ',']).Where(p => !string.IsNullOrEmpty(p)).First();
                    alias = str.Substring(0, str.IndexOf("."));

                    string replacement = $"ROW_NUMBER() OVER (ORDER BY {str})";
                    snowflakeSql = snowflakeSql.Replace($"{alias}.ROWID", replacement);
                    currentIndex += 5;
                }
            }
            return snowflakeSql.ToString();
        }

        public JobToBeConvertedFromOracleToSnowflake[] GetJobsToConvert()
        {
            //NOTES:
            // This is a static list of Oracle SQL files that we want to convert to Snowflake SQL.
            // NOT ALL SQL files are in folders you might expect them to be in, so this list is necessary.
            // These last three SQL files are the only Specialty SQL files that need to be converted.
            JobToBeConvertedFromOracleToSnowflake[] jobsToConvert = [
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="INN701", SqlFileName="SelectTurnaroundTime.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\Library.McKessonDWInterface\\SQL\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="INN528", SqlFileName="SelectThirdPartyClaimsBase.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\Library.McKessonDWInterface\\SQL\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="INN528", SqlFileName="SelectThirdPartyClaimsLookupAcquisitionCost.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\Library.McKessonDWInterface\\SQL\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="INN603", SqlFileName="SelectSureScriptsMedicalHistory.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\Library.McKessonDWInterface\\SQL\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="INN603", SqlFileName="SelectSureScriptsPhysicianNotificationLetters.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\Library.McKessonDWInterface\\SQL\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="INN529", SqlFileName="SelectWorkloadBalance.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\Library.McKessonDWInterface\\SQL\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="INN510", SqlFileName="SelectNewTagPatientGroups.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\Library.McKessonDWInterface\\SQL\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="INN511", SqlFileName="SelectRxErp.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\Library.McKessonDWInterface\\SQL\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="INN512", SqlFileName="SelectSoldDetail.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\Library.McKessonDWInterface\\SQL\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="INN520", SqlFileName="SelectStoreInventoryHistory.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\Library.McKessonDWInterface\\SQL\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX515", SqlFileName="Omnisys_Claim_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX515\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX540", SqlFileName="MightBeSoldTransactions.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX540\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX540", SqlFileName="MightBeRefundTransactions.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX540\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX572", SqlFileName="WEG_086_YYYYMMDD_01_For198.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX572\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX572", SqlFileName="WEG_086_YYYYMMDD_01.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX572\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX576", SqlFileName="DurConflict_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX576\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX576", SqlFileName="EXTENDED_DRUG_FILE_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX576\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX576", SqlFileName="InvAdj_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX576\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX576", SqlFileName="PetPtNums_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX576\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX576", SqlFileName="PRESCRIBER_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX576\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX576", SqlFileName="PRESCRIBERADDRESS_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX576\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX576", SqlFileName="RxTransfer_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX576\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX576", SqlFileName="SupplierPriceDrugFile_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX576\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX576", SqlFileName="TagPatientGroups_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX576\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX578", SqlFileName="Wegmans_HPOne_Pharmacies_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX578\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX578", SqlFileName="Wegmans_HPOne_Prescriptions_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX578\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX578", SqlFileName="FDS_Prescriptions.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX578\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX578", SqlFileName="FDS_Pharmacies.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX578\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX582", SqlFileName="GetFillFacts.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX582\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX582", SqlFileName="GettHipaaClaimElements.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX582\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX801", SqlFileName="WorkersCompMonthly_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX801\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS506", SqlFileName="ETLInfoForLogging_Oracle_DW.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS506\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS506", SqlFileName="ETLCheck_Oracle_DW.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS506\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS512", SqlFileName="DailyDrug_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS512\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS527", SqlFileName="Get_IVR_Outbound_Calls.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS527\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS578", SqlFileName="TEMP_WORKFLOW_STEP.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS578\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS579", SqlFileName="Super_Workflow_Steps_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS578\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS580", SqlFileName="AtebMaster_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS580\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS582", SqlFileName="McKesson_Oracle_DW.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS582\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS605", SqlFileName="Alternative_Payments_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS605\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS608", SqlFileName="CC_Payments_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS608\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS609", SqlFileName="Sold_Detail_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS609\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS610", SqlFileName="Super_Duper_Claim_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS610\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="CRX576", SqlFileName="RX_READY_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\CRX576\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS611", SqlFileName="GetSmartOrderPointsMinMax.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS611\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS613", SqlFileName="Patients_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS613\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS614", SqlFileName="Patient_Addresses_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS614\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS617", SqlFileName="VeriFone_Payments_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS617\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS660", SqlFileName="GetPOAudit_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS660\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS661", SqlFileName="GetPOFact_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS661\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS720", SqlFileName="Deceased_YYYYMMDD.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS720\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS777", SqlFileName="SelectERxUnauthorized.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS777\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS812", SqlFileName="McKesson_DW_Naloxone.sql", SqlFileRelativePath="PharmacyBusiness\\Source\\RX.PharmacyBusiness.ETL\\RXS812\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS622", SqlFileName="GetSpecialtyDispenses.sql", SqlFileRelativePath="Specialty\\source\\RX.Specialty.ETL\\RXS622\\SQL\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS652", SqlFileName="GetEnterpriseRxDataByPatientNum.sql", SqlFileRelativePath="Specialty\\source\\RX.Specialty.ETL\\RXS652\\SQL\\" },
                new JobToBeConvertedFromOracleToSnowflake { JobNumber="RXS642", SqlFileName="GetClaims.sql", SqlFileRelativePath="Specialty\\source\\RX.Specialty.ETL\\RXS642\\SQL\\" }
            ];

            return jobsToConvert;
        }
    }
}
