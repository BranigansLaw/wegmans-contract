namespace RX.PharmacyBusiness.ETL.RXS524
{
    using RX.PharmacyBusiness.ETL.RXS524.Core;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.IO;

    [JobNotes("RXS524", "Merge old and new Smart Order Points for pilot at store(s).", "KBA00013217", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0UJZAOEBOMZJNKTFDXPOJ")]
    public class MergeOrderPointsForPilotStores : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            DateTime runDate = (this.Arguments["-RunDate"] == null) ? DateTime.Now.Date : DateTime.Parse(this.Arguments["-RunDate"].ToString());
            string inputLocation = @"%BATCH_ROOT%\RXS524\Input\";
            string outputLocation = @"%BATCH_ROOT%\RXS524\Output\";
            string archiveLocation = @"%BATCH_ROOT%\RXS524\Archive\";
            string rejectLocation = @"%BATCH_ROOT%\RXS524\Reject\";
            FileHelper fileHelper = new FileHelper(inputLocation, outputLocation, archiveLocation, rejectLocation);
            EmailHelper emailHelper;
            string emailSubject = "List of stores piloting Smart Order Points";
            string emailFrom = @"PharmacyIT@wegmans.com";
            string emailTo = @"Michael.Johnson2@wegmans.com,PharmacyIT@wegmans.com";
            string emailBodyTemplate = string.Empty;

            Log.LogInfo("Executing MergeOrderPointsForPilotStores with RunDate [{0}].", runDate);
            List<SmartOrderPointRecord> oldRecords = new List<SmartOrderPointRecord>();
            List<SmartOrderPointRecord> newRecords = new List<SmartOrderPointRecord>();
            List<SmartOrderPointRecord> mergedRecords = new List<SmartOrderPointRecord>();

            Log.LogInfo("Read OLD and NEW files into a list of Order Point records.");
            DelimitedStreamReaderOptions intputFileOptions = new DelimitedStreamReaderOptions(
                Constants.CharSemiColon,
                new Nullable<char>(),
                false,
                false,
                9,
                0
            );

            oldRecords = fileHelper.ReadFilesToList<SmartOrderPointRecord>(
                "ERxDataFeed_" + runDate.ToString("yyyyMMdd") + "_120000",
                intputFileOptions,
                true);

            newRecords = fileHelper.ReadFilesToList<SmartOrderPointRecord>(
                "ERxDataFeed_" + runDate.ToString("yyyyMMdd") + "_120000.txt",
                intputFileOptions,
                true);

            List<string> pilotStores = newRecords
                .OrderBy(o => o.left_zero_padded_store)
                .GroupBy(g => new
                {
                    g.left_zero_padded_store
                })
                .Select(s => s.First().left_zero_padded_store)
                .ToList();

            mergedRecords.AddRange(newRecords);
            mergedRecords.AddRange(oldRecords
                    .Where(r => !pilotStores.Contains(r.left_zero_padded_store))
                    .OrderBy(o => o.left_zero_padded_store)
                    .Select(s => s)
                    .ToList());

            Log.LogInfo("Write/append to file the list of stores that got an email so that we do not spam them too much in one day from multiple runs.");
            fileHelper.WriteListToFile<SmartOrderPointRecord>(
                mergedRecords,
                outputLocation + "ERxDataFeed_" + runDate.ToString("yyyyMMdd") + "_120000",
                false,
                Constants.CharSemiColon.ToString(),
                string.Empty,
                false,
                false
                );

            emailBodyTemplate = string.Format(@"<!DOCTYPE html><html><head></head><body><b>Hello RxBST and Dev Pharmacy Teams,</b><br/>
<br/>
This email is to inform you of the temporary piloting of Smart Order Points at the following stores on [{0}]:<br/><ol>", runDate.ToShortDateString());

            foreach (var pilotStore in pilotStores)
            {
                emailBodyTemplate += string.Format("<li>Store {0}</li>", pilotStore);
            }

            if (pilotStores.Count == 0)
                emailBodyTemplate += "<li>No stores in pilot at this time</li>";

            emailBodyTemplate += @"</ol><br/>
<b><u>To rollback piloting of new Order Points:</u></b> Edit the code library in 1010data [wegmans.shared.libraries.smartorderpoints.xml] 
in block [generate_order_points] by adding a filter out statement, such as &lt;sel value=&quot;1=0&quot;/&gt; that would exclude all new records.<br/>
<br/>
<b><u>To add new stores to the pilot:</u></b> Simply edit that same code library in the same block near the beginning where the filter list of stores is hard-coded to allow for rapid edits. 
As of 2/7/2022, this filter reads &lt;setv name=&quot;selected_store_list&quot; value =&quot;25&quot;/&gt;.<br/>
<br/>
<b><u>To end this pilot and commit to just new Smart Order Points:</u></b> Eliminate the merge processing within the TPS Plan executed by Control-M batch job RXS524, and just SFTP the new file in its entirety.<br/>
<br/>
For more information, see KBA00012983 here <a href='https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0UJZAOEBOMZJNKTFDXPOJ'>here</a>.<br/>
</body></html>";
            emailHelper = new EmailHelper(
                emailFrom,
                emailTo,
                string.Empty,
                string.Empty);

            try
            {
                emailHelper.SendEmail(
                    emailSubject,
                    emailBodyTemplate,
                    System.Net.Mail.MailPriority.Low,
                    true);
            }
            catch (Exception ex)
            {
                Log.LogWarn($"EXCEPTION while sending low priority (not critical) email to RxBST and Dev Pharmacy Teams {ex.Message}");
            }

            Log.LogInfo("Finished running MergeOrderPointsForPilotStores.");
            result = returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
