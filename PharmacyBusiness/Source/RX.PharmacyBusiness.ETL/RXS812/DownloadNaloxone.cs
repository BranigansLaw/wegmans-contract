
namespace RX.PharmacyBusiness.ETL.RXS812
{
    using RX.PharmacyBusiness.ETL.CRX540.Core;
    using RX.PharmacyBusiness.ETL.RXS812.Core;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS812", "Download Naloxone", "KBA00013330", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAODGJOF29XHYHTA8K")]
    public class DownloadNaloxone : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            OracleHelper oracleHelper = new OracleHelper();
            EmailHelper emailHelper = new EmailHelper(
                ConfigurationManager.AppSettings["RXS812.EmailFrom"].ToString(),
                ConfigurationManager.AppSettings["RXS812.EmailToCSV"].ToString(),
                ConfigurationManager.AppSettings["RXS812.EmailCcCSV"].ToString(),
                ConfigurationManager.AppSettings["RXS812.EmailBccCSV"].ToString());
            FileHelper fileHelper = new FileHelper(
                @"%BATCH_ROOT%\RXS812\Input\",
                @"%BATCH_ROOT%\RXS812\Output\",
                @"%BATCH_ROOT%\RXS812\Archive\",
                @"%BATCH_ROOT%\RXS812\Reject\");
            string emailSubject = "The Wegmans Naloxone Report for last quarter is now available via batch job RXS812.";
            string emailBodyTemplate = @"<!DOCTYPE html><html><head></head><body><b>Hello Naloxone Report Users,</b><br />";
            List<string> attachments = new List<string>();

            Log.LogInfo("Executing DownloadNaloxone with RunDate [{0}].", runDate);

            GetLastQuarterDateRange(runDate, out DateTime startDate, out DateTime endDate);
            Log.LogInfo("Last quarter for RunDate [{0}] is from [{1}] to [{2}].", runDate, startDate, endDate);
            emailBodyTemplate += string.Format("<br />Job RXS812 just completed running the last quarter Naloxone report for report date [{0}] which is from [{1}] to [{2}].", runDate, startDate, endDate);

            List<ReportRecord> reportRecords = new List<ReportRecord>();

            //Add custom header row per requirements given by Kristin Ashby on 10/31/2023, including the extra blank space in ReportingName.
            //Still make an output file even if zero rows of data.
            reportRecords.Add(new ReportRecord
            {
                ReportingName = "Name of person providing  reports",
                ReportingPhone = "Telephone number of person providing report",
                ReportingEmail = "Email of person providing directory information",
                PharmacyName = "Pharmacy name (how pharmacy is generally known in the community)",
                PharmacyNbr = "Store Number",
                ZipCode = "ZipCode",
                PharmacyNPI = "Pharmacy NPI",
                PrescriberNPI = "Dispensing Encounter: Linked NPI (provider)",
                DrugName = "Please include each  Dispensing Encounter: Formulation name",
                DrugNDC = "Dispensing per Encounter: Formulation NDC",
                DrugQty = "Dispensing Encounter: Count of NDC dispensed per encounter"
            });

            List<ReportRecord> dataRecords = oracleHelper.DownloadQueryToList<ReportRecord>(
                150,
                @"%BATCH_ROOT%\RXS812\bin\McKesson_DW_Naloxone.sql",
                new
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    PrescriberNpiCSV = ConfigurationManager.AppSettings["RXS812.SQL.PrescriberNpiCSV"].ToString(),
                    NdcCSV = ConfigurationManager.AppSettings["RXS812.SQL.NdcCSV"].ToString(),
                    ReportingName = ConfigurationManager.AppSettings["RXS812.SQL.ReportingName"].ToString(),
                    ReportingPhone = ConfigurationManager.AppSettings["RXS812.SQL.ReportingPhone"].ToString(),
                    ReportingEmail = ConfigurationManager.AppSettings["RXS812.SQL.ReportingEmail"].ToString()
                },
                "ENTERPRISE_RX"
                );

            reportRecords.AddRange(dataRecords);
            emailBodyTemplate += string.Format(" There are <b>[{0}]</b> rows in the attachment for last quarter.", reportRecords.Count);

            emailBodyTemplate += @" <br/><br/>
<b>Here are the filters used in this report (contact Dev Pharmacy Team to modify these lists):</b><br/>
<table style='border:1px solid black;' cellpadding='0' cellspacing='0'><tr><th>Prescriber Npi</th></tr>";

            foreach (var prescriberNpi in ConfigurationManager.AppSettings["RXS812.SQL.PrescriberNpiCSV"].ToString().Split(','))
            {
                emailBodyTemplate += string.Format(@"<tr><td style='border-top:1px solid black;'>{0}</td></tr>", prescriberNpi);
            }

            emailBodyTemplate += @" </table><br/>
<table style='border:1px solid black;' cellpadding='0' cellspacing='0'><tr><th>Naloxone NDC</th></tr>";

            foreach (var ndc in ConfigurationManager.AppSettings["RXS812.SQL.NdcCSV"].ToString().Split(','))
            {
                emailBodyTemplate += string.Format(@"<tr><td style='border-top:1px solid black;'>{0}</td></tr>", ndc);
            }

            emailBodyTemplate += @" </table>
<br />
<br />Thanks,
<br /><i>Wegmans Pharmacy IT Development Team</i><br />
</body>
</html>";
            // NOTE: The following has been commented out as of 11/1/2023 while the Tech Wintel Team reviews options for creating XLSX files on the batch server.
            //       Until then, we will make a CSV file.
            //       We can switch back and forth via [bool createXlsxFile] on the next line.
            bool createXlsxFile = false;
            string excelFileNameAndPath = string.Empty;
            if (createXlsxFile)
            { 
                Log.LogInfo("Next, will write [{0}] records to an XLSX file.", dataRecords.Count);
                excelFileNameAndPath = fileHelper.WriteListToExcel<ReportRecord>(
                    reportRecords,
                    string.Format("QuarterlyNaloxone_from_{0}_to_{1}.xlsx", startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd")),
                    "Naloxone_Report",
                    false,
                    true);
            }
            else
            {
                Log.LogInfo("Next, will write [{0}] records to a CSV file.", dataRecords.Count);
                excelFileNameAndPath = string.Format(@"%BATCH_ROOT%\RXS812\Output\QuarterlyNaloxone_from_{0}_to_{1}.csv", startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"));
                fileHelper.WriteListToFile<ReportRecord>(
                    reportRecords,
                    excelFileNameAndPath,
                    false,
                    ",",
                    string.Empty,
                    true,
                    false,
                    true);
            }
            attachments.Add(excelFileNameAndPath);

            Log.LogInfo("Next, will email with file attached.", dataRecords.Count);
            emailHelper.SendEmail(
                emailSubject,
                emailBodyTemplate,
                attachments,
                string.Empty,
                System.Net.Mail.MailPriority.Normal,
                true);

            Log.LogInfo("Finished running DownloadNaloxone.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        public static void GetLastQuarterDateRange(DateTime runDate, out DateTime startDate, out DateTime endDate)
        {
            int lastQuarterStartMonth = 0;
            int lastQuarterEndMonth = 0;
            int lastQuarterStartYear = runDate.Year;
            int lastQuarterEndYear = runDate.Year;

            if (runDate.Month >= 1 && runDate.Month <= 3)
            {
                lastQuarterStartMonth = 10;
                lastQuarterEndMonth = 1;
                lastQuarterStartYear = runDate.Year - 1;
            } else if (runDate.Month >= 4 && runDate.Month <= 6)
            {
                lastQuarterStartMonth = 1;
                lastQuarterEndMonth = 4;
            } else if (runDate.Month >= 7 && runDate.Month <= 9)
            {
                lastQuarterStartMonth = 4;
                lastQuarterEndMonth = 7;
            } else if (runDate.Month >= 10 && runDate.Month <= 12)
            {
                lastQuarterStartMonth = 7;
                lastQuarterEndMonth = 10;
            }

            startDate = new DateTime(lastQuarterStartYear, lastQuarterStartMonth, 1);
            endDate = new DateTime(lastQuarterEndYear, lastQuarterEndMonth, 1);
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}