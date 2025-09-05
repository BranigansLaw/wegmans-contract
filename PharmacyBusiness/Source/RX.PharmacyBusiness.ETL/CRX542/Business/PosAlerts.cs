using RX.PharmacyBusiness.ETL.CRX542.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Wegmans.PharmacyLibrary.Logging;

namespace RX.PharmacyBusiness.ETL.CRX542.Business
{
    public class PosAlerts
    {
        private const int _minutesGapThreshold = 10;
        private TimeSpan _morningStoreOpenTime = new TimeSpan(8, 30, 0);
        private TimeSpan _afternoonStoreCloseTime = new TimeSpan(17, 0, 0);
        private DateTime RunDate { get; set; }
        private List<PosGapsInTime> TimeGaps { get; set; }

        [ExcludeFromCodeCoverage]
        private RxLogger Log => new RxLogger(MethodBase.GetCurrentMethod().DeclaringType?.FullName);

        public PosAlerts()
        {
            this.TimeGaps = new List<PosGapsInTime>();
        }

        /// <summary>
        /// Identify gaps in POS reporting times.
        /// </summary>
        /// <param name="posRecords"></param>
        /// <returns></returns>
        public void GetPosGapsInReportingTimes(DateTime runDate, List<BrickAndMortarPosRecord> posRecords)
        {
            this.RunDate = runDate;
            DateTime startOfMissingRecords = DateTime.MinValue;
            DateTime endOfMissingRecords = DateTime.MinValue;
            DateTime previousRecord = DateTime.MinValue;
            int currentMinutesGap = 0;
            int largestMinutesGap = 0;

            foreach (var posRecord in posRecords
                .OrderBy(r => r.Transaction_Date_Time)
                .ThenBy(r => r.Store_Num))
            {
                if (previousRecord == DateTime.MinValue)
                {
                    previousRecord = posRecord.Transaction_Date_Time;
                    continue;
                }

                currentMinutesGap = (int)(posRecord.Transaction_Date_Time - previousRecord).TotalMinutes;

                if (currentMinutesGap > _minutesGapThreshold)
                {
                    if (startOfMissingRecords == DateTime.MinValue)
                    {
                        startOfMissingRecords = previousRecord;
                    }

                    endOfMissingRecords = posRecord.Transaction_Date_Time;

                    if (startOfMissingRecords.TimeOfDay >= _morningStoreOpenTime && endOfMissingRecords.TimeOfDay <= _afternoonStoreCloseTime)
                    { 
                        this.TimeGaps.Add(new PosGapsInTime
                        {
                            RunDate = runDate,
                            MinutesGap = currentMinutesGap,
                            StartOfMissingPosRecords = startOfMissingRecords,
                            EndOfMissingPosRecords = endOfMissingRecords
                        });
                    }

                    startOfMissingRecords = DateTime.MinValue;
                    endOfMissingRecords = DateTime.MinValue;

                    if (currentMinutesGap > largestMinutesGap)
                        largestMinutesGap = currentMinutesGap;
                }

                previousRecord = posRecord.Transaction_Date_Time;
            }

            Log.LogInfo($"With the POS time gap threshhold set to [{_minutesGapThreshold}], there are [{this.TimeGaps.Count()}] gaps in time across all stores for Run Date [{this.RunDate}].");

            if (this.TimeGaps.Count() > 0)
            {
                SendAlert();
            }
        }

        /// <summary>
        /// Send an email alert if meet the threshold of gap in time across all stores.
        /// </summary>
        public void SendAlert()
        {
            EmailHelper emailHelper = new EmailHelper(
                ConfigurationManager.AppSettings["CRX542.EmailFrom"].ToString(),
                ConfigurationManager.AppSettings["CRX542.EmailToCSV"].ToString(),
                ConfigurationManager.AppSettings["CRX542.EmailCcCSV"].ToString(),
                ConfigurationManager.AppSettings["CRX542.EmailBccCSV"].ToString());
            string emailSubject = $"ALERT: There are potentially missing POS transactions for Run Date [{this.RunDate.ToString("MM/dd/yyyy")}].";
            string emailBodyTemplate = string.Format(@"<!DOCTYPE html><html><head></head><body><b>Hello POS Fans,</b><br />
<br />
This alert occurs whenever gaps in POS times are greater than [{0}] minutes across all stores between [{1}] and [{2}], which may be an indication of missing POS transactions.
<br /><br />
This notification is for informational purposes only and does not require any action. No jobs have failed.
<br /><br />",
_minutesGapThreshold,
_morningStoreOpenTime,
_afternoonStoreCloseTime);

            emailBodyTemplate += "<table border='1'><tr><th>Run Date</th><th>Minutes Gap</th><th>Start of Missing Records</th><th>End of Missing Records</th></tr>";
            foreach (var timeGap in this.TimeGaps)
            {
                emailBodyTemplate += $"<tr><td>{timeGap.RunDate.ToString("MM/dd/yyyy")}</td><td>{timeGap.MinutesGap}</td><td>{timeGap.StartOfMissingPosRecords}</td><td>{timeGap.EndOfMissingPosRecords}</td></tr>";
            }
            emailBodyTemplate += "</table>";
            emailBodyTemplate += @"<br />
Thanks,<br />
<i>Wegmans Pharmacy IT Development Team</i><br />
</body></html>";

            emailHelper.SendEmail(
                emailSubject,
                emailBodyTemplate,
                System.Net.Mail.MailPriority.High,
                true);

            Log.LogInfo("Email sent that a gap in POS reporting was greater than a given threshhold.");
        }
    }
}
