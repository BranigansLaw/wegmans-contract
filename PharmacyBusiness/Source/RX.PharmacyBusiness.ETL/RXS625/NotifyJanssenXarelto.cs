namespace RX.PharmacyBusiness.ETL.RXS625
{
    using System.Configuration;
    using Wegmans.PharmacyLibrary.ETL;
    using Wegmans.PharmacyLibrary.ETL.Core;

    [JobNotes("RXS625", "Notify Business about Janssen Xarelto Files.", "KBA00042622", "https://wegmans-smartit.onbmc.com/smartit/app/#/knowledge/AGGAA5V0FZ07RAQ2TBFHQ1UWTB7XX4")]
    public class NotifyJanssenXarelto : ETLBase
    {
        protected override void Execute(out object result)
        {
            ReturnCode returnCode = new ReturnCode();
            EmailHelper emailHelper = new EmailHelper(
                ConfigurationManager.AppSettings["RXS625.EmailFrom"].ToString(),
                ConfigurationManager.AppSettings["RXS625.EmailToCSV"].ToString(),
                ConfigurationManager.AppSettings["RXS625.EmailCcCSV"].ToString(),
                ConfigurationManager.AppSettings["RXS625.EmailBccCSV"].ToString());
            string emailSubject = "The Wegmans Janssen Xarelto files have been sent.";
            string emailBodyTemplate = @"<!DOCTYPE html><html><head></head><body><b>Hello RXBST,</b><br />
<br />
This email is to inform you that Janssen Xarelto files have been sent.
<br />
<br />
Thanks,<br />
<i>Wegmans Pharmacy IT Development Team</i><br />
</body></html>
";

            Log.LogInfo("Begin Janssen Xarelto notification.");

            emailHelper.SendEmail(
                emailSubject,
                emailBodyTemplate,
                System.Net.Mail.MailPriority.Low,
                true);

            Log.LogInfo("Completed Janssen Xarelto notification.");
            result = returnCode.IsFailure ? 1 : 0;
        }
        protected override void DoDispose(bool instantiatedInternally)
        {
        }
    }
}
