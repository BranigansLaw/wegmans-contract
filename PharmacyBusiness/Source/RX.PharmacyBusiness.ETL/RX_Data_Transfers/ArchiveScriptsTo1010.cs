namespace RX.PharmacyBusiness.ETL.RX_Data_Transfers
{
    using RX.PharmacyBusiness.ETL.RX_Data_Transfers.Business;
    using RX.PharmacyBusiness.ETL.RX_Data_Transfers.Core;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("Job-TBD", "Archive (upload) old Scripts data to new 1010data via api call.", "KBA-TBD", "")]
    public class ArchiveScriptsTo1010 : ETLBase
    {
        private string apiLoginSID;
        private string apiLoginPSWD;
        private string apiLoginVERSION;
        private ReturnCode returnCode;

        protected override void Execute(out object result)
        {
            this.returnCode = new ReturnCode();
            DateTime runDate = DateTime.Now;
            string apiRC = string.Empty;
            string filterSchemaOwner = (this.Arguments["-FilterSchemaOwner"] == null) ? string.Empty : this.Arguments["-FilterSchemaOwner"].ToString();
            string filterTableName = (this.Arguments["-FilterTableName"] == null) ? string.Empty : this.Arguments["-FilterTableName"].ToString();
            //bool outputDataXmlFile = (this.Arguments["-OutputDataXmlFile"].ToString() == "Y");
            //bool performCleanString = (this.Arguments["-PerformCleanString"].ToString() == "Y");
            //bool usePowerLoader = (this.Arguments["-UsePowerLoader_YN"].ToString() == "Y");
            this.apiLoginSID = string.Empty;
            this.apiLoginPSWD = string.Empty;
            this.apiLoginVERSION = string.Empty;
            OracleHelper oracleHelper = new OracleHelper();
            FileHelper fileHelper = new FileHelper(
                    @"%BATCH_ROOT%\RX_Data_Transfers\Input\",
                    @"%BATCH_ROOT%\RX_Data_Transfers\Output\",
                    @"%BATCH_ROOT%\RX_Data_Transfers\Archive\",
                    @"%BATCH_ROOT%\RX_Data_Transfers\Reject\");
            string basePath1010 = ConfigurationManager.AppSettings["Archive_Scripts_BasePath1010"].ToString();
            StringBuilder xmlDataFor1010 = new StringBuilder();
            StringBuilder oracleTableAllDataSql = new StringBuilder();

            Log.LogInfo("Begin ArchiveScriptsTo1010...");

            if (string.IsNullOrEmpty(filterSchemaOwner) || string.IsNullOrEmpty(filterTableName))
                throw new Exception("Filters for Schema and Tables are required.");

            Log.LogInfo("Get Oracle Schema filtered on OWNER like 'RX%' (to ensure no system tables) and OWNER like [{0}] and TABLE_NAME like [{1}].",
                filterSchemaOwner,
                filterTableName);

            List<OracleSchema> oracleSchema = oracleHelper.DownloadQueryToList<OracleSchema>(
                150,
                @"%BATCH_ROOT%\RX_Data_Transfers\bin\SelectOracleSchema.sql",
                new { LikeSchemaOwner = filterSchemaOwner, LikeTableName = filterTableName },
                "SCRIPTS_BATCH"
                );
            fileHelper.WriteListToFile<OracleSchema>(
                oracleSchema,
                @"%BATCH_ROOT%\RX_Data_Transfers\Archive\ScriptsDatabaseSchemaToArchive.csv",
                true,
                ",",
                "\"",
                true,
                false
                );

            //Group schema by Table Names.
            var filteredTableNames = oracleSchema
                .GroupBy(g => new
                {
                    g.table_owner,
                    g.table_name
                })
                .Select(s => new
                {
                    TableOwner = s.First().table_owner,
                    TableName = s.First().table_name
                })
                .OrderBy(o => o.TableOwner)
                .ThenBy(o => o.TableName)
                .ToList();

            Log.LogInfo("There are [{0}] Scripts tables to process.", filteredTableNames.Count);
            int tablesProcessed = 0;

            foreach (var filteredTableName in filteredTableNames)
            {
                tablesProcessed++;

                /*
                Log.LogInfo("Generate XML for 1010 table definition and its data to use with upload-API.");

                xmlDataFor1010 = OracleTo1010Converter.Get1010TableDefinition_ForUploadApi(oracleSchema
                    .Where(r => r.table_owner == filteredTableName.TableOwner && r.table_name == filteredTableName.TableName)
                    .ToList());
                List<MultipurposeString> tableDefinition1010 = new List<MultipurposeString>();
                tableDefinition1010.Add(new MultipurposeString(xmlDataFor1010.ToString()));

                oracleTableAllDataSql = OracleTo1010Converter.GetOracleSqlWithFormatAllColumnsAsStrings_ForUploadApi(oracleSchema
                    .Where(r => r.table_owner == filteredTableName.TableOwner && r.table_name == filteredTableName.TableName)
                    .ToList());
                List<MultipurposeString> oracleSqlAllDataList = new List<MultipurposeString>();
                oracleSqlAllDataList.Add(new MultipurposeString(oracleTableAllDataSql.ToString()));
                
                fileHelper.WriteListToFile<MultipurposeString>(
                    tableDefinition1010,
                    @"%BATCH_ROOT%\RX_Data_Transfers\Archive\" + filteredTableName.TableOwner + "_" + filteredTableName.TableName + "_1010_table_definition.xml",
                    false,
                    string.Empty,
                    string.Empty,
                    true,
                    false
                    );                    

                fileHelper.WriteListToFile<MultipurposeString>(
                    oracleSqlAllDataList,
                    @"%BATCH_ROOT%\RX_Data_Transfers\Archive\" + filteredTableName.TableOwner + "_" + filteredTableName.TableName + "_get_all_data.sql",
                    false,
                    string.Empty,
                    string.Empty,
                    true,
                    false
                    );
                */

                Log.LogInfo("--------Output SQL and XML files for data migration of Scripts table [{0}.{1}], which id [{2}] of [{3}].--------", filteredTableName.TableOwner, filteredTableName.TableName, tablesProcessed, filteredTableNames.Count);

                fileHelper.WriteListToFile<MultipurposeString>(
                    OracleTo1010Converter.Get1010TableDefinition_ForOdbcTenUp(oracleSchema
                        .Where(r => r.table_owner == filteredTableName.TableOwner && r.table_name == filteredTableName.TableName)
                        .ToList(), basePath1010),
                    @"%BATCH_ROOT%\RX_Data_Transfers\Output\" + filteredTableName.TableOwner + "_" + filteredTableName.TableName + "_tabletree.xml",
                    false,
                    string.Empty,
                    string.Empty,
                    true,
                    false
                    );

                fileHelper.WriteListToFile<MultipurposeString>(
                    OracleTo1010Converter.GetOracleSqlWithFormatAllColumns_ForOdbcTenUp(oracleSchema
                        .Where(r => r.table_owner == filteredTableName.TableOwner && r.table_name == filteredTableName.TableName)
                        .ToList()),
                    @"%BATCH_ROOT%\RX_Data_Transfers\Output\" + filteredTableName.TableOwner + "_" + filteredTableName.TableName + "-with_datatype_conversions.sql",
                    false,
                    string.Empty,
                    string.Empty,
                    true,
                    false
                    );

                fileHelper.WriteListToFile<MultipurposeString>(
                    OracleTo1010Converter.GetOracleSqlForRowCount(oracleSchema
                        .Where(r => r.table_owner == filteredTableName.TableOwner && r.table_name == filteredTableName.TableName)
                        .ToList()),
                    @"%BATCH_ROOT%\RX_Data_Transfers\Archive\" + filteredTableName.TableOwner + "_" + filteredTableName.TableName + "_get_row_count.sql",
                    false,
                    string.Empty,
                    string.Empty,
                    true,
                    false
                    );

                Log.LogInfo("Executing row count SQL...");
                List<OracleSchemaRowCount> oracleSchemaRowCount = oracleHelper.DownloadQueryByRunDateToList<OracleSchemaRowCount>(
                    150,
                    @"%BATCH_ROOT%\RX_Data_Transfers\Archive\" + filteredTableName.TableOwner + "_" + filteredTableName.TableName + "_get_row_count.sql",
                    runDate,
                    "SCRIPTS_BATCH"
                    );
                Log.LogInfo("Row Count is [{0}].", oracleSchemaRowCount.First().ROW_COUNT);
                fileHelper.WriteListToFile<OracleSchemaRowCount>(
                    oracleSchemaRowCount,
                    @"%BATCH_ROOT%\RX_Data_Transfers\Archive\ScriptsDatabase_table_row_counts.csv",
                    (tablesProcessed == 1),
                    ",",
                    string.Empty,
                    true,
                    true
                    );

                if (oracleSchema
                        .Where(r => r.table_owner == filteredTableName.TableOwner && 
                                    r.table_name == filteredTableName.TableName &&
                                    r.column_data_type == "BLOB")
                        .Count() > 0)
                {

                    fileHelper.WriteListToFile<MultipurposeString>(
                        OracleTo1010Converter.GetOracleSqlToDownloadImages(oracleSchema
                            .Where(r => r.table_owner == filteredTableName.TableOwner && r.table_name == filteredTableName.TableName)
                            .ToList()),
                        @"%BATCH_ROOT%\RX_Data_Transfers\Archive\" + filteredTableName.TableOwner + "_" + filteredTableName.TableName + "_download_images.sql",
                        false,
                        string.Empty,
                        string.Empty,
                        true,
                        false
                        );
                    
                    long fileCount = oracleHelper.DownloadImagesFromBlobsToFiles(
                        150,
                        @"%BATCH_ROOT%\RX_Data_Transfers\Archive\" + filteredTableName.TableOwner + "_" + filteredTableName.TableName + "_download_images.sql",
                        @"%BATCH_ROOT%\RX_Data_Transfers\Output",
                        "SCRIPTS_BATCH"
                        );

                    Log.LogInfo("Table [{0}.{1}] has [{2}] files.", filteredTableName.TableOwner, filteredTableName.TableName, fileCount);
                }


                /*
                Log.LogInfo("Executing all data SQL...");

                List<string> oraclePipeDelimitedRecords = oracleHelper.DownloadQueryByRunDateToList<string>(
                    150,
                    @"%BATCH_ROOT%\RX_Data_Transfers\Archive\" + filteredTableName.TableOwner + "_" + filteredTableName.TableName + "_get_all_data.sql",
                    runDate,
                    "SCRIPTS_BATCH"
                    );

                xmlDataFor1010.AppendLine(@"<data>");

                foreach (var oraclePipeDelimitedRecord in oraclePipeDelimitedRecords)
                {
                    string[] columnData = oraclePipeDelimitedRecord.Split('|');

                    xmlDataFor1010.AppendLine(@"<tr>");

                    //Skip first column which is column count.
                    for (int i = 1; i < columnData.Length; i++)
                    {
                        xmlDataFor1010.Append(@"<td>");
                        
                        if (performCleanString && !decimal.TryParse(columnData[i], out decimal temp))
                        {
                            //String type.
                            xmlDataFor1010.Append(TenTenHelper.CleanStringForTenUp(columnData[i]));
                        }
                        else
                        {
                            //Any number type.
                            xmlDataFor1010.Append(columnData[i]);
                        }

                        xmlDataFor1010.AppendLine(@"</td>");
                    }

                    xmlDataFor1010.AppendLine(@"</tr>");
                }

                xmlDataFor1010.AppendLine(@"</data>");
                xmlDataFor1010.AppendLine(@"</table>");
                xmlDataFor1010.Append(@"<name mode=""replace"">wegmans.devpharm.chrism.demos.");
                xmlDataFor1010.Append(filteredTableName.TableOwner.ToLower());
                xmlDataFor1010.Append(".");
                xmlDataFor1010.Append(filteredTableName.TableName.ToLower());
                xmlDataFor1010.AppendLine(@"</name>");
                xmlDataFor1010.AppendLine(@"<users>");
                xmlDataFor1010.AppendLine(@"<user>wegmans_cjmccarthy</user>");
                xmlDataFor1010.AppendLine(@"<user>wegmans_rx_batch</user>");
                xmlDataFor1010.AppendLine(@"<user>wegmans_rx_batch2</user>");
                xmlDataFor1010.AppendLine(@"</users>");
                xmlDataFor1010.AppendLine(@"</in>");
                */

                /* The following is useful in developing, but not on scale up/go-live.
                List<MultipurposeString> tableWithDataFor1010Api = new List<MultipurposeString>();

                if (outputDataXmlFile)
                { 
                    tableWithDataFor1010Api.Add(new MultipurposeString(xmlDataFor1010.ToString()));

                    fileHelper.WriteListToFile<MultipurposeString>(
                        tableWithDataFor1010Api,
                        @"%BATCH_ROOT%\RX_Data_Transfers\Archive\" + filteredTableName.TableOwner + "_" + filteredTableName.TableName + "_1010_table_data.xml",
                        false,
                        string.Empty,
                        string.Empty,
                        true,
                        false
                        );
                }
                */

                /*
                Log.LogInfo("Execute api=login to get session id.");
                this.CallApi("login", string.Empty);

                if (this.returnCode.IsSuccess)
                {
                    Log.LogInfo("With a session id can now execute api=upload.");

                    //if (usePowerLoader)
                    //{
                    //    //https://docs.1010data.com/XMLAPI/PLTransactions/PLAPI-addtab.html
                    //    apiRC = this.CallApi("addtab", xmlDataFor1010.ToString().Replace("\n","").Trim());
                    //}

                    //https://docs.1010data.com/XMLAPI/TableManagement/TableManagement.html
                    this.CallApi("upload", xmlDataFor1010.ToString().Replace("\n","").Trim());
                }

                Log.LogInfo("Execute api=logout.");
                this.CallApi("logout", string.Empty);
                */
            }

            Log.LogInfo("Finished ArchiveScriptsTo1010.");
            result = this.returnCode.IsFailure ? (this.returnCode.HasWriteError ? 2 : 1) : 0;
        }

        private void CallApi(string apiName, string apiData)
        {
            string apiRC = string.Empty;
            string apiMSG = string.Empty;
            string accountUsername = "wegmans_rx_batch2";
            string aacountPassword = "#################";
            string apiUrl = "https://wegmans.1010data.com/cgi-bin/prod-stable/gw.k?protocol=xml-rpc&apiversion=3&kill=no&uid=" + accountUsername + "&api=" + apiName;

            if (apiName == "login")
                apiUrl += "&pswd=" + aacountPassword;
            else
                apiUrl += "&pswd=" + this.apiLoginPSWD + "&sid=" + this.apiLoginSID;

            HttpWebRequest apiRequest = (HttpWebRequest)WebRequest.Create(apiUrl);
            apiRequest.Method = "POST";
            apiRequest.ContentType = "application/xml";

            //The next line produces a bug with large amounts of data (one million rows):
            //apiRequest.ContentLength = apiData.Length;
            //See bugfixes suggested here, this one says do not set ContentLength: https://stackoverflow.com/questions/19025834/cannot-close-stream-until-all-bytes-are-written-gooddata-api

            //More bugfixes suggested here, this one says to use bytes: https://stackoverflow.com/questions/19025834/cannot-close-stream-until-all-bytes-are-written-gooddata-api
            //byte[] apiDataBytes = Encoding.UTF8.GetBytes(apiData);
            //apiRequest.ContentLength = apiDataBytes.Length;

            using (Stream apiWebStream = apiRequest.GetRequestStream())
            using (StreamWriter apiRequestWriter = new StreamWriter(apiWebStream, System.Text.Encoding.ASCII))
            {
                apiWebStream.WriteTimeout = 600000; //10 minutes, but this apparently has no effect on the "upload" API.
                apiRequestWriter.Write(apiData);
                //The next line is a continuation of the ContentLength bug potentially fixed by using bytes.
                //apiWebStream.Write(apiDataBytes, 0, apiDataBytes.Length);

            }

            try
            {
                WebResponse apiWebResponse = apiRequest.GetResponse();
                using (Stream apiWebStream = apiWebResponse.GetResponseStream() ?? Stream.Null)
                using (StreamReader apiResponseReader = new StreamReader(apiWebStream))
                {
                    string apiHtmlResponse = apiResponseReader.ReadToEnd();
                    /* SAMPLE XML RETURNED from login api:
                    <out>
                        <rc>0</rc>
                        <sid>(session id)</sid>
                        <pswd>(encrypted pw)</pswd>
                        <msg>Last login was: 2022-05-04 22:41:09</msg>
                        <version>prime-17.35</version>
                    </out>
                    */
                    //The following code was taken from https://stackoverflow.com/questions/34863330/read-xml-response-from-page
                    var xmlResponse = System.Xml.Linq.XElement.Parse(apiHtmlResponse);

                    apiRC = xmlResponse.Elements("rc").FirstOrDefault().Value;
                    apiMSG = xmlResponse.Elements("msg").FirstOrDefault().Value;
                    Log.LogInfo("API [{2}] call obtained RC=[{0}] and MSG=[{1}].", apiRC, apiMSG, apiName);

                    if (apiName == "login")
                    {
                        this.apiLoginSID = xmlResponse.Elements("sid").FirstOrDefault().Value;
                        this.apiLoginPSWD = xmlResponse.Elements("pswd").FirstOrDefault().Value;
                        this.apiLoginVERSION = xmlResponse.Elements("version").FirstOrDefault().Value;
                    }
                    //else
                    //{
                    //    Log.LogInfo("----START TenTenApi [{0}] api RESPONSE-------------", apiName);
                    //    Log.LogInfo(apiHtmlResponse);
                    //    Log.LogInfo("----END   TenTenApi [{0}] api RESPONSE-------------", apiName);
                    //}
                }
            }
            catch (Exception ex)
            {
                Log.LogWarn("Exception with [{0}] api call is [{1}].", apiName, ex.Message);
                this.returnCode.IsFailure = true;
            }

            if (apiRC == "5")
            {
                Log.LogWarn("Account already logged in, so need to wait then retry.");
                this.returnCode.HasWriteError = true;
                //TODO: Write code to sleep and retry.
            }

            if (apiRC != "0")
            {
                Log.LogError("Error with RC=[{0}].", apiRC);
                this.returnCode.IsFailure = true;
            }
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
