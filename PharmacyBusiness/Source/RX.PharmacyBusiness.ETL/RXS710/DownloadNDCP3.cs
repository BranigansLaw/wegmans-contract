namespace RX.PharmacyBusiness.ETL.RXS710
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.IO;
    using Wegmans.PharmacyLibrary.Logging;

    [JobNotes("RXS710", "Download NDCP3 data.", "KBA00013315", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAODE6FI1F3CMMKCQ4")]
    public class DownloadNDCP3 : ETLBase
    {
        private IFileManager FileManager { get; set; }
        private ReturnCode returnCode = new ReturnCode();
        private readonly string extractFilePath = Environment.ExpandEnvironmentVariables(@"%BATCH_ROOT%\RXS710\Output\");
        private const string extractFileName = "wegndcsortCombined.txt";
        private const string ndcp_198 = "5807574";

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        protected override void Execute(out object result)
        {
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date.StartOfWeek(DayOfWeek.Sunday) : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            this.FileManager = this.FileManager ?? new FileManager();
            OracleHelper oracleHelper = new OracleHelper();

            Log.LogInfo("Begin NDCP3 header from McKesson Oracle DW.");
            List<NDCP3HeaderIn> ndcp3HeaderInList = oracleHelper.DownloadQueryByRunDateToList<NDCP3HeaderIn>(
                150,
                @"%BATCH_ROOT%\RXS710\bin\SelectNDCP3Header.sql",
                runDate,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Begin NDCP3 detail from McKesson Oracle DW.");
            List<NDCP3DetailIn> ndcp3DetailInList = oracleHelper.DownloadQueryByRunDateToList<NDCP3DetailIn>(
                150,
                @"%BATCH_ROOT%\RXS710\bin\SelectNDCP3Detail.sql",
                runDate,
                "ENTERPRISE_RX"
                );

            int recordsFound = 0;
            int recordsWritten = 0;
            int exceptions = 0;
            int detailRecordCount = 0;

            using (var streamWriter = new StreamWriter(Path.Combine(extractFilePath, extractFileName), false))
            {
                try
                {
                    recordsFound = ndcp3HeaderInList.Count;
                    var sortedHeaderRecords = ndcp3HeaderInList
                        .Where(r => r.NcpdpNumber != ndcp_198)
                        .OrderBy(s1 => s1.NcpdpNumber)
                        .ThenBy(s2 => s2.PharmacyAddress1);
                    foreach (var headerInRecord in sortedHeaderRecords)
                    {
                        detailRecordCount = 0;
                        try
                        {
                            var headerOutRecord = new NDCP3HeaderOut(headerInRecord);
                            streamWriter.WriteLine(headerOutRecord.WriteFixedWidth());
                            recordsWritten++;
                            var detailInRecords = ndcp3DetailInList
                                .Where(x => x.NcpdpNumber == headerInRecord.NcpdpNumber)
                                .OrderBy(s1 => s1.RxNumber)
                                .ThenBy(s2 => s2.RefillNumber)
                                .ThenBy(s3 => s3.FillStateChangeTimestamp);
                            recordsFound += detailInRecords.Count();
                            foreach (var detailInRecord in detailInRecords)
                            {
                                detailRecordCount++;
                                var detailOutRecord = new NDCP3DetailOut(detailInRecord);
                                streamWriter.WriteLine(detailOutRecord.WriteFixedWidth());
                                recordsWritten++;
                            }
                        }
                        catch (Exception ex)
                        {
                            exceptions++;
                            Log.LogError($"An error has occurred for NCPDP [{headerInRecord.NcpdpNumber}], detail record [{detailRecordCount}]. ", ex);
                            this.returnCode.IsFailure = true;
                        }
                    }
                }
                finally
                {
                    streamWriter.Flush();
                }
            }

            Log.LogInfo($"Completed NDCP3 data feed with [{recordsFound}] records found, [{recordsWritten}] records written, and [{exceptions}] exceptions");
            result = this.returnCode.IsSuccess ? 0 : 1;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
