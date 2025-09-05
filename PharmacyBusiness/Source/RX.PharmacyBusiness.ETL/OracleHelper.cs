namespace RX.PharmacyBusiness.ETL
{
    using Dapper;
    using Oracle.ManagedDataAccess.Client;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Wegmans.PharmacyLibrary.IO;
    using Wegmans.PharmacyLibrary.Logging;

    public class OracleHelper
    {
        private IFileManager fileManager { get; set; }
        private string sqlForRepeatedUse { get; set; }
        private int counterOfRepeatedUseSql { get; set; }
        private OracleConnection orclConForRepeatedUse { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        public OracleHelper()
        {
            //For for more information on Oracle Mananged Data Access, see
            // https://www.nuget.org/packages/Oracle.ManagedDataAccess.Core/
            // https://docs.oracle.com/en/database/oracle/oracle-database/19/odpnt/OracleConfigurationClass.html#GUID-F1F71D03-CBB8-4173-A883-0878DA84D11A
            //NOTE: Tns Descriptor, and other OracleConfiguration settings, are in config so set Test and Prod configs accordingly.
            OracleConfiguration.OracleDataSources.Add("ENTERPRISE_RX", ConfigurationManager.AppSettings["OracleConfiguration_TnsDescriptor_ERx"].ToString());
            //OracleConfiguration.OracleDataSources.Add("SCRIPTS_BATCH", ConfigurationManager.AppSettings["OracleConfiguration_TnsDescriptor_Scripts"].ToString());
            //OracleConfiguration.OracleDataSources.Add("RXSPECIALTY_BATCH", ConfigurationManager.AppSettings["OracleConfiguration_TnsDescriptor_Specialty"].ToString());
            OracleConfiguration.SelfTuning = false;
            OracleConfiguration.BindByName = true;
            OracleConfiguration.CommandTimeout = Convert.ToInt32(ConfigurationManager.AppSettings["OracleConfiguration_CommandTimeout"].ToString());
            OracleConfiguration.FetchSize = 1024 * 1024; //About 1 MB
            OracleConfiguration.TraceOption = Convert.ToInt32(ConfigurationManager.AppSettings["OracleConfiguration_TraceOption"].ToString());
            OracleConfiguration.TraceFileLocation = ConfigurationManager.AppSettings["OracleConfiguration_TraceFileLocation"].ToString();
            OracleConfiguration.TraceLevel = Convert.ToInt32(ConfigurationManager.AppSettings["OracleConfiguration_TraceLevel"].ToString());
            OracleConfiguration.DisableOOB = true;
            OracleConfiguration.SqlNetWalletOverride = true;
            OracleConfiguration.WalletLocation = ConfigurationManager.AppSettings["OracleConfiguration_WalletLocation"].ToString();
            OracleConfiguration.TnsAdmin = ConfigurationManager.AppSettings["OracleConfiguration_TnsAdmin"].ToString();

            this.fileManager = this.fileManager ?? new FileManager();
        }

        public void DownloadQueryByRunDateToFile(
            int fetchSizeMultiplier,
            string sqlFile,
            string downloadFile,
            DateTime runDate,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            string connectionName)
        {
            this.DownloadQueryByRunDateToFile(
               fetchSizeMultiplier,
               sqlFile,
               downloadFile,
               runDate,
               hasHeaderRow,
               false,
               delimiter,
               textQualifier,
               makeExtractWhenNoData,
               connectionName);
        }

            /// <summary>
            /// Executes the query and outputs results to a file.
            /// The Oracle Reader execution used here differs from generic Dapper queries by setting FetchSize for performance tuning.
            /// </summary>
            /// <param name="fetchSizeMultiplier">A multiplier used to set Oracle Reader FetchSize property.</param>
            /// <param name="sqlFile">The file path and name holding the SQL query executed by the Oracle Reader.</param>
            /// <param name="downloadFile">The file path and name where the query results will be written to.</param>
            /// <param name="runDate">Run Date is the date the batch job would be run on, not that data date which is typically the day prior. The queries used in this class are all date driven. </param>
            /// <param name="hasHeaderRow">Boolean to output a header row based on query field names.</param>
            /// <param name="delimiter">Data file delimiter, which for feeds to 1010data are typically a pipe character.</param>
            /// <param name="textQualifier">Text Qualifier is typically double quote character that surrounds a text type data field in a CSV file needed when importing into Excel.</param>
            /// <param name="makeExtractWhenNoData">Set to false when you do not want to create a blank file, or true when you do want to create a file anyways when there is no data.</param>
            /// <param name="connectionName">The Oracle TNS Name of the database connection.</param>
            public void DownloadQueryByRunDateToFile(
            int fetchSizeMultiplier,
            string sqlFile,
            string downloadFile,
            DateTime runDate,
            bool hasHeaderRow,
            bool writeHeaderRowEvenIfNoData,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            string connectionName)
        {
            sqlFile = Environment.ExpandEnvironmentVariables(sqlFile);
            downloadFile = Environment.ExpandEnvironmentVariables(downloadFile);
            delimiter = delimiter ?? string.Empty;
            textQualifier = textQualifier ?? string.Empty;

            Log.LogInfo("Output results from query file [{0}] with FetchSizeMultiplier [{1}] and Run Date [{2}] to file [{3}].", sqlFile, fetchSizeMultiplier, runDate.ToString("yyyyMMdd"), downloadFile);
            DateTime queryStartTime;
            DateTime queryStopTime;
            DateTime downloadStartTime;
            DateTime downloadStopTime;
            int rowCounter = 0;
            var output = new StringBuilder();
            var fields = new Collection<string>();
            string selectSql = string.Empty;
            using (var fileReader = new StreamReader(this.fileManager.OpenRead(sqlFile)))
            {
                selectSql = fileReader.ReadToEnd();
            }

            OracleParameter pRunDate = new OracleParameter("RunDate", OracleDbType.Date, ParameterDirection.Input);
            pRunDate.Value = runDate;
            OracleConnection orclCon = null;

            try
            {
                orclCon = new OracleConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString());
                orclCon.Open();
                
                OracleCommand orclCmd = orclCon.CreateCommand();
                orclCmd.CommandType = CommandType.Text;
                orclCmd.CommandText = selectSql;
                orclCmd.AddToStatementCache = false;
                orclCmd.BindByName = true;
                orclCmd.Parameters.Clear();
                orclCmd.Parameters.Add(pRunDate);

                using (OracleDataReader oracleReader = orclCmd.ExecuteReader())
                {
                    queryStartTime = DateTime.Now;
                    oracleReader.FetchSize = orclCmd.RowSize * fetchSizeMultiplier;
                    queryStopTime = DateTime.Now;

                    using (var writerOutputData = this.fileManager.OpenWriter(downloadFile, false))
                    {
                        downloadStartTime = DateTime.Now;
                        while (oracleReader.Read())
                        {
                            rowCounter++;
                            fields.Clear();

                            if (hasHeaderRow)
                            {
                                for (int i = 0; i < oracleReader.FieldCount; i++)
                                {
                                    fields.Add(string.Format("{0}{1}{0}", textQualifier, oracleReader.GetName(i)));
                                }

                                writerOutputData.WriteLine(string.Join(delimiter, fields.ToArray()));
                                fields.Clear();
                                hasHeaderRow = false;
                            }

                            for (int i = 0; i < oracleReader.FieldCount; i++)
                            {
                                if (!string.IsNullOrEmpty(delimiter) &&
                                    string.IsNullOrEmpty(textQualifier) &&
                                    oracleReader.GetValue(i).ToString().IndexOf(delimiter) > -1)
                                    Log.LogWarn("Input file contains embedded delimiter [{0}] row [{1}] col [{2}]",
                                        delimiter, rowCounter, (i + 1));

                                if (oracleReader.GetValue(i).GetType() == typeof(System.String))
                                {
                                    fields.Add(string.Format("{0}{1}{0}", textQualifier, oracleReader.GetValue(i).ToString()));
                                }
                                else
                                {
                                    fields.Add(oracleReader.GetValue(i).ToString());
                                }
                            }

                            writerOutputData.WriteLine(string.Join(delimiter, fields.ToArray()));
                        }

                        if (writeHeaderRowEvenIfNoData && makeExtractWhenNoData && hasHeaderRow && rowCounter == 0)
                        {
                            //Output header row from schema
                            //https://docs.oracle.com/cd/E85694_01/ODPNT/DataReaderGetSchemaTable.htm

                            //get the schema table
                            DataTable schemaTable = oracleReader.GetSchemaTable();
                            DataRowCollection schemaRows = schemaTable.Rows;

                            //print out the column info
                            fields.Clear();
                            for (int i = 0; i < schemaRows.Count; i++)
                            {
                                fields.Add(string.Format("{0}{1}{0}", textQualifier, schemaRows[i]["COLUMNNAME"]));
                            }

                            writerOutputData.WriteLine(string.Join(delimiter, fields.ToArray()));
                        }

                        oracleReader.Dispose();
                        downloadStopTime = DateTime.Now;
                    }

                    if (!makeExtractWhenNoData && rowCounter == 0)
                    { 
                        Log.LogInfo("Query returned zero rows and job is set to not create an empty file, so now deleting this empty output file.");
                        this.fileManager.DeleteFile(downloadFile);
                    }
                }

                orclCmd.Parameters.Clear();
                orclCmd.Dispose();
                orclCon.Close();
            }
            finally
            {
                if (null != orclCon)
                    orclCon.Close();
            }

            TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
            double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
            TimeSpan tsDifferenceForDownload = downloadStopTime - downloadStartTime;
            double tsSecondsForDownload = Math.Round(tsDifferenceForDownload.TotalSeconds, 3);
            Log.LogInfo("Query ran for [{0}] seconds, returned [{1}] rows, and the data downloaded in [{2}] seconds.", tsSecondsForQuery, rowCounter, tsSecondsForDownload);
        }

        /// <summary>
        /// Returns results to a typed List.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fetchSizeMultiplier"></param>
        /// <param name="sqlFile"></param>
        /// <param name="runDate"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public List<T> DownloadQueryByRunDateToList<T>(
            int fetchSizeMultiplier,
            string sqlFile,
            DateTime runDate,
            string connectionName)
        {
            List<T> returnList = new List<T>();
            sqlFile = Environment.ExpandEnvironmentVariables(sqlFile);
            
            Log.LogInfo("Output results from query file [{0}] with FetchSizeMultiplier [{1}] and Run Date [{2}] to a typed List<T>.", sqlFile, fetchSizeMultiplier, runDate.ToString("yyyyMMdd"));
            DateTime queryStartTime;
            DateTime queryStopTime;
            string selectSql = string.Empty;
            using (var fileReader = new StreamReader(this.fileManager.OpenRead(sqlFile)))
            {
                selectSql = fileReader.ReadToEnd();
            }
            OracleConnection orclCon = null;

            try
            {
                orclCon = new OracleConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString());
                orclCon.Open();
                queryStartTime = DateTime.Now;
                returnList = orclCon.Query<T>(selectSql, new { RunDate = runDate }).ToList();
                queryStopTime = DateTime.Now;
                orclCon.Close();
            }
            finally
            {
                if (null != orclCon)
                    orclCon.Close();
            }

            TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
            double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
            Log.LogInfo("Query ran for [{0}] seconds, returned [{1}] rows.", tsSecondsForQuery, returnList.Count);
            return returnList;
        }

        /// <summary>
        /// Returns results to a typed List.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fetchSizeMultiplier"></param>
        /// <param name="sqlFile"></param>
        /// <param name="runDate"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public List<T> DownloadQueryWithLiteralsToList<T>(
            int fetchSizeMultiplier,
            string sqlFile,
            Dictionary<string, string> findReplaceLiteralParams,
            string connectionName)
        {
            List<T> returnList = new List<T>();
            sqlFile = Environment.ExpandEnvironmentVariables(sqlFile);

            Log.LogInfo("Output results from query file [{0}] with FetchSizeMultiplier [{1}] and literals to a typed List<T>.", sqlFile, fetchSizeMultiplier);
            DateTime queryStartTime;
            DateTime queryStopTime;
            string selectSql = string.Empty;
            using (var fileReader = new StreamReader(this.fileManager.OpenRead(sqlFile)))
            {
                selectSql = fileReader.ReadToEnd();
            }

            foreach (var findReplaceLiteralParam in findReplaceLiteralParams)
            {
                selectSql = selectSql.Replace(findReplaceLiteralParam.Key, findReplaceLiteralParam.Value);
            }

            OracleConnection orclCon = null;

            try
            {
                orclCon = new OracleConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString());
                orclCon.Open();
                queryStartTime = DateTime.Now;
                returnList = orclCon.Query<T>(selectSql).ToList();
                queryStopTime = DateTime.Now;
                orclCon.Close();
            }
            finally
            {
                if (null != orclCon)
                    orclCon.Close();
            }

            TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
            double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
            Log.LogInfo("Query ran for [{0}] seconds, returned [{1}] rows.", tsSecondsForQuery, returnList.Count);
            return returnList;
        }

        /// <summary>
        /// Returns results to a typed List.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fetchSizeMultiplier"></param>
        /// <param name="sqlFile"></param>
        /// <param name="queryParams"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public List<T> DownloadQueryToList<T>(
            int fetchSizeMultiplier,
            string sqlFile,
            object queryParams,
            string connectionName)
        {
            List<T> returnList = new List<T>();
            sqlFile = Environment.ExpandEnvironmentVariables(sqlFile);

            Log.LogInfo("Output results from query file [{0}] with FetchSizeMultiplier [{1}] to a typed List<T>.", sqlFile, fetchSizeMultiplier);
            DateTime queryStartTime;
            DateTime queryStopTime;
            string selectSql = string.Empty;
            using (var fileReader = new StreamReader(this.fileManager.OpenRead(sqlFile)))
            {
                selectSql = fileReader.ReadToEnd();
            }
            OracleConnection orclCon = null;

            try
            {
                orclCon = new OracleConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString());
                orclCon.Open();
                queryStartTime = DateTime.Now;
                returnList = orclCon.Query<T>(selectSql, queryParams).ToList();
                queryStopTime = DateTime.Now;
                orclCon.Close();
            }
            finally
            {
                if (null != orclCon)
                    orclCon.Close();
            }

            TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
            double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
            Log.LogInfo("Query ran for [{0}] seconds, returned [{1}] rows.", tsSecondsForQuery, returnList.Count);
            return returnList;
        }

        public void OpenConnectionAndSqlForRepeatedUse(string sqlFile, string connectionName)
        {
            Log.LogInfo("Setting SQL from query file [{0}] and opening db connection.", sqlFile);
            sqlFile = Environment.ExpandEnvironmentVariables(sqlFile);
            using (var fileReader = new StreamReader(this.fileManager.OpenRead(sqlFile)))
            {
                this.sqlForRepeatedUse = fileReader.ReadToEnd();
            }
            this.counterOfRepeatedUseSql = 0;
            this.orclConForRepeatedUse = new OracleConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString());
            this.orclConForRepeatedUse.Open();
        }

        public void CloseConnectionAndResetSqlForRepeatedUse()
        {
            Log.LogInfo("Resetting SQL and closing db connection, which were ran [{0}] times.", this.counterOfRepeatedUseSql);
            if (null != this.orclConForRepeatedUse)
                this.orclConForRepeatedUse.Close();

            this.orclConForRepeatedUse = new OracleConnection();
            this.sqlForRepeatedUse = string.Empty;
            this.counterOfRepeatedUseSql = 0;
        }

        /// <summary>
        /// Returns results to a typed List. Optimized for repeated use.
        /// No logging because that would add tens of thousands of rows of logging.
        /// </summary>
        /// <typeparam name="T">Object class</typeparam>
        /// <param name="queryParams">Query parameter object</param>
        /// <returns></returns>
        public List<T> DownloadQueryToListForRepeatedUse<T>(object queryParams)
        {
            this.counterOfRepeatedUseSql++;
            return this.orclConForRepeatedUse.Query<T>(sqlForRepeatedUse, queryParams).ToList();
        }

        /// <summary>
        /// Execute a non-query procedure.
        /// </summary>
        /// <param name="connectionName">TNS connection name</param>
        /// <param name="procedureName">Full name of procedure</param>
        /// <param name="procedureParams">Array of Oracle parameters</param>
        public void CallNonQueryProcedure(string connectionName, string procedureName, OracleParameter[] procedureParams)
        {
            Log.LogInfo("Calling non query procedure [{0}].", procedureName);
            DateTime queryStartTime;
            DateTime queryStopTime;
            OracleConnection orclCon = null;

            try
            {
                orclCon = new OracleConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString());
                orclCon.Open();

                OracleCommand orclCmd = orclCon.CreateCommand();
                orclCmd.CommandType = CommandType.StoredProcedure;
                orclCmd.CommandText = procedureName;
                orclCmd.AddToStatementCache = false;
                orclCmd.BindByName = true;
                orclCmd.Parameters.Clear();

                foreach (OracleParameter param in procedureParams)
                {
                    orclCmd.Parameters.Add(param);
                }

                queryStartTime = DateTime.Now;
                orclCmd.ExecuteNonQuery();
                queryStopTime = DateTime.Now;

                orclCmd.Parameters.Clear();
                orclCmd.Dispose();
                orclCon.Close();
            }
            finally
            {
                if (null != orclCon)
                    orclCon.Close();
            }

            TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
            double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
            Log.LogInfo("Procedure ran for [{0}] seconds.", tsSecondsForQuery);
        }

        /// <summary>
        /// Execute a procedure that returns a ref cursor and download results to a file.
        /// </summary>
        /// <param name="connectionName">TNS connection name</param>
        /// <param name="procedureName">Full name of procedure</param>
        /// <param name="procedureParams">Array of Oracle parameters</param>
        public void DownloadRefCursorFunctionToFile(
            string connectionName, 
            string functionName, 
            OracleParameter[] procedureParams,
            string downloadFile,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData)
        {
            downloadFile = Environment.ExpandEnvironmentVariables(downloadFile);
            delimiter = delimiter ?? string.Empty;
            textQualifier = textQualifier ?? string.Empty;

            Log.LogInfo("Output results from RefCursor query procedure [{0}] to file [{1}].", functionName, downloadFile);
            DateTime queryStartTime;
            DateTime queryStopTime;
            OracleConnection orclCon = null;
            DataSet ds = new DataSet();

            try
            {
                orclCon = new OracleConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString());
                orclCon.Open();

                OracleCommand orclCmd = orclCon.CreateCommand();
                orclCmd.CommandType = CommandType.StoredProcedure;
                orclCmd.CommandText = functionName;
                orclCmd.AddToStatementCache = false;
                orclCmd.BindByName = true;
                orclCmd.Parameters.Clear();

                foreach (OracleParameter param in procedureParams)
                {
                    orclCmd.Parameters.Add(param);
                }

                queryStartTime = DateTime.Now;
                OracleDataAdapter da = new OracleDataAdapter(orclCmd);
                da.Fill(ds);
                queryStopTime = DateTime.Now;

                orclCmd.Parameters.Clear();
                orclCmd.Dispose();
                orclCon.Close();

                Log.LogInfo("DataSet table has [{0}] rows and [{1}] cols.", ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count);
                int rowCount = 0;
                int colCount = 0;
                using (var writerOutputData = this.fileManager.OpenWriter(downloadFile, false))
                { 
                    StringBuilder sb = new StringBuilder();
                    foreach (DataTable tbl in ds.Tables)
                    {
                        if (hasHeaderRow)
                        {
                            foreach (DataColumn col in tbl.Columns)
                            {
                                if (col.ColumnName != tbl.Columns[0].ColumnName)
                                {
                                    //Skip adding a delimiter for the first column.
                                    sb.Append(delimiter.ToString());
                                }
                                else
                                {
                                    //This is the first column.

                                    //Known bug with CSV files opened in Excel:
                                    //  If the first two characters of the exported file are upper case "ID", then
                                    //  you get the error "SYLK: File format is not valid".
                                    //  See http://support.microsoft.com/kb/323626
                                    if (col.ColumnName.IndexOf("ID") == 0)
                                    {
                                        //Either make it lower case, or add an apostrophe.
                                        sb.Append("'");
                                    }
                                }

                                //Output column names.
                                sb.Append(string.Format("{0}{1}{0}", textQualifier, col.ColumnName));
                            }

                            //Line return
                            sb.Append("\r\n");
                        }

                        foreach (DataRow row in tbl.Rows)
                        {
                            rowCount++;
                            for (int i = 0; i <= row.ItemArray.Length - 1; i++)
                            {
                                if (rowCount == 1)
                                    colCount++;

                                if (i > 0)
                                    sb.Append(delimiter.ToString());

                                if (downloadFile.ToUpper().EndsWith("CSV") &&
                                    textQualifier.Equals(((char)34).ToString()))
                                {
                                    string cellValue = row[i].ToString();

                                    if (cellValue.IndexOf(delimiter) > -1)
                                        Log.LogWarn("Input file contains embedded delimiter [{0}] row [{1}] col [{2}]",
                                            delimiter, rowCount, (i+1));

                                    if (cellValue.IndexOf("\"") > -1 || cellValue.IndexOf(",") > -1)
                                    {
                                        //Escape double quotes.
                                        cellValue = cellValue.Replace("\"", "\"\"");
                                        sb.Append(string.Format("{0}{1}{0}", textQualifier, cellValue));
                                    }
                                    else
                                    {
                                        if (string.IsNullOrEmpty(cellValue))
                                        {
                                            sb.Append(string.Empty);
                                        }
                                        else
                                        {
                                            if (row[i].GetType() == typeof(System.String))
                                            {
                                                sb.Append(string.Format("{0}{1}{0}", textQualifier, cellValue));
                                            }
                                            else
                                            {
                                                sb.Append(cellValue);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    sb.Append(row[i].ToString());
                                }
                            }

                            writerOutputData.WriteLine(sb.ToString());
                            sb.Clear();
                        }
                    }
                }
                Log.LogInfo("Output file has [{0}] rows and [{1}] cols.", rowCount, colCount);

                if (!makeExtractWhenNoData && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    Log.LogInfo("Query returned zero rows and job is set to not create an empty file, so now deleting this empty output file.");
                    this.fileManager.DeleteFile(downloadFile);
                }
            }
            finally
            {
                if (null != orclCon)
                    orclCon.Close();
            }

            TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
            double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
            Log.LogInfo("Procedure ran for [{0}] seconds.", tsSecondsForQuery);
        }

        public void CallNonQueryAnonymousBlock(
            string connectionName, 
            string sqlFile, 
            OracleParameter[] procedureParams)
        {
            sqlFile = Environment.ExpandEnvironmentVariables(sqlFile);
            Log.LogInfo("Calling non query anonymous block in file [{0}].", sqlFile);

            string plSql = string.Empty;
            using (var fileReader = new StreamReader(this.fileManager.OpenRead(sqlFile)))
            {
                plSql = fileReader.ReadToEnd();
            }

            DateTime queryStartTime;
            DateTime queryStopTime;
            OracleConnection orclCon = null;

            try
            {
                orclCon = new OracleConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString());

                if (orclCon.State != ConnectionState.Open)
                    orclCon.Open();

                OracleCommand orclCmd = orclCon.CreateCommand();
                orclCmd.CommandType = CommandType.Text;
                orclCmd.CommandText = plSql;
                orclCmd.AddToStatementCache = true;
                orclCmd.BindByName = true;
                orclCmd.Parameters.Clear();

                foreach (OracleParameter param in procedureParams)
                {
                    orclCmd.Parameters.Add(param);
                }

                queryStartTime = DateTime.Now;
                orclCmd.ExecuteNonQuery();
                queryStopTime = DateTime.Now;

                orclCmd.Parameters.Clear();
                orclCmd.Dispose();
                orclCon.Close();
            }
            finally
            {
                if (null != orclCon)
                    orclCon.Close();
            }

            TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
            double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
            Log.LogInfo("Procedure ran for [{0}] seconds.", tsSecondsForQuery);
        }

        public long DownloadImagesFromBlobsToFiles(
            int fetchSizeMultiplier,
            string sqlFile,
            string outputPath,
            string connectionName)
        {
            long fileCount = 0;
            sqlFile = Environment.ExpandEnvironmentVariables(sqlFile);
            string downloadPath = Environment.ExpandEnvironmentVariables(outputPath);

            Log.LogInfo("Output images from Blobs in Oracle to files located here [{0}].", downloadPath);
            DateTime queryStartTime;
            DateTime queryStopTime;
            DateTime downloadStartTime;
            DateTime downloadStopTime;
            int rowCounter = 0;
            string selectSql = string.Empty;
            using (var fileReader = new StreamReader(this.fileManager.OpenRead(sqlFile)))
            {
                selectSql = fileReader.ReadToEnd();
            }

            OracleConnection orclCon = null;

            try
            {
                orclCon = new OracleConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString());
                orclCon.Open();

                OracleCommand orclCmd = orclCon.CreateCommand();
                orclCmd.CommandType = CommandType.Text;
                orclCmd.CommandText = selectSql;
                orclCmd.AddToStatementCache = false;
                orclCmd.BindByName = true;
                orclCmd.Parameters.Clear();

                using (OracleDataReader oracleReader = orclCmd.ExecuteReader())
                {
                    queryStartTime = DateTime.Now;
                    oracleReader.FetchSize = orclCmd.RowSize * fetchSizeMultiplier;
                    queryStopTime = DateTime.Now;

                    downloadStartTime = DateTime.Now;
                    while (oracleReader.Read())
                    {
                        rowCounter++;

                        string imageName = oracleReader.GetValue(0).ToString();
                        if (!string.IsNullOrEmpty(imageName))
                        {
                            fileCount++;
                            byte[] rawData = (byte[])oracleReader[1];
                            string downloadPathAndFileName = Path.Combine(downloadPath, imageName);
 
                            /* TODO: Write directly to Azure storage.
                            using (FileStream fs = new FileStream(downloadPathAndFileName, FileMode.OpenOrCreate, FileAccess.Write))
                            {
                                fs.Write(rawData, 0, rawData.Length);
                            }
                            */
                        }
                    }

                    oracleReader.Dispose();
                    downloadStopTime = DateTime.Now;
                }

                orclCmd.Parameters.Clear();
                orclCmd.Dispose();
                orclCon.Close();
            }
            finally
            {
                if (null != orclCon)
                    orclCon.Close();
            }

            TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
            double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
            TimeSpan tsDifferenceForDownload = downloadStopTime - downloadStartTime;
            double tsSecondsForDownload = Math.Round(tsDifferenceForDownload.TotalSeconds, 3);
            Log.LogInfo("Download ran for [{0}] seconds, returned [{1}] files, and the files downloaded in [{2}] seconds.", tsSecondsForQuery, rowCounter, tsSecondsForDownload);

            return fileCount;
        }
    }
}
