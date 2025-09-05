namespace RX.PharmacyBusiness.ETL
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Configuration;
    using System.Data;
    using System.Data.OleDb;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Wegmans.PharmacyLibrary.IO;
    using Wegmans.PharmacyLibrary.Logging;

    public class NetezzaHelper
    {
        private IFileManager fileManager { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        public NetezzaHelper()
        {
            this.fileManager = this.fileManager ?? new FileManager();
        }

        /// <summary>
        /// Returns query results to a DataSet.
        /// </summary>
        /// <param name="sqlFile"></param>
        /// <param name="runDate"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public DataSet DownloadQueryByRunDateToDataSet(
           string sqlFile,
           DateTime runDate,
           string connectionName)
        {
            DataSet ds = new DataSet();
            sqlFile = Environment.ExpandEnvironmentVariables(sqlFile);

            Log.LogInfo("Output results from query file [{0}] with Run Date [{1}] to DataSet.", sqlFile, runDate.ToString("yyyyMMdd"));
            DateTime queryStartTime;
            DateTime queryStopTime;
            string selectSql = string.Empty;
            using (var fileReader = new StreamReader(this.fileManager.OpenRead(sqlFile)))
            {
                selectSql = fileReader.ReadToEnd();
            }
            if (selectSql == string.Empty)
                throw new Exception("Cannot find SQL file [" + sqlFile + "].");

            OleDbParameter pRunDate = new OleDbParameter("@RunDate", SqlDbType.Date);
            pRunDate.Value = runDate;
            OleDbConnection sqlsCon = null;

            try
            {
                sqlsCon = new OleDbConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString());
                sqlsCon.Open();

                OleDbCommand cmd = new OleDbCommand(selectSql, sqlsCon);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 900;
                cmd.Parameters.Clear();
                cmd.Parameters.Add(pRunDate);

                queryStartTime = DateTime.Now;
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(ds);
                queryStopTime = DateTime.Now;

                cmd.Parameters.Clear();
                cmd.Dispose();
                sqlsCon.Close();
            }
            finally
            {
                if (null != sqlsCon)
                    sqlsCon.Close();
            }

            TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
            double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
            Log.LogInfo("Query ran for [{0}] seconds, and returned a DataTable with [{1}] rows and [{2}] cols.", tsSecondsForQuery, ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count);
            return ds;
        }

        public DataSet DownloadQueryWithParamsToDataSet(
           string sqlFile,
           OleDbParameter[] queryParams,
           string connectionName)
        {
            DataSet ds = new DataSet();
            sqlFile = Environment.ExpandEnvironmentVariables(sqlFile);

            Log.LogInfo("Output results from query file [{0}] with query parameters passed in to DataSet.", sqlFile);
            DateTime queryStartTime;
            DateTime queryStopTime;
            string selectSql = string.Empty;
            using (var fileReader = new StreamReader(this.fileManager.OpenRead(sqlFile)))
            {
                selectSql = fileReader.ReadToEnd();
            }
            if (selectSql == string.Empty)
                throw new Exception("Cannot find SQL file [" + sqlFile + "].");
            
            OleDbConnection sqlsCon = null;

            try
            {
                sqlsCon = new OleDbConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString());
                sqlsCon.Open();

                OleDbCommand cmd = new OleDbCommand(selectSql, sqlsCon);
                cmd.CommandType = CommandType.Text;
                cmd.CommandTimeout = 900;
                cmd.Parameters.Clear();

                foreach (OleDbParameter param in queryParams)
                {
                    cmd.Parameters.Add(param);
                }

                queryStartTime = DateTime.Now;
                OleDbDataAdapter da = new OleDbDataAdapter(cmd);
                da.Fill(ds);
                queryStopTime = DateTime.Now;

                cmd.Parameters.Clear();
                cmd.Dispose();
                sqlsCon.Close();
            }
            finally
            {
                if (null != sqlsCon)
                    sqlsCon.Close();
            }

            TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
            double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
            Log.LogInfo("Query ran for [{0}] seconds, and returned a DataTable with [{1}] rows and [{2}] cols.", tsSecondsForQuery, ds.Tables[0].Rows.Count, ds.Tables[0].Columns.Count);
            return ds;
        }

        /// <summary>
        /// Returns query results to a typed List.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fetchSizeMultiplier"></param>
        /// <param name="sqlFile"></param>
        /// <param name="runDate"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        /*
        public List<T> DownloadQueryByRunDateToList<T>(
            string sqlFile,
            DateTime runDate,
            string connectionName)
        {
            List<T> returnList = new List<T>();
            sqlFile = Environment.ExpandEnvironmentVariables(sqlFile);

            Log.LogInfo("Output results from query file [{0}] with Run Date [{1}] to a typed List<T>.", sqlFile, runDate.ToString("yyyyMMdd"));
            DateTime queryStartTime;
            DateTime queryStopTime;
            string selectSql = string.Empty;
            using (var fileReader = new StreamReader(this.fileManager.OpenRead(sqlFile)))
            {
                selectSql = fileReader.ReadToEnd();
            }

            OleDbConnection sqlsCon = null;

            try
            {
                sqlsCon = new OleDbConnection(ConfigurationManager.ConnectionStrings[connectionName].ToString());
                sqlsCon.Open();
                queryStartTime = DateTime.Now;
                returnList = sqlsCon.Query<T>(selectSql, new { RunDate = runDate }).ToList();
                queryStopTime = DateTime.Now;
                sqlsCon.Close();
            }
            finally
            {
                if (null != sqlsCon)
                    sqlsCon.Close();
            }

            TimeSpan tsDifferenceForQuery = queryStopTime - queryStartTime;
            double tsSecondsForQuery = Math.Round(tsDifferenceForQuery.TotalSeconds, 3);
            Log.LogInfo("Query ran for [{0}] seconds, returned [{1}] rows.", tsSecondsForQuery, returnList.Count);
            return returnList;
        }
        */

        /// <summary>
        /// Outputs DataTable to a file.
        /// </summary>
        /// <param name="downloadDataTable"></param>
        /// <param name="downloadFile"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        public void WriteDataTableToFile(
            DataTable downloadDataTable,
            string downloadFile,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData)
        {
            this.WriteDataTableToFile(
                downloadDataTable,
                downloadFile,
                hasHeaderRow,
                delimiter,
                textQualifier,
                makeExtractWhenNoData,
                false);
        }

        /// <summary>
        /// Outputs DataSet to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="downloadDataTable"></param>
        /// <param name="downloadFile"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        /// <param name="shouldAppendToExistingFile"></param>
        public void WriteDataTableToFile(
            DataTable downloadDataTable,
            string downloadFile,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            bool shouldAppendToExistingFile)
        {
            downloadFile = Environment.ExpandEnvironmentVariables(downloadFile);
            delimiter = delimiter ?? string.Empty;
            textQualifier = textQualifier ?? string.Empty;

            Log.LogInfo("Output data in DataTable to output file [{0}].", downloadFile);

            using (var writerOutputData = this.fileManager.OpenWriter(downloadFile, shouldAppendToExistingFile))
            {
                StringBuilder sb = new StringBuilder();
                if (hasHeaderRow)
                {
                    foreach (DataColumn col in downloadDataTable.Columns)
                    {
                        if (col.ColumnName != downloadDataTable.Columns[0].ColumnName)
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
                foreach (DataRow row in downloadDataTable.Rows)
                {
                    for (int i = 0; i <= row.ItemArray.Length - 1; i++)
                    {
                        if (i > 0)
                            sb.Append(delimiter.ToString());

                        if (downloadFile.ToUpper().EndsWith("CSV") &&
                            textQualifier.Equals(((char)34).ToString()))
                        {
                            string cellValue = row[i].ToString();

                            if (!string.IsNullOrEmpty(delimiter) && cellValue.IndexOf(delimiter) > -1)
                                Log.LogWarn("Input file contains embedded delimiter [{0}]", delimiter);

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

            if (!makeExtractWhenNoData && downloadDataTable.Rows.Count == 0)
            {
                Log.LogInfo("List has zero rows and job is set to not create an empty file, so now deleting this empty output file.");
                this.fileManager.DeleteFile(downloadFile);
            }
        }

        /// <summary>
        /// Outputs a typed List to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="downloadList"></param>
        /// <param name="downloadFile"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        public void WriteListToFile<T>(
            List<T> downloadList,
            string downloadFile,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData)
        {
            this.WriteListToFile<T>(
               downloadList,
               downloadFile,
               hasHeaderRow,
               delimiter,
               textQualifier,
               makeExtractWhenNoData,
               false);
        }

        /// <summary>
        /// Outputs a typed List to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="downloadList"></param>
        /// <param name="downloadFile"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        /// <param name="shouldAppendToExistingFile"></param>
        public void WriteListToFile<T>(
            List<T> downloadList,
            string downloadFile,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            bool shouldAppendToExistingFile)
        {
            downloadFile = Environment.ExpandEnvironmentVariables(downloadFile);
            delimiter = delimiter ?? string.Empty;
            textQualifier = textQualifier ?? string.Empty;
            var output = new StringBuilder();
            var fields = new Collection<string>();
            Type elementType = typeof(T);

            Log.LogInfo("Output data in typed List<T> to output file [{0}].", downloadFile);

            //If the file already exists and shouldAppendToExistingFile=true, then do not output the header row with the append.
            hasHeaderRow = (
                hasHeaderRow &&
                shouldAppendToExistingFile &&
                this.fileManager.FileExists(downloadFile)) ?
                    false : hasHeaderRow;

            using (var writerOutputData = this.fileManager.OpenWriter(downloadFile, shouldAppendToExistingFile))
            {
                if (hasHeaderRow)
                {
                    foreach (var propInfo in elementType.GetProperties())
                    {
                        fields.Add(string.Format("{0}{1}{0}", textQualifier, propInfo.Name));
                    }

                    writerOutputData.WriteLine(string.Join(delimiter, fields.ToArray()));
                    fields.Clear();
                    hasHeaderRow = false;
                }

                foreach (T record in downloadList)
                {
                    fields.Clear();

                    foreach (var propInfo in elementType.GetProperties())
                    {
                        if ((propInfo.GetValue(record, null) ?? DBNull.Value).GetType() == typeof(System.String))
                        {
                            fields.Add(string.Format("{0}{1}{0}", textQualifier, (propInfo.GetValue(record, null) ?? DBNull.Value).ToString()));
                        }
                        else
                        {
                            fields.Add((propInfo.GetValue(record, null) ?? DBNull.Value).ToString());
                        }
                    }

                    writerOutputData.WriteLine(string.Join(delimiter, fields.ToArray()));
                }
            }

            if (!makeExtractWhenNoData && downloadList.Count == 0)
            {
                Log.LogInfo("List has zero rows and job is set to not create an empty file, so now deleting this empty output file.");
                this.fileManager.DeleteFile(downloadFile);
            }
        }


        /// <summary>
        /// Outputs query results to a file.
        /// </summary>
        /// <param name="sqlFile"></param>
        /// <param name="downloadFile"></param>
        /// <param name="runDate"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        /// <param name="connectionName"></param>
        /// <returns></returns>
        public int DownloadQueryByRunDateToFile(
            string sqlFile,
            string downloadFile,
            DateTime runDate,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            string connectionName)
        {
            return this.DownloadQueryByRunDateToFile(
               sqlFile,
               downloadFile,
               runDate,
               hasHeaderRow,
               delimiter,
               textQualifier,
               makeExtractWhenNoData,
               connectionName,
               false);
        }

        /// <summary>
        /// Outputs query results to a file, with the option to append to an existing file.
        /// </summary>
        /// <param name="sqlFile"></param>
        /// <param name="downloadFile"></param>
        /// <param name="runDate"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        /// <param name="connectionName"></param>
        /// <param name="shouldAppendToExistingFile"></param>
        /// <returns></returns>
        public int DownloadQueryByRunDateToFile(
            string sqlFile,
            string downloadFile,
            DateTime runDate,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            string connectionName,
            bool shouldAppendToExistingFile)
        {
            Log.LogInfo("Output results from query file [{0}] and Run Date [{1}] to output file [{2}].", sqlFile, runDate.ToString("yyyyMMdd"), downloadFile);
            int returnCount = 0;
            DataSet ds = this.DownloadQueryByRunDateToDataSet(sqlFile, runDate, connectionName);

            if (ds.Tables.Count > 0)
            {
                this.WriteDataTableToFile(
                    ds.Tables[0],
                    downloadFile,
                    hasHeaderRow,
                    delimiter,
                    textQualifier,
                    makeExtractWhenNoData,
                    shouldAppendToExistingFile);
                returnCount = ds.Tables[0].Rows.Count;
            }

            Log.LogInfo("Wrote [{0}] rows to output file.", returnCount);
            return returnCount;
        }

        public int DownloadQueryWithParamsToFile(
            string sqlFile,
            string downloadFile,
            OleDbParameter[] queryParams,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            string connectionName,
            bool shouldAppendToExistingFile)
        {
            Log.LogInfo("Output results from query file [{0}] with query params passed in to output file [{1}].", sqlFile, downloadFile);
            int returnCount = 0;
            DataSet ds = this.DownloadQueryWithParamsToDataSet(sqlFile, queryParams, connectionName);

            if (ds.Tables.Count > 0)
            {
                this.WriteDataTableToFile(
                    ds.Tables[0],
                    downloadFile,
                    hasHeaderRow,
                    delimiter,
                    textQualifier,
                    makeExtractWhenNoData,
                    shouldAppendToExistingFile);
                returnCount = ds.Tables[0].Rows.Count;
            }

            Log.LogInfo("Wrote [{0}] rows to output file.", returnCount);
            return returnCount;
        }
    }
}
