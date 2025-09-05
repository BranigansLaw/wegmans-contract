using CaseServiceWrapper;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.EmailSender;
using Library.EmplifiInterface.Exceptions;
using Library.EmplifiInterface.Helper;
using Library.McKessonDWInterface.DataModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using System.Collections.ObjectModel;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

namespace Library.EmplifiInterface
{
    public class EmplifiInterfaceImp : IEmplifiInterface
    {
        private readonly IOptions<EmplifiConfig> _config;
        private readonly ILogger<EmplifiInterfaceImp> _logger;
        private readonly ICaseService _caseService;
        private readonly IDelayAndDenialHelper _delayAndDenialHelperImp;
        private readonly IJpapTriageHelper _jpapTriageHelper;
        private readonly IOncologyTriageHelper _oncologyTriageHelper;
        private readonly IAstuteAdherenceDispenseHelper _astuteAdherenceDispenseHelper;
        private readonly IOncologyVoucherTriageHelper _oncologyVoucherTriageHelper;
        private readonly IJpapEligibilityHelper _jpapEligibilityHelperImp;
        private readonly IVerificationOfBenefitsHelper _verificationOfBenefitsHelper;
        private readonly IJpapDispenseAndStatusHelper _jpapDispenseAndStatusHelper;

        /// <summary>
        /// Retry logic
        /// </summary>
        private readonly AsyncRetryPolicy RetryPolicy;
        private const string TRADITIONAL_PROGRAM_TYPE = "Traditional Specialty";
        private const string DISPENSE_LOGIC_MISMATCH = "Dispense Logic Mismatch";
        private const string PATIENT_NOT_FOUND = "patient not found";
        private const string NO_DISPENSE = "NO DISPENSE";
        private const string CaseNotFoundAction = "Case Not Found";
        private const string SHIPPED_PATIENT_STATUS = "A1=Shipped";
        private const string SHIPPED_WORKFLOW_STATUS = "Call Complete-Shipped";
        private const string CLINICAL_QUESTION_DECLINED = "Clinical/Qol - Declined";
        
        // Verification of Benefits 
        private const string ReferredToUserCode = "vobexceptions";
        private const string ActionTypeCodeMultipleCasesFound = "multiple cases found";
        private const string RerunStatus = "RERUN";
        private const string FileNamePrefix = "CarePath_VOB_";
        private const string FileNameSuffix = ".txt";

        // Case Statuses
        private const string CLOSED = "C";
        private const string OPEN = "O";
        private const string ALL = "B";

        private const string ISSUE_SOURCE = "Dispense (Erx)";
        private const string REP_NOTES_TYPE = "Rep Notes";
        private const string CASE_TEXT_DESCRIPTION = "2";
        private const string ID_NOT_FOUND = "id not found";
        private const string DOB_NOT_FOUND = "DOB Not Found";
        private const string NAME_DOB_NOT_FOUND = "Patient Name DOB Not Found";
        private const string PATIENT_ADDRESS_TYPE = "PATIENT";
        private const string DELAY_AND_DENIAL_PROGRAM_TYPE = "Janssen Oncology Delay & Denial";
        private const string JPAP_PROGRAM_TYPE = "JPAP";
        private const string DELAY_AND_DENIAL_VOUCHER_PROGRAM_TYPE = "Janssen Oncology Voucher";
        private const string PROGRAM_HEADER_MISMATCH = "Program Header Mismatch";
        private const string ADMIN_QUEUE_USER = "adminqueue";
        private const string ONCOLOGY_PROGRAM_TYPE = "Janssen Oncology Voucher";
        private const string MULTIPLE_CASES_FOUND = "Dispense Applied To Multiple Cases";
        private const string XARELTO_PROGRAM_TYPE = "Janssen Select - Xarelto";
        private const string NEW_REFERRAL_PATIENT_STATUS = "P1=New Referral";
        private const string PatientIdType = "JPAP CarePath Patient ID";
        private const string CompanyId = "SYS";

        public EmplifiInterfaceImp(
            IOptions<EmplifiConfig> config,
            ILogger<EmplifiInterfaceImp> logger,
            ICaseService caseService,
            IDelayAndDenialHelper delayAndDenialHelperImp,
            IJpapTriageHelper jpapTriageHelper,
            IOncologyTriageHelper oncologyTriageHelper,
            IVerificationOfBenefitsHelper verificationOfBenefitsHelper,
            IAstuteAdherenceDispenseHelper astuteAdherenceDispenseHelper,
            IOncologyVoucherTriageHelper oncologyVoucherTriageHelper,
            IJpapEligibilityHelper jpapEligibilityHelper,
            IJpapDispenseAndStatusHelper jpapDispenseAndStatusHelper)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _caseService = caseService ?? throw new ArgumentNullException(nameof(caseService));
            _delayAndDenialHelperImp = delayAndDenialHelperImp ?? throw new ArgumentNullException(nameof(delayAndDenialHelperImp));
            _jpapTriageHelper = jpapTriageHelper ?? throw new ArgumentNullException(nameof(jpapTriageHelper));
            _oncologyTriageHelper = oncologyTriageHelper ?? throw new ArgumentNullException(nameof(oncologyTriageHelper));
            _verificationOfBenefitsHelper = verificationOfBenefitsHelper ?? throw new ArgumentNullException(nameof(verificationOfBenefitsHelper));
            _astuteAdherenceDispenseHelper = astuteAdherenceDispenseHelper ?? throw new ArgumentNullException(nameof(astuteAdherenceDispenseHelper));
            _oncologyVoucherTriageHelper = oncologyVoucherTriageHelper ?? throw new ArgumentNullException(nameof(oncologyVoucherTriageHelper));
            _jpapEligibilityHelperImp = jpapEligibilityHelper ?? throw new ArgumentNullException(nameof(jpapEligibilityHelper));
            _jpapDispenseAndStatusHelper = jpapDispenseAndStatusHelper ?? throw new ArgumentNullException(nameof(jpapDispenseAndStatusHelper));

            RetryPolicy = Policy
                .Handle<HttpRequestException>(e => e.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                .Or<SocketException>() // Network error
                .WaitAndRetryAsync(
                    retryCount: 10,
                    retryAttempt => TimeSpan.FromSeconds(30 * retryAttempt), // retry after 30s, 60s, ... 5 minutes
                    onRetry: (ex, waitTime) => _logger.LogWarning(ex, $"Retrying in {waitTime.Seconds} seconds")
                );
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Case>> GetDelayAndDenialOutboundStatusAsync(
            DateTime startDateTime,
            DateTime endDateTime,
            CancellationToken c)
        {
            List<Case> returnCaseList = new List<Case>();
            SearchCase searchCase = new SearchCase()
            {
                company_id = "SYS",
                b07_code = "Janssen Oncology Delay & Denial", // Program Type
                case_status = "O", // O-Open, C-Close and B-Both Case Status. We only want Open cases per requirements from Melissa Hatch 7/2/2024 so that her team can close duplicate cases and thus exclude them from reports.
                AddressList = new SearchAddressList()
                {
                    Address =
                        [
                            new SearchAddress
                            {
                                address_type_code = "PATIENT"
                            }
                        ]
                },
            };

            IEnumerable<Case> caseResultsList = new List<Case>();
            List<string> caseIdExclusionList = new List<string>();
            var requestList = _delayAndDenialHelperImp.GetOutboundStatusRequests(startDateTime, endDateTime);

            int requestNbr = 1;
            foreach (var request in requestList)
            {
                _logger.LogDebug($"Processing request [{requestNbr}] out of a total of [{requestList.Count()}] requests.");
                caseIdExclusionList = new List<string>(); //Reset the exclusion list for each request so that we can merge issues found in various requests.
                ICollection<Filter> requestFilters = request;
                int filterSequence = request.LastOrDefault()?.filter_seq ?? 0;

                do
                {
                    _logger.LogDebug($"Looping through results from request nbr [{requestNbr}] using [{filterSequence}] Filters.");

                    if (caseIdExclusionList.Count > 0)
                    {
                        //The API only returns up to 250 results per request, so you need to run the same request repeatedly to page through all of the results.
                        //Add a filter to exclude the prior paged results so that we effectively get the next page of results.
                        filterSequence++;
                        requestFilters.Add(new Filter()
                        {
                            filter_seq = filterSequence,
                            selection_operator = SelectionOperator.NOTIN,
                            selection_category_id = "case_id",
                            selection_code = string.Join(";", caseIdExclusionList)
                        });
                    }

                    caseResultsList = await RunSearchRequestAsync(requestFilters, searchCase, c).ConfigureAwait(false);
                    caseIdExclusionList = caseResultsList.Select(c => c.case_id).ToList();
                    returnCaseList.AddRange(caseResultsList);
                } while (caseResultsList.Count() > 0);
            }

            return returnCaseList;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Case>> RunSearchRequestAsync(
            IEnumerable<Filter> filterList,
            SearchCase searchCase,
            CancellationToken c)
        {
            var request = new CaseSearchRequest()
            {
                UserName = _config.Value.Username,
                Password = _config.Value.Password,
                Type = CaseSearchType.Get,
                Case = searchCase,
                ResponseFormat = new ResponseFormat()
                {
                    CaseList = new CaseListFormat()
                    {
                        Case = new CaseFormat()
                        {
                            AllAttributes = TrueFalseType.Item1,
                            AddressList = new AddressListFormat()
                            {
                                Address = new AddressFormat()
                                {
                                    AllAttributes = TrueFalseType.Item1,
                                    PhoneList = new PhoneFormat[]
                                    {
                                        new PhoneFormat()
                                        {
                                            AllAttributes = TrueFalseType.Item1
                                        }
                                    }
                                }
                            },
                            IssueList = new IssueListFormat()
                            {
                                Issue = new IssueFormat()
                                {
                                    AllAttributes = TrueFalseType.Item1
                                }
                            }
                        }
                    }
                },
                FilterList = new FilterList()
                {
                    keys = "filter_seq",
                    Filter = filterList.ToArray()
                }
            };

            CaseListResponse response = await RetryPolicy.ExecuteAsync(() =>
                _caseService.SearchCaseAsync(request)).ConfigureAwait(false);

            var caseList = new List<Case>();

            if (response != null)
            {
                if (response.Valid != ValidState.Ok)
                {
                    _logger.LogDebug(string.Join(',', response.MessageList.Select(m => m.Text)));
                }

                if (response.Case != null)
                {
                    caseList.AddRange(response.Case);
                }
            }

            return caseList;
        }

        /// <inheritdoc />
        public async Task WriteListToFileAsync<T>(
            List<T> downloadList,
            string downloadFileName,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            CancellationToken c)
        {
            await this.WriteListToFileAsync<T>(
               downloadList,
               downloadFileName,
               hasHeaderRow,
               delimiter,
               textQualifier,
               makeExtractWhenNoData,
               false,
               c).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task WriteListToFileAsync<T>(
            List<T> downloadList,
            string downloadFileName,
            bool hasHeaderRow,
            string delimiter,
            string textQualifier,
            bool makeExtractWhenNoData,
            bool shouldAppendToExistingFile,
            CancellationToken c)
        {
            downloadFileName = Path.Combine(_config.Value.OutputFileLocation, downloadFileName);
            delimiter = delimiter ?? string.Empty;
            textQualifier = textQualifier ?? string.Empty;
            var output = new StringBuilder();
            var fields = new Collection<string>();
            Type elementType = typeof(T);

            _logger.LogInformation("Output data in typed List<T> to output file [{0}].", downloadFileName);

            if (File.Exists(downloadFileName) && !shouldAppendToExistingFile)
            {
                _logger.LogWarning("Output file [{0}] already exists and shouldAppendToExistingFile=false, so now deleting this existing output file.", downloadFileName);
                File.Delete(downloadFileName);
            }

            //If the file already exists and shouldAppendToExistingFile=true, then do not output the header row with the append.
            hasHeaderRow = (
                hasHeaderRow &&
                shouldAppendToExistingFile &&
                File.Exists(downloadFileName) ?
                    false : hasHeaderRow);

            using (var writerOutputData = new StreamWriter(downloadFileName, shouldAppendToExistingFile))
            {
                if (hasHeaderRow)
                {
                    foreach (var propInfo in elementType.GetProperties())
                    {
                        //Get the column label from the ExportHeaderColumnLabelAttribute if it exists, otherwise use the property name.
                        string columnLabel = propInfo.Name;
                        var attributes = propInfo.GetCustomAttributes(false);
                        var columnMapping = attributes.FirstOrDefault(a => a.GetType() == typeof(DataModel.ExportHeaderColumnLabelAttribute));
                        if (columnMapping != null)
                        {
                            var mapsto = columnMapping as DataModel.ExportHeaderColumnLabelAttribute;

                            if (mapsto != null)
                                columnLabel = mapsto.Name;
                        }

                        fields.Add(string.Format("{0}{1}{0}", textQualifier, columnLabel));
                    }

                    await writerOutputData.WriteLineAsync(string.Join(delimiter, fields.ToArray())).ConfigureAwait(false);
                    fields.Clear();
                    hasHeaderRow = false;
                }

                foreach (T record in downloadList)
                {
                    fields.Clear();

                    foreach (var propInfo in elementType.GetProperties())
                    {
                        if ((propInfo.GetValue(record, null) ?? DBNull.Value).GetType() == typeof(System.String))
                        {
                            fields.Add(string.Format("{0}{1}{0}", textQualifier, (propInfo.GetValue(record, null) ?? DBNull.Value).ToString()));
                        }
                        else
                        {
                            fields.Add((propInfo.GetValue(record, null) ?? DBNull.Value).ToString() ?? string.Empty);
                        }
                    }

                    await writerOutputData.WriteLineAsync(string.Join(delimiter, fields.ToArray())).ConfigureAwait(false);
                }
            }

            if (!makeExtractWhenNoData && downloadList.Count == 0)
            {
                _logger.LogInformation("List has zero rows and job is set to not create an empty file, so now deleting this empty output file.");
                if (File.Exists(downloadFileName))
                    File.Delete(downloadFileName);
            }
        }

        /// <inheritdoc />
        public void SendDataIntegrityNotification(
            IEmailExceptionComposer emailException,
            DataIntegrityException dataIntegrityException,
            List<string> exceptions)
        {
            IEmailSenderInterface sender = new EmailSenderInterfaceImp(_logger);
            sender.SmtpClientSendMail(emailException.Compose(_config.Value.RejectFileLocation, dataIntegrityException, _config.Value.NotificationEmailTo, exceptions));
        }

        public void SendDataNotification(
            IEmailExceptionComposer emailComposer,
            Dictionary<decimal, List<string>> dataConstraints,
            EmailAttributeData emailAttributeData,
            List<string> exceptions)
        {
            IEmailSenderInterface sender = new EmailSenderInterfaceImp(_logger);
            sender.SmtpClientSendMail(emailComposer.ComposeDataEmail(_config.Value.RejectFileLocation, dataConstraints, _config.Value.EmplifiDispenseNotificationEmailTo, emailAttributeData.OutputFileName, exceptions));
        }

        /// <inheritdoc/>
        public void SendTriageNotification(
            IEmailExceptionComposer emailNotification,
            string notificationEmailSubject,
            string notificationEmailBody)
        {
            IEmailSenderInterface sender = new EmailSenderInterfaceImp(_logger);
            sender.SmtpClientSendMail(emailNotification.ComposeNotificationEmail(_config.Value.EmplifiTriageNotificationEmailTo, notificationEmailSubject, notificationEmailBody));
        }

        /// <inheritdoc/>
        public void SendEligibilityNotification(
            IEmailExceptionComposer emailNotification,
            string notificationEmailSubject,
            string notificationEmailBody)
        {
            IEmailSenderInterface sender = new EmailSenderInterfaceImp(_logger);
            sender.SmtpClientSendMail(emailNotification.ComposeNotificationEmail(_config.Value.EmplifiEligibilityNotificationEmailTo, notificationEmailSubject, notificationEmailBody));
        }

        public void SendVobNotification(
            IEmailExceptionComposer emailNotification,
            string notificationEmailSubject,
            string notificationEmailBody)
        {
            IEmailSenderInterface sender = new EmailSenderInterfaceImp(_logger);
            sender.SmtpClientSendMail(emailNotification.ComposeNotificationEmail(_config.Value.EmplifiDispenseNotificationEmailTo, notificationEmailSubject, notificationEmailBody));
        }

        /// <inheritdoc />
        public async Task<List<string>> SetExtractedDateAsync(
            IEnumerable<EmplifiRecordReportingStatus> successfullyExportedRecords,
            DateTime extractedDate,
            CancellationToken c)
        {
            List<string> exceptions = new List<string>();

            foreach (var successfullyExportedRecord in successfullyExportedRecords)
            {
                try
                {

                    if (string.IsNullOrEmpty(successfullyExportedRecord.CaseId) ||
                        string.IsNullOrEmpty(successfullyExportedRecord.IssueSeq) ||
                        !successfullyExportedRecord.IsValidForReporting)
                        continue;

                    var status = ValidState.Unknown;
                    int attemptNbr = 1;

                    var request = new CaseListUpdateRequest
                    {
                        UserName = _config.Value.Username,
                        Password = _config.Value.Password,
                        Type = CaseListUpdateType.Update,
                        Case = new Case[]
                        {
                        new Case()
                        {
                            APCWEditMode = APCWEditModeType.Modified,
                            company_id = "SYS",
                            case_id = successfullyExportedRecord.CaseId,
                            IssueList = new IssueList()
                            {
                                Issue = new Issue[]
                                {
                                    new Issue()
                                    {
                                        APCWEditMode = APCWEditModeType.Modified,
                                        issue_seq = successfullyExportedRecord.IssueSeq,
                                        c81_code = extractedDate.ToString()
                                    }
                                }
                            }
                        }
                        },
                        ResponseFormat = new ResponseFormat()
                        {
                            CaseList = new CaseListFormat()
                            {
                                Case = new CaseFormat()
                                {
                                    AllAttributes = TrueFalseType.Item1
                                }
                            }
                        }
                    };

                    int maxAttempts = 3;
                    while (status != ValidState.Ok && attemptNbr < maxAttempts)
                    {
                        CaseListResponse response = await RetryPolicy.ExecuteAsync(() =>
                            _caseService.UpdateCaseAsync(request)).ConfigureAwait(false);

                        if (response != null)
                        {
                            status = response.Valid;

                            if (response.Valid == ValidState.Ok)
                            {
                                successfullyExportedRecord.ExtractedDateValueSetInUpdateApiCall = extractedDate;
                                successfullyExportedRecord.CaseWasReleasedAfterUpdateApiCall = false;
                                await _astuteAdherenceDispenseHelper.ReleaseCaseAsync(successfullyExportedRecord, c).ConfigureAwait(false);
                            }
                            else
                            {
                                _logger.LogDebug($"API to update ExtractedDate for CaseId [{successfullyExportedRecord.CaseId}], IssueSeq [{successfullyExportedRecord.IssueSeq}] had response status: [{response.Valid}], and message is: [{string.Join(',', response.MessageList.Select(m => m.Text))}]");
                            }
                        }

                        if (status != ValidState.Ok)
                        {
                            _logger.LogDebug($"Attempt Nbr [{attemptNbr}] failed out of max of [{maxAttempts}] attempts, so waiting 5 seconds before trying again.");
                            Thread.Sleep(5000);
                        }

                        attemptNbr++;
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex.Message);
                }
            }
            return exceptions;
        }

        /// <inheritdoc />
        public async Task<List<string>> ProcessAstuteAdherenceDispenseAsync(
            IEnumerable<AstuteAdherenceDispenseReportRow> astuteAdherenceDispenseReportRows,
            CancellationToken c)
        {
            List<string> exceptions = new List<string>();

            foreach (var dispense in astuteAdherenceDispenseReportRows)
            {
                try
                {
                    // Look for open CRM cases to update
                    var openCases = await _astuteAdherenceDispenseHelper.FindOpenCasesAsync(dispense, c).ConfigureAwait(false);

                    if (!(openCases is null))
                    {
                        // For the Traditional Specialty program, all drugs will apply to the same patient/program case, for all other programs, there will be one case per patient/program/drug
                        if (dispense.ProgramType == TRADITIONAL_PROGRAM_TYPE)
                        {
                            exceptions.AddRange(await _astuteAdherenceDispenseHelper.ProcessCasesAsync(dispense, openCases, c).ConfigureAwait(false));
                        }
                        else
                        {
                            var casesWithMatchingDrugs = openCases
                                .Where(x => x.IssueList?.Issue?.Count(y => !(y.product_code is null) &&
                                            !string.IsNullOrEmpty(dispense.BaseProductName) &&
                                            y.product_code.Contains(dispense.BaseProductName)) > 0);

                            if (casesWithMatchingDrugs.Count() > 0)
                            {
                                exceptions.AddRange(await _astuteAdherenceDispenseHelper.ProcessCasesAsync(dispense, casesWithMatchingDrugs, c).ConfigureAwait(false));
                            }
                        }
                    }

                    // Create a case in the admin queue for any dispense not matched
                    if (dispense.CasesMatchedInCrm == 0)
                    {
                        await _astuteAdherenceDispenseHelper.CreateAdminQueueCaseAsync(dispense, c).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex.Message);
                }
            }

            return exceptions;
        }

        public async Task<TriageNotification> ProcessJpapTriageAsync(
            IEnumerable<JpapTriage> jpapTriages,
            CancellationToken c)
        {
            var jpapTriageNotification = new TriageNotification()
            {
                RecordsRead = jpapTriages.Count(),
                RecordsLoaded = 0,
                RecordsFailed = 0,
                FailedImageFileNames = [],
                ErrorMessages = []
            };

            int recordCount = 0;

            foreach (var jpapTriage in jpapTriages)
            {
                try
                {
                    recordCount++;

                    var addresses = await _jpapTriageHelper.FindAddressesForJpapTriageAsync(
                        jpapTriage.PatientDemographicId,
                        jpapTriage.CarePathPatientId,
                        jpapTriage.DerivedPatientDateOfBirth,
                        jpapTriage.PatientLastName,
                        jpapTriage.PatientFirstName,
                        jpapTriage.PatientState).ConfigureAwait(false);

                    // If there are no existing addresses, create a new address, a new case, and add any Rx images as attachments to the case
                    if (!addresses.Any())
                    {
                        var addressId = await _jpapTriageHelper.CreateAddressFromJpapTriageAsync(jpapTriage, c).ConfigureAwait(false);
                        var caseId = await _jpapTriageHelper.CreateCaseFromJpapTriageAsync(addressId, jpapTriage, c).ConfigureAwait(false);
                        await _jpapTriageHelper.UploadJpapTriageImageFilesAndAttachToCaseAsync(caseId, jpapTriage.DerivedImages, c).ConfigureAwait(false);
                    }
                    else
                    {
                        foreach (var address in addresses)
                        {
                            await _jpapTriageHelper.UpdateAddressFromJpapTriageAsync(address, jpapTriage, c).ConfigureAwait(false);

                            var addressId = Convert.ToInt32(address.address_id);

                            var cases = await _jpapTriageHelper.FindCasesByAddressIdProgramAndStatusAsync(addressId, JPAP_PROGRAM_TYPE, caseStatus: ALL).ConfigureAwait(false);

                            // If there is an existing address but no case, create a new case, and add any Rx images as attachments to the case
                            if (cases.Count == 0)
                            {
                                var caseId = await _jpapTriageHelper.CreateCaseFromJpapTriageAsync(addressId, jpapTriage, c).ConfigureAwait(false);
                                await _jpapTriageHelper.UploadJpapTriageImageFilesAndAttachToCaseAsync(caseId, jpapTriage.DerivedImages, c).ConfigureAwait(false);
                            }
                            // If there is an existing address with one or more cases, update each case and add any Rx images as attachments
                            else
                            {
                                foreach (var caseRecord in cases)
                                {
                                    var caseId = Convert.ToInt32(caseRecord.case_id);
                                    await _jpapTriageHelper.UpdateCaseFromJpapTriageAsync(caseId, jpapTriage, c).ConfigureAwait(false);
                                    await _jpapTriageHelper.UploadJpapTriageImageFilesAndAttachToCaseAsync(caseId, jpapTriage.DerivedImages, c).ConfigureAwait(false);

                                    // If there are more than one existing cases, post an action to each to let the team know there are multiple cases
                                    if (cases.Count > 1 || addresses.ToList().Count > 1)
                                    {
                                        await _jpapTriageHelper.PostActionAsync("Multiple EPAO Cases", "triageexceptions", caseId, DateTime.Now, c)
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                    }

                    jpapTriageNotification.RecordsLoaded++;
                }
                catch (FileUploadException ex)
                {
                    jpapTriageNotification.FailedImageFileNames.Add(ex.FileName);
                    jpapTriageNotification.ErrorMessages.Add($"File upload failure occurred for record [{recordCount}], {ex.Message}");
                }
                catch (Exception ex)
                {
                    jpapTriageNotification.RecordsFailed++;
                    jpapTriageNotification.ErrorMessages.Add($"Failure occurred for record [{recordCount}], {ex.Message}");
                }
            }

            return jpapTriageNotification;
        }

        public async Task<TriageNotification> ProcessOncologyTriageAsync(
            IEnumerable<OncologyTriage> oncologyTriages,
            CancellationToken c)
        {
            var oncologyTriageNotification = new TriageNotification()
            {
                RecordsRead = oncologyTriages.Count(),
                RecordsLoaded = 0,
                RecordsFailed = 0,
                FailedImageFileNames = [],
                ErrorMessages = []
            };

            int recordCount = 0;

            foreach (var oncologyTriage in oncologyTriages)
            {
                try
                {
                    recordCount++;

                    var addresses = await _oncologyTriageHelper.FindAddressesForOncologyTriageAsync(
                        oncologyTriage.PatientDemographicId,
                        oncologyTriage.CarePathPatientId,
                        oncologyTriage.PatientDateOfBirth,
                        oncologyTriage.PatientLastName,
                        oncologyTriage.PatientFirstName);

                    if (!addresses.Any())
                    {
                        var addressId = await _oncologyTriageHelper.CreateAddressFromOncologyTriageAsync(oncologyTriage, c).ConfigureAwait(false);
                        var caseId = await _oncologyTriageHelper.CreateCaseFromOncologyTriageAsync(addressId, oncologyTriage, c).ConfigureAwait(false);
                        await _oncologyTriageHelper.UploadOncologyTriageImageFilesAndAttachToCaseAsync(caseId, oncologyTriage.DerivedImages, c).ConfigureAwait(false);
                    }
                    else
                    {
                        foreach (var address in addresses)
                        { 
                            await _oncologyTriageHelper.UpdateAddressFromOncologyTriageAsync(address, oncologyTriage, c).ConfigureAwait(false);
                            
                            var addressId = Convert.ToInt32(address.address_id);

                            var cases = await _oncologyTriageHelper.FindCasesByAddressIdProgramAndStatusAsync(addressId, DELAY_AND_DENIAL_PROGRAM_TYPE, caseStatus: OPEN).ConfigureAwait(false);

                            if (cases.Count == 0)
                            {
                                var caseId = await _oncologyTriageHelper.CreateCaseFromOncologyTriageAsync(addressId, oncologyTriage, c).ConfigureAwait(false);
                                int intCaseId = Convert.ToInt32(caseId);
                                await _oncologyTriageHelper.UploadOncologyTriageImageFilesAndAttachToCaseAsync(intCaseId, oncologyTriage.DerivedImages, c).ConfigureAwait(false);
                            }
                            else
                            {
                                foreach (var caseRecord in cases)
                                {
                                    var caseId = Convert.ToInt32(caseRecord.case_id);
                                    await _oncologyTriageHelper.UpdateCaseFromOncologyTriageAsync(caseId, oncologyTriage, c).ConfigureAwait(false);
                                    await _oncologyTriageHelper.UploadOncologyTriageImageFilesAndAttachToCaseAsync(caseId, oncologyTriage.DerivedImages, c).ConfigureAwait(false);

                                    if (addresses.ToList().Count > 1)
                                    {
                                        await _oncologyTriageHelper.PostActionAsync("Duplicate patient from triage", "triageexception", caseId, DateTime.Now, c)
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                    }
                    oncologyTriageNotification.RecordsLoaded++;
                }
                catch (FileUploadException ex)
                {
                    oncologyTriageNotification.FailedImageFileNames.Add(ex.FileName);
                    oncologyTriageNotification.ErrorMessages.Add($"File upload failure occurred for record [{recordCount}], {ex.Message}");
                }
                catch (Exception ex)
                {
                    oncologyTriageNotification.RecordsFailed++;
                    oncologyTriageNotification.ErrorMessages.Add($"Failure occurred for record [{recordCount}], {ex.Message}");
                }
            }
            return oncologyTriageNotification;
        }

        public async Task<TriageNotification> ProcessOncologyVoucherTriageAsync(
            IEnumerable<OncologyVoucherTriage> oncologyVoucherTriages,
            CancellationToken c)
        {
            var oncologyVoucherTriageNotification = new TriageNotification()
            {
                RecordsRead = oncologyVoucherTriages.Count(),
                RecordsLoaded = 0,
                RecordsFailed = 0,
                FailedImageFileNames = [],
                ErrorMessages = []
            };

            int recordCount = 0;

            foreach (var oncologyVoucherTriage in oncologyVoucherTriages)
            {
                try
                {
                    recordCount++;

                    var addresses = await _oncologyVoucherTriageHelper.FindAddressesForOncologyVoucherTriageAsync(
                        oncologyVoucherTriage.CarePathPatientId,
                        oncologyVoucherTriage.PatientDemographicId,
                        oncologyVoucherTriage.PatientState,
                        oncologyVoucherTriage.PatientDateOfBirth,
                        oncologyVoucherTriage.PatientLastName,
                        oncologyVoucherTriage.PatientFirstName).ConfigureAwait(false);

                    // If there are no existing addresses, create a new address, a new case, and add any Rx images as attachments to the case
                    if (!addresses.Any())
                    {
                        var addressId = await _oncologyVoucherTriageHelper.CreateAddressFromOncologyVoucherTriageAsync(oncologyVoucherTriage, c).ConfigureAwait(false);
                        var caseId = await _oncologyVoucherTriageHelper.CreateCaseFromOncologyVoucherTriageAsync(addressId, oncologyVoucherTriage, c).ConfigureAwait(false);
                        await _oncologyVoucherTriageHelper.UploadOncologyVoucherTriageImageFilesAndAttachToCaseAsync(caseId, oncologyVoucherTriage.DerivedImages, c).ConfigureAwait(false);
                    }
                    else
                    {
                        foreach (var address in addresses)
                        {
                            await _oncologyVoucherTriageHelper.UpdateAddressFromOncologyVoucherTriageAsync(address, oncologyVoucherTriage, c).ConfigureAwait(false);

                            var addressId = Convert.ToInt32(address.address_id);

                            var cases = await _oncologyVoucherTriageHelper.FindCasesByAddressIdProgramAndStatusAsync(addressId, DELAY_AND_DENIAL_VOUCHER_PROGRAM_TYPE, caseStatus: ALL).ConfigureAwait(false);

                            // If there is an existing address but no case, create a new case, and add any Rx images as attachments to the case
                            if (cases.Count == 0)
                            {
                                var caseId = await _oncologyVoucherTriageHelper.CreateCaseFromOncologyVoucherTriageAsync(addressId, oncologyVoucherTriage, c).ConfigureAwait(false);
                                await _oncologyVoucherTriageHelper.UploadOncologyVoucherTriageImageFilesAndAttachToCaseAsync(caseId, oncologyVoucherTriage.DerivedImages, c).ConfigureAwait(false);
                            }
                            // If there is an existing address with one or more cases, update each case and add any Rx images as attachments
                            else
                            {
                                foreach (var caseRecord in cases)
                                {
                                    var caseId = Convert.ToInt32(caseRecord.case_id);
                                    await _oncologyVoucherTriageHelper.UpdateCaseFromOncologyVoucherTriageAsync(caseId, oncologyVoucherTriage, c).ConfigureAwait(false);
                                    await _oncologyVoucherTriageHelper.UploadOncologyVoucherTriageImageFilesAndAttachToCaseAsync(caseId, oncologyVoucherTriage.DerivedImages, c).ConfigureAwait(false);

                                    // If there are more than one existing cases, post an action to each to let the team know there are multiple cases
                                    if (cases.Count > 1 || addresses.ToList().Count > 1)
                                    {
                                        await _oncologyVoucherTriageHelper.PostActionAsync("Multiple EPAO Cases", "triageexceptions", caseId, DateTime.Now, c)
                                            .ConfigureAwait(false);
                                    }
                                }
                            }
                        }
                    }

                    oncologyVoucherTriageNotification.RecordsLoaded++;
                }
                catch (FileUploadException ex)
                {
                    oncologyVoucherTriageNotification.FailedImageFileNames.Add(ex.FileName);
                    oncologyVoucherTriageNotification.ErrorMessages.Add($"File upload failure occurred for record [{recordCount}], {ex.Message}");
                }
                catch (Exception ex)
                {
                    oncologyVoucherTriageNotification.RecordsFailed++;
                    oncologyVoucherTriageNotification.ErrorMessages.Add($"Failure occurred for record [{recordCount}], {ex.Message}");
                }
            }

            return oncologyVoucherTriageNotification;
        }


        public async Task<EligibilityNotification> ProcessJpapEligibilityAsync(
            IEnumerable<JpapEligibilityRow> jpapEligibilityRows,
            CancellationToken c)
        {
            var jpapEligibilityNotification = new EligibilityNotification()
            {
                RecordsRead = jpapEligibilityRows.Count(),
                RecordsLoaded = 0,
                RecordsFailed = 0,
                RecordsSkipped = 0,
                FailedFileNames = [],
                ErrorMessages = []
            };

            int recordCount = 0;

            foreach (var jpapEligibilityRow in jpapEligibilityRows)
            {
                try
                {
                    recordCount++;

                    var cases = await _jpapEligibilityHelperImp.FindCasesByProgramTypePatientIdAndDob(JPAP_PROGRAM_TYPE, jpapEligibilityRow.PatientId, jpapEligibilityRow.DateOfBirth, caseStatus: ALL).ConfigureAwait(false);

                    if (cases.Count == 0)
                    {
                        await _jpapEligibilityHelperImp.CreateBlankCase(jpapEligibilityRow, c).ConfigureAwait(false);
                        jpapEligibilityNotification.RecordsSkipped++;
                    }
                    else
                    {
                        foreach (var caseRecord in cases)
                        {
                            int caseId = Convert.ToInt32(caseRecord.case_id);
                            await _jpapEligibilityHelperImp.UpdateCaseFromJpapEligibilityAsync(caseId, jpapEligibilityRow, c).ConfigureAwait(false);
                            jpapEligibilityNotification.RecordsLoaded++;
                        }
                    }
                }
                catch (FileUploadException ex)
                {
                    jpapEligibilityNotification.FailedFileNames.Add(ex.FileName);
                    jpapEligibilityNotification.ErrorMessages.Add($"File upload failure occurred for record [{recordCount}], {ex.Message}");
                }
                catch (Exception ex)
                {
                    jpapEligibilityNotification.RecordsFailed++;
                    jpapEligibilityNotification.ErrorMessages.Add($"Failure occurred for record [{recordCount}], {ex.Message}");
                }
            }

            return jpapEligibilityNotification;
        }

        public async Task<TriageNotification> ProcessVerificationOfBenefitsAsync(
            IEnumerable<VerificationOfBenefits> verificationOfBenefits,
            string file,
            CancellationToken c)
        {
            var verificationOfBenefitsNotification = new TriageNotification()
            {
                RecordsRead = verificationOfBenefits.Count(),
                RecordsLoaded = 0,
                RecordsFailed = 0,
                FailedImageFileNames = [],
                ErrorMessages = []
            };

            int recordCount = 0;

            foreach (var verificationOfBenefit in verificationOfBenefits)
            {
                try
                {
                    recordCount++;

                    var cases = await _verificationOfBenefitsHelper.FindCasesByProgramTypePatientIdAndDob(DELAY_AND_DENIAL_PROGRAM_TYPE, verificationOfBenefit.PatientBirthYear, OPEN, verificationOfBenefit.PatientIdentifiers).ConfigureAwait(false);

                    if (cases.Count == 0)
                    {
                        var caseId = await _verificationOfBenefitsHelper.CreateCaseFromVerificationOfBenefitsAsync(verificationOfBenefit, c).ConfigureAwait(false);
                        await _verificationOfBenefitsHelper.PostActionAsync(CaseNotFoundAction, ReferredToUserCode, caseId, DateTime.Now, c).ConfigureAwait(false);

                        if (!file.Contains(RerunStatus))
                        {
                            var guid = Guid.NewGuid();
                            string fileName = file.Replace(FileNameSuffix, "") + $"_{RerunStatus}_{guid}{FileNameSuffix}";
                            await _verificationOfBenefitsHelper.AppendDataToNewFile(file, fileName).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        foreach (var caseRecord in cases)
                        {
                            int caseId = Convert.ToInt32(caseRecord.case_id);
                            await _verificationOfBenefitsHelper.UpdateCaseFromVerificationOfBenefitsAsync(caseRecord, caseId, verificationOfBenefit, c).ConfigureAwait(false);
                            await _verificationOfBenefitsHelper.UploadVerificationOfBenefitsImageFilesAndAttachToCaseAsync(caseId, verificationOfBenefit.DerivedImages, c).ConfigureAwait(false);

                            if (cases.Count > 1)
                            {
                                await _verificationOfBenefitsHelper.PostActionAsync(ActionTypeCodeMultipleCasesFound, ReferredToUserCode, caseId, DateTime.Now, c).ConfigureAwait(false);
                            }
                        }
                    }
                    verificationOfBenefitsNotification.RecordsLoaded++;
                }
                catch (FileUploadException ex)
                {
                    verificationOfBenefitsNotification.FailedImageFileNames.Add(ex.FileName);
                    verificationOfBenefitsNotification.ErrorMessages.Add($"File upload failure occurred for record [{recordCount}], {ex.Message}");
                }
                catch (Exception ex)
                {
                    verificationOfBenefitsNotification.RecordsFailed++;
                    verificationOfBenefitsNotification.ErrorMessages.Add($"Failure occurred for record [{recordCount}], {ex.Message}");
                }
            }
            return verificationOfBenefitsNotification;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Case>> GetJpapOutboundStatusAsync(
            DateTime startDateTime,
            DateTime endDateTime,
            CancellationToken c)
        {
            List<Case> returnCaseList = new List<Case>();
            SearchCase searchCase = new SearchCase()
            {
                company_id = CompanyId,
                b07_code = JPAP_PROGRAM_TYPE,
                case_status = OPEN, 
                AddressList = new SearchAddressList()
                {
                    Address =
                        [
                            new SearchAddress
                            {
                                address_type_code = PATIENT_ADDRESS_TYPE
                            }
                        ]
                },
            };

            IEnumerable<Case> caseResultsList = [];
            List<string> caseIdExclusionList = [];
            var requestList = _jpapDispenseAndStatusHelper.GetOutboundStatusRequests(startDateTime, endDateTime);

            int requestNbr = 1;
            foreach (var request in requestList)
            {
                _logger.LogDebug($"Processing request [{requestNbr}] out of a total of [{requestList.Count()}] requests.");
                caseIdExclusionList = new List<string>(); //Reset the exclusion list for each request so that we can merge issues found in various requests.
                ICollection<Filter> requestFilters = request;
                int filterSequence = request.LastOrDefault()?.filter_seq ?? 0;

                do
                {
                    _logger.LogDebug($"Looping through results from request nbr [{requestNbr}] using [{filterSequence}] Filters.");

                    if (caseIdExclusionList.Count > 0)
                    {
                        //The API only returns up to 250 results per request, so you need to run the same request repeatedly to page through all of the results.
                        //Add a filter to exclude the prior paged results so that we effectively get the next page of results.
                        filterSequence++;
                        requestFilters.Add(new Filter()
                        {
                            filter_seq = filterSequence,
                            selection_operator = SelectionOperator.NOTIN,
                            selection_category_id = "BF1",
                            selection_code = string.Join(";", caseIdExclusionList)
                        });
                    }

                    caseResultsList = await RunSearchRequestAsync(requestFilters, searchCase, c).ConfigureAwait(false);
                    caseIdExclusionList = caseResultsList.Select(c => c.case_id).ToList();
                    returnCaseList.AddRange(caseResultsList);
                } while (caseResultsList.Count() > 0);
            }

            return returnCaseList;
        }
    }
}
