using System.Diagnostics;
using XXXDeveloperTools.CompareDerivedData.DataModels;
using XXXDeveloperTools.CompareDerivedData.Helper;

namespace XXXDeveloperTools.CompareOutputFiles
{
    public class CompareDerivedDataRunner
    {
        public string NewJobName { get; }
        public string OldJobName { get; set; }
        public DateTime RunDate { get; }

        public CompareDerivedDataRunner(string newJobName, DateTime runDate)
        {
            NewJobName = newJobName;
            OldJobName = string.Empty;
            RunDate = runDate;
        }

        public int Run()
        {
            int returnCode = 0;

            switch (NewJobName)
            {
                case "DEMO01":
                    OldJobName = "OLD001";
                    CompareDerivedDataCollectionsHelperImp<Demo01_SalesRow> demo01Helper = new CompareDerivedDataCollectionsHelperImp<Demo01_SalesRow>(
                        new CompareDerivedDataSpecifications
                        {
                            NewFilePath = "../../../../YYYQATools.CompareOutputFiles/DemoData/NewFiles/",
                            OldFilePath = "../../../../YYYQATools.CompareOutputFiles/DemoData/OldFiles/",
                            FileName = $"Demo01_{RunDate.ToString("yyyyMMdd")}.csv",
                            RowDelimiter = "\n",
                            ColumnDelimiter = ",",
                            HasHeaderRow = true
                        });
                    returnCode = demo01Helper.CompareCollections();
                    break;
                case "INN601":
                    OldJobName = "CRX515";
                    LaunchNewInnovationCommandLine($"export-omnisys-claim-from-mckesson-dw --run-for {RunDate.ToString("MM/dd/yyyy")}");
                    LaunchOldInterfaceEngineCommandLine($"-a RX.PharmacyBusiness.ETL.dll -i DownloadOmnisysClaim -RunDate {RunDate.ToString("MM/dd/yyyy")}");

                    CompareDerivedDataCollectionsHelperImp<OmnisysClaimRow> omnisysClaimHelper = new CompareDerivedDataCollectionsHelperImp<OmnisysClaimRow>(
                        new CompareDerivedDataSpecifications
                        {
                            NewFilePath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\DataFileExports"),
                            OldFilePath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX515\Output"),
                            FileName = $"Wegmans_SOLDDATE_REQ_{RunDate.AddDays(-1).ToString("yyyyMMdd")}.TXT",
                            RowDelimiter = "\r\n",
                            ColumnDelimiter = "\r",
                            HasHeaderRow = true
                        });
                    returnCode = omnisysClaimHelper.CompareCollections();
                    GetResultsReport(omnisysClaimHelper.Specs.FileName, omnisysClaimHelper.GetCompareSummaryReport());
                    break;
                case "INN701":
                    //NOTE: Most jobs will have only one file to compare. Job INN701 "Turnaround Time Reports is a rare job with nine reports.
                    OldJobName = "CRX575";

                    //File 1 of 9.
                    CompareDerivedDataCollectionsHelperImp<TurnaroundTimeRawDataRow> rawDataHelper = new CompareDerivedDataCollectionsHelperImp<TurnaroundTimeRawDataRow>(
                        new CompareDerivedDataSpecifications
                        {
                            NewFilePath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\DataFileExports"),
                            OldFilePath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX575\Output"),
                            FileName = $"TAT_Excellus_Raw_Data_Prior_Week_{RunDate.ToString("yyyyMMdd")}.csv",
                            RowDelimiter = "\n",
                            ColumnDelimiter = ",",
                            HasHeaderRow = true
                        });
                    rawDataHelper.CompareCollections();
                    GetResultsReport(rawDataHelper.Specs.FileName, rawDataHelper.GetCompareSummaryReport(10));

                    //File 2 of 9.
                    rawDataHelper.ResetComparisonSpecs($"TAT_IHA_Raw_Data_Prior_Week_{RunDate.ToString("yyyyMMdd")}.csv");
                    rawDataHelper.CompareCollections();
                    GetResultsReport(rawDataHelper.Specs.FileName, rawDataHelper.GetCompareSummaryReport(10));

                    //File 3 of 9.
                    rawDataHelper.ResetComparisonSpecs($"TAT_Specialty_Raw_Data_Prior_Week_{RunDate.ToString("yyyyMMdd")}.csv");
                    rawDataHelper.CompareCollections();
                    GetResultsReport(rawDataHelper.Specs.FileName, rawDataHelper.GetCompareSummaryReport(10));

                    //File 4 of 9.
                    CompareDerivedDataCollectionsHelperImp<TurnaroundTimeSummaryRow> summaryReportHelper = new CompareDerivedDataCollectionsHelperImp<TurnaroundTimeSummaryRow>(
                        new CompareDerivedDataSpecifications
                        {
                            NewFilePath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\DataFileExports"),
                            OldFilePath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX575\Output"),
                            FileName = $"TAT_Excellus_Prior_Week_{RunDate.ToString("yyyyMMdd")}.csv",
                            RowDelimiter = "\n",
                            ColumnDelimiter = ",",
                            HasHeaderRow = true
                        });
                    summaryReportHelper.CompareCollections();
                    GetResultsReport(summaryReportHelper.Specs.FileName, summaryReportHelper.GetCompareSummaryReport(10));

                    //File 5 of 9.
                    summaryReportHelper.ResetComparisonSpecs($"TAT_IHA_Prior_Week_{RunDate.ToString("yyyyMMdd")}.csv");
                    summaryReportHelper.CompareCollections();
                    GetResultsReport(summaryReportHelper.Specs.FileName, summaryReportHelper.GetCompareSummaryReport(10));

                    //File 6 of 9.
                    summaryReportHelper.ResetComparisonSpecs($"TAT_Specialty_Prior_Week_{RunDate.ToString("yyyyMMdd")}.csv");
                    summaryReportHelper.CompareCollections();
                    GetResultsReport(summaryReportHelper.Specs.FileName, summaryReportHelper.GetCompareSummaryReport(10));

                    //File 7 of 9.
                    CompareDerivedDataCollectionsHelperImp<TurnaroundTimeSummaryMaxRxRow> summaryMaxRxReportHelper = new CompareDerivedDataCollectionsHelperImp<TurnaroundTimeSummaryMaxRxRow>(
                        new CompareDerivedDataSpecifications
                        {
                            NewFilePath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\DataFileExports"),
                            OldFilePath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\CRX575\Output"),
                            FileName = $"TAT_Excellus_MaxRx_Prior_Week_{RunDate.ToString("yyyyMMdd")}.csv",
                            RowDelimiter = "\n",
                            ColumnDelimiter = ",",
                            HasHeaderRow = true
                        });
                    summaryMaxRxReportHelper.CompareCollections();
                    GetResultsReport(summaryMaxRxReportHelper.Specs.FileName, summaryMaxRxReportHelper.GetCompareSummaryReport(10));

                    //File 8 of 9.
                    summaryMaxRxReportHelper.ResetComparisonSpecs($"TAT_IHA_MaxRx_Prior_Week_{RunDate.ToString("yyyyMMdd")}.csv");
                    summaryMaxRxReportHelper.CompareCollections();
                    GetResultsReport(summaryMaxRxReportHelper.Specs.FileName, summaryMaxRxReportHelper.GetCompareSummaryReport(10));

                    //File 9 of 9.
                    summaryMaxRxReportHelper.ResetComparisonSpecs($"TAT_Specialty_YTD_{RunDate.ToString("yyyyMMdd")}.csv");
                    summaryMaxRxReportHelper.CompareCollections();
                    GetResultsReport(summaryMaxRxReportHelper.Specs.FileName, summaryMaxRxReportHelper.GetCompareSummaryReport(10));

                    break;
                default:
                    //Note: This would only be a failure point during initial setup of a new job.
                    throw new ArgumentException("Invalid new job name.");
            }

            return returnCode;
        }

        private string GetResultsReport(string fileName, string summaryReport)
        {
            List<string> resultsReport = new List<string>();
            resultsReport.Add("=========================================================================");
            resultsReport.Add($"File Comparison Results Summary Report created on {DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss")}");
            resultsReport.Add($"FileName [{fileName}]");
            resultsReport.Add("_________________________________________________________________________");
            resultsReport.Add(summaryReport);
            resultsReport.Add(string.Empty);
            resultsReport.Add("_________________________________________________________________________");

            string resultsFileName = $"CompareResults_{NewJobName}_to_{OldJobName}_ForRunDate_{RunDate.ToString("yyyyMMdd")}.txt";

            using (StreamWriter writerOutputData = new StreamWriter(Path.Combine(Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\DataFileExports"), resultsFileName), true))
            {
                foreach (string line in resultsReport)
                {
                    writerOutputData.WriteLine(line);
                }
            }

            return string.Join(Environment.NewLine, resultsReport) + Environment.NewLine;
        }

        private void LaunchNewInnovationCommandLine(
            string arguments = "export-omnisys-claim-from-mckesson-dw --run-for 07/16/2024")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%/Development/ae7c5bac-8baf-400e-890c-00724466277d/Innovation/INN.JobRunner/INN.JobRunner.exe");
            startInfo.Arguments = arguments;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            using (Process exeProcess = Process.Start(startInfo))
            {
                if (exeProcess == null)
                {
                    throw new Exception("Could not start the process.");
                }
                exeProcess.WaitForExit();
            }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        }

        private void LaunchOldInterfaceEngineCommandLine(
            string arguments = "-a RX.PharmacyBusiness.ETL.dll -i DownloadOmnisysClaim -RunDate 07/16/2024")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.CreateNoWindow = false;
            startInfo.UseShellExecute = false;
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.FileName = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\PharmacyBusiness\bin\Wegmans.InterfaceEngine.exe");
            startInfo.Arguments = arguments;

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
            using (Process exeProcess = Process.Start(startInfo))
            {
                if (exeProcess == null)
                {
                    throw new Exception("Could not start the process.");
                }
                exeProcess.WaitForExit();
            }
#pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.
        }
    }
}
