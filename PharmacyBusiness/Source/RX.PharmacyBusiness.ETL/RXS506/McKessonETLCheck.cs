namespace RX.PharmacyBusiness.ETL.RXS506
{
    using RX.PharmacyBusiness.ETL.CRX540.Core;
    using RX.PharmacyBusiness.ETL.RXS506.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.IO;

    [JobNotes("RXS506", "Check McKesson ETL.", "KBA00039018", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGGAA5V0FZQ1XAQDSNNYQCUVXQZY9E")]
    public class McKessonETLCheck : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = DateTime.Now.Date; //NOTE: Control-M kicks off this job at 1 AM daily.
            string checkDatabase = (this.Arguments["-Database"] == null) ? "ALL" : this.Arguments["-Database"].ToString();
            OracleHelper oracleHelper = new OracleHelper();
            SqlServerHelper sqlServerHelper = new SqlServerHelper();
            FileHelper fileHelper = new FileHelper(
                @"%BATCH_ROOT%\CRX542\Input\",
                @"%BATCH_ROOT%\CRX542\Output\",
                @"%BATCH_ROOT%\CRX542\Archive\",
                @"%BATCH_ROOT%\CRX542\Reject\");

            if (checkDatabase == "ALL" || checkDatabase == "ORACLE_DW")
            { 
                Log.LogInfo("Begin query to McKesson Oracle DW ETL_CHECK table to get raw data of most recent record - merely to enhance this logging.");
                List<string> loggingInfoOracleDW = oracleHelper.DownloadQueryByRunDateToList<string>(
                    150,
                    @"%BATCH_ROOT%\RXS506\bin\ETLInfoForLogging_Oracle_DW.sql",
                    runDate,
                    "ENTERPRISE_RX"
                    );
                using (StreamWriter sw = File.CreateText(Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\ArchiveForQA\InterfaceEngine\ETLInfoForLogging_Oracle_DW_" + runDate.ToString("yyyyMMdd") + ".txt")))
                {
                    foreach (string logRecord in loggingInfoOracleDW)
                    {
                        sw.WriteLine(logRecord);
                    }
                }

                foreach (string logRecord in loggingInfoOracleDW)
                {
                    Log.LogInfo(logRecord);
                }

                Log.LogInfo("Begin query to McKesson Oracle DW ETL_CHECK table looking for a record with a START_TIME of at least 11 PM previous day, the RUN_TYPE='AM'/'PM' does not matter, an END_TIME of sometime today before 3 PM (the 'PM' ETL starts at 4 PM-ish), and has STATUS='C' (C=Complete, R=Running, A=Aborted) - this is the critical business logic that determines the success or failure of this job.");
                List<DateTime> etlRunsOracleDW = oracleHelper.DownloadQueryByRunDateToList<DateTime>(
                    150,
                    @"%BATCH_ROOT%\RXS506\bin\ETLCheck_Oracle_DW.sql",
                    runDate,
                    "ENTERPRISE_RX"
                    );
                using (StreamWriter sw = File.CreateText(Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\ArchiveForQA\InterfaceEngine\ETLCheck_Oracle_DW_" + runDate.ToString("yyyyMMdd") + ".txt")))
                {
                    foreach (DateTime etlRun in etlRunsOracleDW)
                    {
                        sw.WriteLine(etlRun.ToString("MM/dd/yyyy HH:mm:ss"));
                    }
                }

                if (etlRunsOracleDW.Count == 1)
                {
                    Log.LogInfo("McKesson Oracle DW ETL completed at [{0:MMM dd HH:mm:ss}].", etlRunsOracleDW[0]);
                }
                else
                { 
                    Log.LogInfo("McKesson Oracle DW ETL has NOT yet completed as of [{0:MMM dd HH:mm:ss}].", DateTime.Now);
                    returnCode.IsFailure = true;
                }
            }

            if (checkDatabase == "ALL" || checkDatabase == "AZURE_CPS")
            {
                Log.LogInfo("Begin query to McKesson Azure CPS to get raw data of every record in the DW_Last_Load_Watermark table - merely to enhance this logging.");
                List<string> loggingInfoAzureCPS = sqlServerHelper.DownloadQueryByRunDateToList<string>(
                    @"%BATCH_ROOT%\RXS506\bin\ETLInfoForLogging_Azure_CPS.sql",
                    runDate,
                    "ENTERPRISE_RX_AZURE"
                    );
                foreach (string logRecord in loggingInfoAzureCPS)
                {
                    Log.LogInfo(logRecord);
                }

                Log.LogInfo("Begin query to McKesson Azure CPS looking for all records to have a LAST_LOAD_DATE of sometime today (typically between 00:30 and 03:00) - this is the critical business logic that determines the success or failure of this job.");
                List<DateTime> etlRunsAzureCPS = sqlServerHelper.DownloadQueryByRunDateToList<DateTime>(
                    @"%BATCH_ROOT%\RXS506\bin\ETLCheck_Azure_CPS.sql",
                    runDate,
                    "ENTERPRISE_RX_AZURE"
                    );

                if (etlRunsAzureCPS.Count == 1)
                {
                    Log.LogInfo("McKesson Azure CPS ETL completed at [{0:MMM dd HH:mm:ss}].", etlRunsAzureCPS[0]);
                }
                else
                {
                    Log.LogInfo("McKesson Azure CPS ETL has NOT yet completed as of [{0:MMM dd HH:mm:ss}].", DateTime.Now);
                    returnCode.IsFailure = true;
                }
            }

            if (checkDatabase == "ALL" || checkDatabase == "CPS_RECORD_COUNTS")
            {
                Log.LogInfo("Read from previous outputted file and compare to new results, if different throw an error.");
                
                DelimitedStreamReaderOptions rowCountsFileOptions = new DelimitedStreamReaderOptions(
                    Constants.CharComma,
                    new Nullable<char>(),
                    true,
                    false,
                    2,
                    1
                );
                List<RowCountRecord> previousRowCounts = fileHelper.ReadFilesToList<RowCountRecord>(
                    "RowCounts_Azure_CPS_" + runDate.ToString("yyyyMMdd") + ".csv",
                    rowCountsFileOptions,
                    false);

                List<RowCountRecord> currentRowCounts = sqlServerHelper.DownloadQueryByRunDateToList<RowCountRecord>(
                    @"%BATCH_ROOT%\RXS506\bin\RowCounts_Azure_CPS.sql",
                    runDate,
                    "ENTERPRISE_RX_AZURE"
                    );

                foreach(RowCountRecord previousRowCount in previousRowCounts)
                {
                    foreach (RowCountRecord currentRowCount in currentRowCounts)
                    {
                        if (previousRowCount.TableNane == currentRowCount.TableNane)
                        {
                            if (previousRowCount.RowCount != currentRowCount.RowCount)
                            {
                                Log.LogWarn("Table [{0}] just changed row counts, which are listed in the Output folder.", previousRowCount.TableNane);

                                if (returnCode.IsFailure == false)
                                { 
                                    sqlServerHelper.WriteListToFile<RowCountRecord>(
                                        previousRowCounts,
                                        @"%BATCH_ROOT%\RXS506\Output\PreviousRowCounts_Azure_CPS_" + DateTime.Now.AddMinutes(-5).ToString("yyyyMMdd_HH24MISS") + ".csv",
                                        true,
                                        ",",
                                        string.Empty,
                                        true);

                                    sqlServerHelper.WriteListToFile<RowCountRecord>(
                                        currentRowCounts,
                                        @"%BATCH_ROOT%\RXS506\Output\CurrentRowCounts_Azure_CPS_" + DateTime.Now.ToString("yyyyMMdd_HH24MISS") + ".csv",
                                        true,
                                        ",",
                                        string.Empty,
                                        true);
                                }

                                returnCode.IsFailure = true;
                            }
                        }
                    }
                }
            }

            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
