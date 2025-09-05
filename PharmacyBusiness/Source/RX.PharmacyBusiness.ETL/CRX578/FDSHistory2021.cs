namespace RX.PharmacyBusiness.ETL.CRX578
{
    using System;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX578", "Download Medicare Feeds.", "KBA00028461", "https://smartit.wegmans.com/smartit/app/#/knowledge/AGGAA5V0FYX55AOUCQU6OTGLZXEXZA")]
    public class FDSHistory2021 : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();

            Log.LogInfo("Creating data feed for vendor FDS 2021 historical data.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDSHistory202101.sql",
                @"%BATCH_ROOT%\CRX578\Output\FDS_202101_Wegmans.csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Creating data feed for vendor FDS 2021 historical data.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDSHistory202102.sql",
                @"%BATCH_ROOT%\CRX578\Output\FDS_202102_Wegmans.csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Creating data feed for vendor FDS 2021 historical data.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDSHistory202103.sql",
                @"%BATCH_ROOT%\CRX578\Output\FDS_202103_Wegmans.csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Creating data feed for vendor FDS 2021 historical data.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDSHistory202104.sql",
                @"%BATCH_ROOT%\CRX578\Output\FDS_202104_Wegmans.csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Creating data feed for vendor FDS 2021 historical data.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDSHistory202105.sql",
                @"%BATCH_ROOT%\CRX578\Output\FDS_202105_Wegmans.csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Creating data feed for vendor FDS 2021 historical data.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDSHistory202106.sql",
                @"%BATCH_ROOT%\CRX578\Output\FDS_202106_Wegmans.csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Creating data feed for vendor FDS 2021 historical data.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDSHistory202107.sql",
                @"%BATCH_ROOT%\CRX578\Output\FDS_202107_Wegmans.csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Creating data feed for vendor FDS 2021 historical data.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDSHistory202108.sql",
                @"%BATCH_ROOT%\CRX578\Output\FDS_202108_Wegmans.csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Creating data feed for vendor FDS 2021 historical data.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDSHistory202109.sql",
                @"%BATCH_ROOT%\CRX578\Output\FDS_202109_Wegmans.csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Creating data feed for vendor FDS 2021 historical data.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDSHistory202110.sql",
                @"%BATCH_ROOT%\CRX578\Output\FDS_202110_Wegmans.csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            Log.LogInfo("Creating data feed for vendor FDS 2021 historical data.");
            oracleHelper.DownloadQueryByRunDateToFile(
                150,
                @"%BATCH_ROOT%\CRX578\bin\FDSHistory202111.sql",
                @"%BATCH_ROOT%\CRX578\Output\FDS_202111_Wegmans.csv",
                runDate,
                true,
                "|",
                string.Empty,
                true,
                "ENTERPRISE_RX"
                );

            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
