using CaseServiceWrapper;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.EmailSender;
using Library.EmplifiInterface.Exceptions;
using Library.McKessonDWInterface.DataModel;

namespace Library.EmplifiInterface
{
    public interface IEmplifiInterface
    {
        /// <summary>
        /// Get the case search results for the given date
        /// </summary>
        /// <param name="startDateTime"></param>
        /// <param name="endDateTime"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<Case>> GetDelayAndDenialOutboundStatusAsync(
            DateTime startDateTime,
            DateTime endDateTime,
            CancellationToken c);

        /// <summary>
        /// Run the search request for the given filter list, search case, start date and end date
        /// </summary>
        /// <param name="filterList"></param>
        /// <param name="searchCase"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<IEnumerable<Case>> RunSearchRequestAsync(
            IEnumerable<Filter> filterList,
            SearchCase searchCase,
            CancellationToken c);

        /// <summary>
        /// Writes a list of objects to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="downloadList"></param>
        /// <param name="downloadFileName"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        /// <param name="c"></param>
        Task WriteListToFileAsync<T>(
            List<T> downloadList,
            string downloadFileName,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            CancellationToken c);

        /// <summary>
        /// Writes a list of objects to a file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="downloadList"></param>
        /// <param name="downloadFileName"></param>
        /// <param name="hasHeaderRow"></param>
        /// <param name="delimiter"></param>
        /// <param name="textQualifier"></param>
        /// <param name="makeExtractWhenNoData"></param>
        /// <param name="shouldAppendToExistingFile"></param>
        /// <param name="c"></param>
        Task WriteListToFileAsync<T>(
            List<T> downloadList,
            string downloadFileName,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            bool shouldAppendToExistingFile,
            CancellationToken c);

        /// <summary>
        /// Send a notification email for data integrity issues
        /// </summary>
        /// <param name="emailException"></param>
        /// <param name="dataIntegrityException"></param>
        /// <returns></returns>
        void SendDataIntegrityNotification(
            IEmailExceptionComposer emailException,
            DataIntegrityException dataIntegrityException,
            List<string> exceptions);

        /// <summary>
        /// Send a notification email for data integrity issues
        /// </summary>
        /// <param name="emailNotification"></param>
        /// <param name="notificationEmailSubject"></param>
        /// <param name="notificationEmailBody"></param>
        void SendEligibilityNotification(
            IEmailExceptionComposer emailNotification,
            string notificationEmailSubject,
            string notificationEmailBody);

        /// <summary>
        /// Set the extracted date for the given records in the CRM application.
        /// </summary>
        /// <param name="successfullyExportedRecords"></param>
        /// <param name="extractedDate"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<List<string>> SetExtractedDateAsync(
            IEnumerable<EmplifiRecordReportingStatus> successfullyExportedRecords,
            DateTime extractedDate,
            CancellationToken c);

        Task<List<string>> ProcessAstuteAdherenceDispenseAsync(
            IEnumerable<AstuteAdherenceDispenseReportRow> astuteAdherenceDispenseReportRows,
            CancellationToken c);

        void SendDataNotification(
            IEmailExceptionComposer emailComposer,
            Dictionary<decimal, List<string>> dataConstraints,
            EmailAttributeData emailAttributeData,
            List<string> exceptions);

        void SendVobNotification(
            IEmailExceptionComposer emailNotification,
            string notificationEmailSubject,
            string notificationEmailBody);

        /// <summary>
        /// Process JPAP Triage data into Emplifi and return a TriageNotification object with details for user notification.
        /// </summary>
        /// <param name="jpapTriages"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<TriageNotification> ProcessJpapTriageAsync(
            IEnumerable<JpapTriage> jpapTriages,
            CancellationToken c);

        /// <summary>
        /// Send a triage notification email.
        /// </summary>
        /// <param name="emailNotification"></param>
        /// <param name="notificationEmailSubject"></param>
        /// <param name="notificationEmailBody"></param>
        void SendTriageNotification(
            IEmailExceptionComposer emailNotification,
            string notificationEmailSubject,
            string notificationEmailBody);


        /// <summary>
        /// Process Oncology Triage data into Emplifi and return a TriageNotification object with details for user notification
        /// </summary>
        /// <param name="oncologyTriages"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<TriageNotification> ProcessOncologyTriageAsync(
            IEnumerable<OncologyTriage> oncologyTriages,
            CancellationToken c);

        /// <summary>
        /// Process Verification of Benefits data into Emplifi and return a TriageNotification object with details for user notification
        /// </summary>
        /// <param name="verificationOfBenefits"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<TriageNotification> ProcessVerificationOfBenefitsAsync(
            IEnumerable<VerificationOfBenefits> verificationOfBenefits,
            string file,
            CancellationToken c);

        /// <summary>
        /// Process Oncology Voucher Triage data into Emplifi and return a TriageNotification object with details for user notification
        /// </summary>
        /// <param name="oncologyTriages"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        Task<TriageNotification> ProcessOncologyVoucherTriageAsync(
            IEnumerable<OncologyVoucherTriage> oncologyVoucherTriages,
            CancellationToken c);

        Task<EligibilityNotification> ProcessJpapEligibilityAsync(
            IEnumerable<JpapEligibilityRow> jpapEligibilityRows,
            CancellationToken c);

        Task<IEnumerable<Case>> GetJpapOutboundStatusAsync(
            DateTime startDateTime,
            DateTime endDateTime,
            CancellationToken c);
    }
}
