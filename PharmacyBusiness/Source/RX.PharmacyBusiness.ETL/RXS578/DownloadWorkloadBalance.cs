namespace RX.PharmacyBusiness.ETL.RXS578
{
    using Oracle.ManagedDataAccess.Client;
    using RX.PharmacyBusiness.ETL.RXS578.Core;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.IO;

    [JobNotes("RXS578", "Download WLB Feeds.", "KBA00013306", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGGAA5V0FYX55APAT2RCOZWYQB5WUY")]
    public class DownloadWorkloadBalance : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            string targetRecipients = (this.Arguments["-TargetRecipients"] == null) ? "ALL" : this.Arguments["-TargetRecipients"].ToString();
            OracleHelper oracleHelper = new OracleHelper();
            SqlServerHelper sqlServerHelper = new SqlServerHelper();

            string inputLocation = @"%BATCH_ROOT%\RXS578\Input\";
            string outputLocation = @"%BATCH_ROOT%\RXS578\Output\";
            string archiveLocation = @"%BATCH_ROOT%\RXS578\Archive\";
            string rejectLocation = @"%BATCH_ROOT%\RXS578\Reject\";
            FileHelper fileHelper = new FileHelper(inputLocation, outputLocation, archiveLocation, rejectLocation);

            Log.LogInfo("Executing DownloadWorkloadBalance with RunDate [{0}], and TargetRecipients [{1}].", runDate, targetRecipients);
            
            if (targetRecipients == "ALL" || targetRecipients == "TENTEN")
            {
                Log.LogInfo("TENTEN feeds step 1 of 1: Output [Workflow_Steps] query.");
                oracleHelper.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\RXS578\bin\Super_Workflow_Steps_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\RXS578\Output\Workflow_Steps_" + runDate.ToString("yyyyMMdd") + ".txt",
                    runDate,
                    true,
                    "|",
                    string.Empty,
                    true,
                    "ENTERPRISE_RX"
                    );
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS578\Output\Workflow_Steps_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            if (targetRecipients == "ERXCERT")
            {
                Log.LogInfo("TENTEN feeds step 1 of 1: Output [Workflow_Steps] query.");
                oracleHelper.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\RXS578\bin\Super_Workflow_Steps_YYYYMMDD.sql",
                    @"%BATCH_ROOT%\RXS578\Output\ERXCERT_Workflow_Steps_" + runDate.ToString("yyyyMMdd") + ".txt",
                    runDate,
                    true,
                    "|",
                    string.Empty,
                    false,
                    "ENTERPRISE_RX"
                    );
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS578\Output\ERXCERT_Workflow_Steps_" + runDate.ToString("yyyyMMdd") + ".txt");
            }

            if (targetRecipients == "ALL" || targetRecipients == "LOGILE")
            {
                Log.LogInfo("LOGILE feeds step 1 of 4: [WorkflowStepLoad] procedure.");
                OracleParameter[] procedureParamsLoad = new OracleParameter[2];
                procedureParamsLoad[0] = new OracleParameter("start_date", OracleDbType.Date, ParameterDirection.Input);
                procedureParamsLoad[0].Value = runDate.AddDays(-1);
                procedureParamsLoad[1] = new OracleParameter("end_date", OracleDbType.Date, ParameterDirection.Input);
                procedureParamsLoad[1].Value = runDate.AddDays(-1);
                oracleHelper.CallNonQueryProcedure(
                    "ENTERPRISE_RX",
                    "WEGMANS.SCRIPTS_WORKLOAD_BALANCING.WorkflowStepLoad",
                    procedureParamsLoad);

                oracleHelper.DownloadQueryByRunDateToFile(
                    150,
                    @"%BATCH_ROOT%\RXS578\bin\TEMP_WORKFLOW_STEP.sql",
                    @"%BATCH_ROOT%\ArchiveForQA\InterfaceEngine\TEMP_WORKFLOW_STEP_" + runDate.ToString("yyyyMMdd") + ".txt",
                    runDate,
                    true,
                    "|",
                    string.Empty,
                    false,
                    "ENTERPRISE_RX"
                    );

                Log.LogInfo("LOGILE feeds step 2 of 4: [WorkflowStepClean] procedure.");
                OracleParameter[] procedureParamsClean = new OracleParameter[0];
                oracleHelper.CallNonQueryProcedure(
                    "ENTERPRISE_RX",
                    "WEGMANS.SCRIPTS_WORKLOAD_BALANCING.WorkflowStepClean",
                    procedureParamsClean);
                
                Log.LogInfo("LOGILE feeds step 3 of 4: [WorkflowIntervalSel] procedure.");
                OracleParameter[] procedureParamsIntervals = new OracleParameter[2];
                procedureParamsIntervals[0] = new OracleParameter("extract_date", OracleDbType.Date, ParameterDirection.Input);
                procedureParamsIntervals[0].Value = runDate.AddDays(-1);
                procedureParamsIntervals[1] = new OracleParameter("ref_cursor", OracleDbType.RefCursor, ParameterDirection.ReturnValue);
                oracleHelper.DownloadRefCursorFunctionToFile(
                    "ENTERPRISE_RX",
                    "WEGMANS.FORMAT_WLB_PKG.WorkflowIntervalSel",
                    procedureParamsIntervals,
                    @"%BATCH_ROOT%\RXS578\Output\WLB_Intervals_" + runDate.ToString("yyyyMMdd") + ".txt",
                    false,
                    ",",
                    string.Empty,
                    true);
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS578\Output\WLB_Intervals_" + runDate.ToString("yyyyMMdd") + ".txt");

                Log.LogInfo("LOGILE feeds step 4 of 4: [WorkflowTotalsSel] procedure.");
                OracleParameter[] procedureParamsTotals = new OracleParameter[2];
                procedureParamsTotals[0] = new OracleParameter("extract_date", OracleDbType.Date, ParameterDirection.Input);
                procedureParamsTotals[0].Value = runDate.AddDays(-1);
                procedureParamsTotals[1] = new OracleParameter("ref_cursor", OracleDbType.RefCursor, ParameterDirection.ReturnValue);
                oracleHelper.DownloadRefCursorFunctionToFile(
                    "ENTERPRISE_RX",
                    "WEGMANS.FORMAT_WLB_PKG.WorkflowTotalsSel",
                    procedureParamsTotals,
                    @"%BATCH_ROOT%\RXS578\Output\WLB_Totals_" + runDate.ToString("yyyyMMdd") + ".txt",
                    false,
                    ",",
                    string.Empty,
                    true);
                fileHelper.CopyFileToArchiveForQA(@"%BATCH_ROOT%\RXS578\Output\WLB_Totals_" + runDate.ToString("yyyyMMdd") + ".txt");

                Log.LogInfo("Get yesterday's activities in CPS database to be sent to LOGILE.");
                List<CpsRecord> cpsRecords = sqlServerHelper.DownloadQueryByRunDateToList<CpsRecord>(
                    @"%BATCH_ROOT%\RXS578\bin\McKesson_Azure_CPS.sql",
                    runDate,
                    "ENTERPRISE_RX_AZURE"
                    );

                Log.LogInfo("Get matrix of activities-codes-earned times to be sent to LOGILE.");
                DelimitedStreamReaderOptions logileMatrixFileOptions = new DelimitedStreamReaderOptions(
                    Constants.CharPipe,
                    new Nullable<char>(),
                    true,
                    false,
                    10,
                    1
                );
                List<LogileMatrixRecord> logileMatrixRecords = fileHelper.ReadFilesToList<LogileMatrixRecord>(
                    "logile_earned_time_matrix.txt",
                    logileMatrixFileOptions,
                    false);

                if (!logileMatrixRecords.Any())
                    throw new Exception("No records found in logile_earned_time_matrix.txt that was created by job RXS518.");

                Log.LogInfo("Merge CPS data with matrix data to generate LOGILE-specific records.");
                List<LogileIntervalRecord> intervals = new List<LogileIntervalRecord>();
                foreach (var cpsRecord in cpsRecords)
                {
                    foreach (var logileMatrixRecord in logileMatrixRecords)
                    {
                        if (logileMatrixRecord.EffectiveAsOfDate <= runDate &&
                            logileMatrixRecord.IneffectiveAsOfDate > runDate &&
                            cpsRecord.ProgramName == logileMatrixRecord.CPSProgramName &&
                            (cpsRecord.KeyDesc == logileMatrixRecord.CPSKeyDesc || string.IsNullOrEmpty(logileMatrixRecord.CPSKeyDesc)) &&
                            (cpsRecord.KeyValue == logileMatrixRecord.CPSKeyValue || string.IsNullOrEmpty(logileMatrixRecord.CPSKeyValue)) &&
                            (cpsRecord.StatusName == logileMatrixRecord.CPSStatusName || string.IsNullOrEmpty(logileMatrixRecord.CPSStatusName)))
                        {
                            intervals.Add(new LogileIntervalRecord(cpsRecord, logileMatrixRecord));
                        }
                    }
                }

                Log.LogInfo("Output intervals to existing file below that goes to Dev Store / LOGILE.");
                sqlServerHelper.WriteListToFile<LogileIntervalRecord>(
                    intervals.GroupBy(g => new
                    {
                        g.INTV_STR_NUM_OUT,
                        g.INTV_COM_CODE_OUT,
                        g.INTV_DESCRP_OUT,
                        g.INTV_TIME_OUT
                    })
                    .Select(s => new LogileIntervalRecord
                    {
                        INTV_STR_NUM_OUT = s.First().INTV_STR_NUM_OUT,
                        INTV_DEPT_NAME_OUT = s.First().INTV_DEPT_NAME_OUT,
                        INTV_DATE_OUT = s.First().INTV_DATE_OUT,
                        INTV_DEP_OUT = s.First().INTV_DEP_OUT,
                        INTV_DEP_DESC = s.First().INTV_DEP_DESC,
                        INTV_CAT_OUT = s.First().INTV_CAT_OUT,
                        INTV_CAT_DESC = s.First().INTV_CAT_DESC,
                        INTV_CLS_OUT = s.First().INTV_CLS_OUT,
                        INTV_CLS_DESC = s.First().INTV_CLS_DESC,
                        FILLER1_OUT = s.First().FILLER1_OUT,
                        FILLER1_DESC_OUT = s.First().FILLER1_DESC_OUT,
                        FILLER2_OUT = s.First().FILLER2_OUT,
                        FILLER2_DESC_OUT = s.First().FILLER2_DESC_OUT,
                        FILLER3_OUT = s.First().FILLER3_OUT,
                        FILLER3_DESC_OUT = s.First().FILLER3_DESC_OUT,
                        INTV_COM_CODE_OUT = s.First().INTV_COM_CODE_OUT,
                        INTV_DESCRP_OUT = s.First().INTV_DESCRP_OUT,
                        INTV_DSD_ITEM_OUT = s.First().INTV_DSD_ITEM_OUT,
                        INTV_DATA_TYPE_OUT = s.First().INTV_DATA_TYPE_OUT,
                        INTV_ITEM_PER_CASE_OUT = s.First().INTV_ITEM_PER_CASE_OUT,
                        INTV_UNITS_OUT = string.Format("{0:0000000.00}", s.Count()),
                        INTV_ITEM_SEL_UNIT_OUT = s.First().INTV_ITEM_SEL_UNIT_OUT,
                        INTV_WEIGHT_OUT = s.First().INTV_WEIGHT_OUT,
                        INTV_ITEM_PERLB_OUT = s.First().INTV_ITEM_PERLB_OUT,
                        INTV_ITEM_SHRINK_OUT = s.First().INTV_ITEM_SHRINK_OUT,
                        INTV_SALES_OUT = s.First().INTV_SALES_OUT,
                        INTV_TIME_OUT = s.First().INTV_TIME_OUT,
                        INTV_DRVR_NAME_OUT = s.First().INTV_DRVR_NAME_OUT
                    })
                    .OrderBy(o => o.INTV_STR_NUM_OUT)
                    .ThenBy(o => o.INTV_COM_CODE_OUT)
                    .ThenBy(o => o.INTV_TIME_OUT)
                    .ToList(),
                    @"%BATCH_ROOT%\RXS578\Output\WLB_Intervals_" + runDate.ToString("yyyyMMdd") + ".txt",
                    false,
                    ",",
                    string.Empty,
                    true,
                    true
                    );

                Log.LogInfo("Output totals to existing file below that goes to Dev Store / LOGILE.");
                sqlServerHelper.WriteListToFile<LogileTotalRecord>(
                    intervals.GroupBy(g => new
                    {
                        g.INTV_STR_NUM_OUT,
                        g.INTV_DEPT_NAME_OUT,
                        g.INTV_COM_CODE_OUT
                    })
                    .Select(s => new LogileTotalRecord
                    {
                        DATA_STR_NUM_ITEM_OUT = string.Format("{0,3}", s.First().INTV_STR_NUM_OUT),
                        DATA_DEPT_NAME_ITEM_OUT = string.Format("{0,-15}", s.First().INTV_DEPT_NAME_OUT),
                        DATA_COM_CODE_ITEM_OUT = string.Format("{0,-64}", s.First().INTV_COM_CODE_OUT),
                        DATA_DESCRP_ITEM_OUT = string.Format("{0,-35}", s.First().INTV_DESCRP_OUT),
                        DATA_DATE_ITEM_OUT = string.Format("{0,-10}", s.First().INTV_DATE_OUT),
                        DATA_DATA_TYPE_ITEM_OUT = string.Format("{0,-1}", "M"),
                        DATA_UNITS_ITEM_OUT = string.Format("{0:0000000.00}", s.Count()),
                        DATA_WEIGHT_ITEM_OUT = string.Format("{0:0000000.00}", 0)
                    })
                    .OrderBy(o => o.DATA_STR_NUM_ITEM_OUT)
                    .ThenBy(o => o.DATA_COM_CODE_ITEM_OUT)
                    .ToList(),
                    @"%BATCH_ROOT%\RXS578\Output\WLB_Totals_" + runDate.ToString("yyyyMMdd") + ".txt",
                    false,
                    ",",
                    string.Empty,
                    true,
                    true
                    );
            }

            Log.LogInfo("Finished running DownloadWorkloadBalance.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
