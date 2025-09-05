namespace RX.PharmacyBusiness.ETL.CRX582
{
    using System.Configuration;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("CRX582", "Download Third Party Claims.", "KBA00023012", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGHAA5V0G0U0AAO5XDAO6M9SSN2HDY")]
    public class NotifyThirdPartyClaims : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            EmailHelper emailHelper = new EmailHelper(
                ConfigurationManager.AppSettings["CRX582.EmailFrom"].ToString(),
                ConfigurationManager.AppSettings["CRX582.EmailToCSV"].ToString(),
                ConfigurationManager.AppSettings["CRX582.EmailCcCSV"].ToString(),
                ConfigurationManager.AppSettings["CRX582.EmailBccCSV"].ToString());
            string emailSubject = "The Wegmans SmartRx claim files are now available.";
            string emailBodyTemplate = @"<!DOCTYPE html><html><head></head><body><b>Hello SmartRx Fans,</b><br />
<br />
This email is to inform you that SmartRx claim files are now available.
<br />
<br />
Thanks,<br />
<i>Wegmans Pharmacy IT Development Team</i><br />
</body></html>
";

            Log.LogInfo("Begin SmartRx notification.");

            emailHelper.SendEmail(
                emailSubject,
                emailBodyTemplate,
                System.Net.Mail.MailPriority.Low,
                true);

            Log.LogInfo("Completed SmartRx notification.");
            result = returnCode.IsFailure ? 1 : 0;
        }
        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
