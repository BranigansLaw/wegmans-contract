namespace RX.PharmacyBusiness.ETL.RXS516
{
    using RX.PharmacyBusiness.ETL.RXS516.Core;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Globalization;
    using System.Linq;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;
    using Wegmans.PharmacyLibrary.IO;

    [JobNotes("RXS516", "POS Exceptions API Calls.", "KBA00012983", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAODE3591DJOR495JX")]
    public class POSExceptionAPICalls : ETLBase
    {
        private DateTime runDate;
        private FileHelper fileHelper;
        private string inputLocation;
        private string outputLocation;
        private string archiveLocation;
        private string rejectLocation;
        private string emailExclusionsCsvFileName;
        private string apiCallFailuresCsvFileName;
        private List<NotificationsSent> notificationsSentListThisRun;
        private List<APICallFailures> apiCallFailures;
        private ReturnCode returnCode;

        protected override void Execute(out object result)
        {
            this.returnCode = new ReturnCode();
            this.runDate = DateTime.Now.Date; //Note: We do not rerun for prior days because POS Exceptions are only meaningful to run for today.
            this.inputLocation = @"%BATCH_ROOT%\RXS516\Input\";
            this.outputLocation = @"%BATCH_ROOT%\RXS516\Output\";
            this.archiveLocation = @"%BATCH_ROOT%\RXS516\Archive\";
            this.rejectLocation = @"%BATCH_ROOT%\RXS516\Reject\";
            this.fileHelper = new FileHelper(this.inputLocation, this.outputLocation, this.archiveLocation, this.rejectLocation);
            this.emailExclusionsCsvFileName = "Store_Emails_Already_Sent_Today_" + this.runDate.ToString("yyyyMMdd") + ".csv";
            this.apiCallFailuresCsvFileName = "API_Call_Failures_" + this.runDate.ToString("yyyyMMdd") + ".csv";
            this.notificationsSentListThisRun = new List<NotificationsSent>();

            Log.LogInfo("Executing POSExceptionAPICalls with RunDate [{0}].", this.runDate);
            List<UnresolvedPosExceptionRecord> unresolvedPosExceptions = new List<UnresolvedPosExceptionRecord>();
            List<ProcessedPosException> processedPosExceptions = new List<ProcessedPosException>();
            this.apiCallFailures = new List<APICallFailures>();

            Log.LogInfo("Read file to get a list of Unresolved POS Exceptions from 1010data.");
            DelimitedStreamReaderOptions unresolvedPosExceptionFileOptions = new DelimitedStreamReaderOptions(
                Constants.CharComma,
                new Nullable<char>(),
                true,
                false,
                9,
                1
            );

            unresolvedPosExceptions = this.fileHelper.ReadFilesToList<UnresolvedPosExceptionRecord>(
                "POS_Exception_Unresolved_" + this.runDate.ToString("yyyyMMdd") + ".csv",
                unresolvedPosExceptionFileOptions,
                true);

            if (unresolvedPosExceptions.Count == 0)
            {
                Log.LogInfo("There are no Unresolved POS Exceptions to process.");
            }
            else
            {
                Log.LogInfo(string.Format("There are [{0}] Type 2 (uses the Sell API), [{1}] of Type 4 (no API calls to make), and [{2}] of Type 8 (uses the Refund API) POS Exception records to be processed.",
                    unresolvedPosExceptions.Where(r => r.classification_code == 2).Count(),
                    unresolvedPosExceptions.Where(r => r.classification_code == 4).Count(),
                    unresolvedPosExceptions.Where(r => r.classification_code == 8).Count()));

                var posMessaging = new POSMessaging();

                foreach (UnresolvedPosExceptionRecord posException in unresolvedPosExceptions)
                {
                    bool apiCallWasSuccessful = false;

                    //Process the Type 2 Exceptions (uses the Sell API).
                    if (posException.classification_code == 2 ||
                        (posException.classification_code == 4 &&
                         posException.in_wcb == "N")
                       )
                    {
                        try
                        {
                            apiCallWasSuccessful = posMessaging.SellPrescription(
                                posException.store_num.ToString(),
                                posException.rx_num,
                                (short)posException.refill_num,
                                (short)posException.part_seq_num,
                                posException.sold_datetime,
                                posException.copay
                                );
                        }
                        catch (Exception ex)
                        {
                            //Log.LogWarn($"Call to ERx API SellPrescription failed with {}");
                            this.apiCallFailures.Add(new APICallFailures
                            {
                                exception_date = posException.exception_date,
                                store_num = posException.store_num,
                                rx_num = posException.rx_num,
                                refill_num = posException.refill_num,
                                part_seq_num = posException.part_seq_num,
                                classification_code = posException.classification_code,
                                in_wcb = posException.in_wcb,
                                sold_datetime = posException.sold_datetime,
                                copay = posException.copay,
                                APICallMadeDate = DateTime.Now,
                                APIName = "SellPrescription",
                                APIExceptionMessage = ex.Message
                            });
                        }
                    }

                    //Process the Type 6 Exceptions (uses the Refund API).
                    if (posException.classification_code == 8)
                    {
                        try
                        {
                            apiCallWasSuccessful = posMessaging.RefundPrescription(
                                posException.store_num.ToString(),
                                posException.rx_num,
                                (short)posException.refill_num,
                                (short)posException.part_seq_num,
                                posException.sold_datetime,
                                posException.copay
                                );
                        }
                        catch (Exception ex)
                        {
                            //Log.LogWarn($"Call to ERx API RefundPrescription failed with {ex.Message}");
                            this.apiCallFailures.Add(new APICallFailures
                            {
                                exception_date = posException.exception_date,
                                store_num = posException.store_num,
                                rx_num = posException.rx_num,
                                refill_num = posException.refill_num,
                                part_seq_num = posException.part_seq_num,
                                classification_code = posException.classification_code,
                                in_wcb = posException.in_wcb,
                                sold_datetime = posException.sold_datetime,
                                copay = posException.copay,
                                APICallMadeDate = DateTime.Now,
                                APIName = "RefundPrescription",
                                APIExceptionMessage = ex.Message
                            });
                        }
                    }

                    //Create a record that will be uploaded back into 1010data.
                    processedPosExceptions.Add(new ProcessedPosException
                    {
                        resolved_datetime = TenTenHelper.FormatDateWithTimeForTenUp(DateTime.Now),
                        userdata_uid = "wegmans_rx_batch", //This user id is merely used to distinguish updates made by the batch user from updates made by real humans.
                        exception_date = posException.exception_date,
                        store_num = posException.store_num,
                        rx_num = posException.rx_num,
                        refill_num = posException.refill_num,
                        part_seq_num = posException.part_seq_num,
                        classification_code = posException.classification_code,
                        in_wcb = posException.in_wcb,
                        is_resolved = apiCallWasSuccessful ? 1 : 0
                    });
                }

                //Send notifications to stores for any yet to be resolved exceptions.
                //Exclude any recent Exception Type 2's for up to three days because
                //those probably originated from Covid Vaccination Clinics, and those
                //just need some time to marinate in McKesson EnterpriseRx.
                var exceptionsToBeManuallyResolvedByStores = processedPosExceptions
                    .Where(r => r.is_resolved == 0 &&
                                (r.classification_code == 4 ||
                                 r.classification_code == 8 ||
                                 (r.classification_code == 2 &&
                                  Convert.ToInt32((DateTime.Now.Date.Subtract(DateTime.ParseExact(
                                      r.exception_date.ToString(),
                                      "yyyyMMdd",
                                      CultureInfo.InvariantCulture,
                                      DateTimeStyles.None)).TotalDays)) >= 3
                                 )
                                )
                          )
                    .Select(s => s)
                    .ToList();
                NotifyStores(exceptionsToBeManuallyResolvedByStores);
            }

            Log.LogInfo("Make a file to be uploaded to 1010data with results from API Calls.");
            this.fileHelper.WriteListToFile<ProcessedPosException>(
                processedPosExceptions,
                this.outputLocation + "POS_Exceptions_Processed_" + this.runDate.ToString("yyyyMMdd") + ".csv",
                true,
                ",",
                string.Empty,
                true,
                false
                );

            Log.LogInfo("Write/append to file the list of stores that got an email so that we do not spam them too much in one day from multiple runs.");
            this.fileHelper.WriteListToFile<NotificationsSent>(
                this.notificationsSentListThisRun,
                this.inputLocation + this.emailExclusionsCsvFileName,
                true,
                ",",
                string.Empty,
                true,
                true
                );

            Log.LogInfo("Write/append to file the list of of API Call Failures.");
            this.fileHelper.WriteListToFile<APICallFailures>(
                this.apiCallFailures,
                this.rejectLocation + this.apiCallFailuresCsvFileName,
                true,
                ",",
                Constants.CharDoubleQuote.ToString(),
                true,
                true
                );

            Log.LogInfo("Finished running POSExceptionAPICalls.");
            result = this.returnCode.IsFailure ? 1 : 0;
        }

        protected override void DoDispose(bool instantiatedInternally)
        {
        }

        private void NotifyStores(List<ProcessedPosException> exceptionsStillUnresolved)
        {
            if (exceptionsStillUnresolved.Count == 0)
                return;

            EmailHelper emailHelper;
            string emailSubject = string.Empty;
            string emailBodyTemplate = string.Empty;
            string emailFrom = ConfigurationManager.AppSettings["RXS516_RXBSTEmailFrom"].ToString();
            string emailToWhenUnderThresholdValue = emailFrom; // Send only to PharmacyIT
            string storeEmailTo = ConfigurationManager.AppSettings["RXS516_StoreEmailToCSV"].ToString();
            string adminEmailTo = ConfigurationManager.AppSettings["RXS516_AdminEmailToCSV"].ToString();
            string storeSpecificEmailTo = string.Empty;
            string emailCc = ConfigurationManager.AppSettings["RXS516_EmailCcCSV"].ToString();
            string emailBcc = ConfigurationManager.AppSettings["RXS516_EmailBccCSV"].ToString();
            int alertThreshhold = Convert.ToInt32(ConfigurationManager.AppSettings["RXS516_AlertThreshhold"].ToString());
            int max1010ConcurrentUsers = Convert.ToInt32(ConfigurationManager.AppSettings["Max1010ConcurrentUsers"].ToString());
            List<NotificationsSent> notificationExclusionList = new List<NotificationsSent>();
            bool isFirstRunOfTheDay = !this.fileHelper.FileExists(this.inputLocation + this.emailExclusionsCsvFileName);

            if (!isFirstRunOfTheDay)
            { 
                Log.LogInfo("Read file to get a list of stores that already got an email from previous runs earlier today so that we do not spam them too much from subsequent runs all on the same day.");
                DelimitedStreamReaderOptions emailExclusionsFileOptions = new DelimitedStreamReaderOptions(
                    Constants.CharComma,
                    new Nullable<char>(),
                    true,
                    false,
                    3,
                    1
                );

                notificationExclusionList = this.fileHelper.ReadFilesToList<NotificationsSent>(
                    this.emailExclusionsCsvFileName,
                    emailExclusionsFileOptions,
                    false);
            }

            //Sort to prioritize notifications to those stores having the most POS Exceptions.
            var storeExceptions = exceptionsStillUnresolved
                .GroupBy(g => new
                {
                    g.store_num
                })
                .Select(s => new
                {
                    StoreNbr = s.First().store_num,
                    RecordCount = s.Count()
                })
                .OrderByDescending(o => o.RecordCount)
                .ToList();

            //The first run of the day limits the number of notifications to send due to 1010data's limit on concurrent users logged in.
            int limitToSendInFirstRunOfTheDay = (storeExceptions.Count > (max1010ConcurrentUsers * 2)) ?
                (max1010ConcurrentUsers * 2) : storeExceptions.Count;

            foreach (var storeException in storeExceptions)
            {
                if ((isFirstRunOfTheDay && this.notificationsSentListThisRun.Count <= limitToSendInFirstRunOfTheDay) ||
                    (!isFirstRunOfTheDay && notificationExclusionList.Where(r => r.store_num == storeException.StoreNbr).Count() == 0))
                { 
                    storeSpecificEmailTo = storeEmailTo.Replace("THREE_DIGIT_STORE_NBR", storeException.StoreNbr.ToString("000"));
                    emailSubject = string.Format("Please process [{0}] POS Exceptions for Store [{1}], Do Not Reply.", storeException.RecordCount, storeException.StoreNbr);
                    emailBodyTemplate = string.Format(@"<!DOCTYPE html><html><head></head><body><b>Hello Pharmacy Store {0},</b><br/>
<br/>
This email is to inform you that you have a total of <span style='font-weight:bold; background-color:yellow;'>[{1}]</span> POS Exception(s) to process as of today {2:MM/dd/yyyy}. 
Please click the &quot;1010Data Forms&quot; link under the &quot;Workflow&quot; section of the quick links and complete the POS Exceptions report <b>TODAY</b>. 
You will continue to receive this notification until all POS Exceptions have been resolved.
<br/>
<br/>
<b>For guidance on how to resolve these exceptions, please reference the workstation guide: </b>
<a href='https://wegmans.sharepoint.com/sites/StoreOps/Pharmacy/Pharmacy/Pharmacy%20Job%20Aids/1010%20POS%20Exceptions.pdf'>1010 POS Exceptions.pdf (sharepoint.com)</a>
<br/>
<br/>
<i>Thanks,</i><br/>
<i>Pharmacy Business Solutions</i><br/>
<i>DO NOT REPLY to this email directly as it will go to an unmonitored mailbox.  If you need additional help with a prescription please fill out a “POS Removal Form”: Pages - POS Exception Removal Form (sharepoint.com).</i><br/>
</body></html>
", 
storeException.StoreNbr, 
storeException.RecordCount, 
this.runDate);

                    Log.LogInfo("Sending email to Store [{0}].", storeException.StoreNbr);
                
                    emailHelper = new EmailHelper(
                        emailFrom,
                        storeSpecificEmailTo,
                        string.Empty,
                        emailBcc);

                    try
                    { 
                        emailHelper.SendEmail(
                            emailSubject,
                            emailBodyTemplate,
                            System.Net.Mail.MailPriority.Normal,
                            true);

                        this.notificationsSentListThisRun.Add(new NotificationsSent
                        {
                            store_num = storeException.StoreNbr,
                            pos_exceptions_count = storeException.RecordCount,
                            datetime_sent = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                        });
                    }
                    catch (Exception ex)
                    {
                        Log.LogWarn($"EXCEPTION while sending email to Store with these email addressess [{storeSpecificEmailTo}]. (Note that the sending of emails is NOT an essential feature to POS Exceptions.) {ex.Message}");
                        this.notificationsSentListThisRun.Add(new NotificationsSent
                        {
                            store_num = storeException.StoreNbr,
                            pos_exceptions_count = storeException.RecordCount,
                            datetime_sent = ex.Message
                        });
                    }
                }
            }

            Log.LogInfo("Send notification to RxBST and Dev Pharmacy teams to investigate if issues still exist and maybe open a ticket with McKesson.");
            emailHelper = new EmailHelper(
                emailFrom,
                (exceptionsStillUnresolved.Count > alertThreshhold ? adminEmailTo : emailToWhenUnderThresholdValue),
                (exceptionsStillUnresolved.Count > alertThreshhold ? emailCc : string.Empty),
                emailBcc);
            emailSubject = string.Format("{1}There are [{2}] Unresolved POS Exceptions as of [{0:MM/dd/yyyy}] from batch job RXS516", 
                this.runDate,
                (exceptionsStillUnresolved.Count > alertThreshhold ? "ALERT - " : string.Empty),
                exceptionsStillUnresolved.Count);
            emailBodyTemplate = string.Format(@"{7}Hello RxBST and Dev Pharmacy,<br/>
<br/>
There are a total of <b>[{0}] POS Exceptions across [{6}] stores</b> generated by Control-M batch job RXS516 as of [{1:MM/dd/yyyy}]. 
Note that the number of unresolved exceptions include those still unresolved within the past 366 days. 
This job RXS516 also verifies that POS exceptions identified within the past 30 days are still valid exceptions, so it is possible some of these exceptions will resolve themselves automatically should POS and McKesson tie up given some time to synchronize. 
So, it is possible the numbers reported today reflect older still unresolved exceptions in addition to any next exceptions identified today. 
Log into <a href='https://wegmans.1010data.com/'>1010data</a> and view the POS Exceptions Report for further details.<br/>
<br/>
Here is a breakdown of these POS Exceptions by Exception Type across all [{6}] stores:<br/><ul>
<li>[{3}] are of Exception Type 2 (sale exists in POS but transaction missing in EnterpriseRx)</li>
<li>[{4}] are of Exception Type 4 (Rx possibly in Will Call Bin, so stores need to manually correct)</li>
<li>[{5}] are of Exception Type 8 (refund exists in POS but transaction missing in EnterpriseRx)</li>
</ul><br/>
Here is a breakdown of these POS Exceptions of any Exception Type by store:<br/><ol>",
                    exceptionsStillUnresolved.Count,
                    this.runDate,
                    alertThreshhold,
                    exceptionsStillUnresolved.Where(r => r.classification_code == 2).Count(),
                    exceptionsStillUnresolved.Where(r => r.classification_code == 4).Count(),
                    exceptionsStillUnresolved.Where(r => r.classification_code == 8).Count(),
                    storeExceptions.Count,
                    (exceptionsStillUnresolved.Count > alertThreshhold ? 
                        "<span style='font-weight:bold; background-color:yellow;'>ALERT</span> ": string.Empty));

            foreach (var storeException in storeExceptions)
            {
                string email_notification = "will be sent an email next run of Control-M job RXS516";

                if (notificationExclusionList.Where(r => r.store_num == storeException.StoreNbr).Count() == 1)
                {
                    email_notification = "was sent an email at " + notificationExclusionList
                        .Where(r => r.store_num == storeException.StoreNbr)
                        .Select(s => s.datetime_sent)
                        .FirstOrDefault();
                }
                else
                {
                    if (this.notificationsSentListThisRun.Where(r => r.store_num == storeException.StoreNbr).Count() == 1)
                    { 
                        email_notification = "was sent an email at " + this.notificationsSentListThisRun
                            .Where(r => r.store_num == storeException.StoreNbr)
                            .Select(s => s.datetime_sent)
                            .FirstOrDefault();
                    }
                }

                emailBodyTemplate += string.Format("<li>Store {0:000} has [{1}] and {2}.</li>", 
                    storeException.StoreNbr, 
                    storeException.RecordCount,
                    email_notification);
            }

            //Notify teams of the scope of API Call Exceptions.
            var scopeOfAPICallExceptions = this.apiCallFailures
                .GroupBy(g => new
                {
                    g.APIExceptionMessage
                })
                .Select(s => new
                {
                    Message = s.First().APIExceptionMessage,
                    MessageCount = s.Count()
                })
                .OrderByDescending(o => o.Message)
                .ToList();

            emailBodyTemplate += string.Format(@" </ol><br/>
<b>Here is the scope and type of API Call Failures today:</b><br/>
<table style='border:1px solid black;' cellpadding='0' cellspacing='0'><tr><th>Counts</th><th>API Call Failure Messages</th></tr>
");

            foreach (var scopeOfAPICallException in scopeOfAPICallExceptions)
            {
                emailBodyTemplate += string.Format(@"
<tr><td style='border:1px solid black;'>{0}</td>
<td style='border:1px solid black;'>{1}</td></tr>",
scopeOfAPICallException.MessageCount,
scopeOfAPICallException.Message);
            }

            if (scopeOfAPICallExceptions.Count == 0)
            {
                emailBodyTemplate += string.Format(@"
<tr><td style='border:1px solid black;'>{0}</td>
<td style='border:1px solid black;'>{1}</td></tr>",
0,
"(no failures occured in this job run)");
            }

            emailBodyTemplate += string.Format(@" </table>
<br/>
For more details of API Call Failures for each Store/Rx/Refill, see<br/>
{1}<br/>
<br/>
Whenever this email is raised to an ALERT status, then RxBST and Dev Pharmacy should work together to determine if there are too many Unresolved POS Exceptions that priority to investigate root cause is made clear.
The threshhold for raising this email to an ALERT is when the total number of Unresolved POS Exceptions is greater than a threshhold of [{0}], which is subjective so feel free to modify this setting in the Wegmans.InterfaceEngine.exe.config file in the PharmacyBusiness repository in Azure DevOps. 
Keep in mind as you investigate that the root cause(s) could be:
<ul>
<li>user error (in which case Pharmacy Trainers might optimize training guides and/or ask users to revisit training guides)</li>
<li>some issue with the McKesson API (in which case RxBST should open a ticket with McKesson)</li>
<li>batch job RXS516 may have opportunity for improvements (in which case Dev Pharmacy should read through logs and improve code that write to those logs if necessary to identify root cause)</li>
<li>some combination of all of the above</li>
</ul>
<br/>
For more information, see KBA00012983 here <a href='https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAODE3591DJOR495JX'>here</a>.<br/>
</body></html>",
alertThreshhold,
(Environment.ExpandEnvironmentVariables(this.rejectLocation) + this.apiCallFailuresCsvFileName));

            try
            {
                emailHelper.SendEmail(
                    emailSubject,
                    emailBodyTemplate,
                    (exceptionsStillUnresolved.Count > alertThreshhold ? 
                        System.Net.Mail.MailPriority.High : System.Net.Mail.MailPriority.Low),
                    true);
            }
            catch (Exception ex)
            {
                Log.LogWarn($"EXCEPTION while sending ALERT to this email address [{emailFrom}]. (Note that the sending of emails is NOT an essential feature to POS Exceptions unless the total count is more than the job threshhold of [{alertThreshhold}].) {ex.Message}");

                if (exceptionsStillUnresolved.Count > alertThreshhold)
                    this.returnCode.IsFailure = true;
            }

            Log.LogInfo("Completed notifications.");
        }
    }
}
