using AddressServiceWrapper;
using CaseServiceWrapper;
using CaseStreamServiceWrapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly.Retry;
using Polly;
using System.Drawing;
using System.Net.Sockets;
using Library.EmplifiInterface.DataModel;
using Library.McKessonDWInterface.DataModel;
using Library.EmplifiInterface.Exceptions;

namespace Library.EmplifiInterface.Helper;

public class AstuteAdherenceHelperImp : IAstuteAdherenceDispenseHelper
{
    private readonly ILogger<AstuteAdherenceHelperImp> _logger;
    private readonly IOptions<EmplifiConfig> _config;
    private readonly ICaseService _caseService;
    private readonly IAddressService _addressService;
    private readonly ICaseStreamService _caseStreamService;
    private readonly AsyncRetryPolicy RetryPolicy;

    private const string TRADITIONAL_PROGRAM_TYPE = "Traditional Specialty";
    private const string DISPENSE_LOGIC_MISMATCH = "Dispense Logic Mismatch";
    private const string PATIENT_NOT_FOUND = "patient not found";
    private const string NO_DISPENSE = "NO DISPENSE";
    private const string SHIPPED_PATIENT_STATUS = "A1=Shipped";
    private const string SHIPPED_WORKFLOW_STATUS = "Call Complete-Shipped";
    private const string CLINICAL_QUESTION_DECLINED = "Clinical/Qol - Declined";

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
    private const string PROGRAM_HEADER_MISMATCH = "Program Header Mismatch";
    private const string ADMIN_QUEUE_USER = "adminqueue";
    private const string ONCOLOGY_PROGRAM_TYPE = "Janssen Oncology Voucher";
    private const string MULTIPLE_CASES_FOUND = "Dispense Applied To Multiple Cases";
    private const string XARELTO_PROGRAM_TYPE = "Janssen Select - Xarelto";
    private const string NEW_REFERRAL_PATIENT_STATUS = "P1=New Referral";

    private readonly string[] programHeadersToExclude = new string[] { "RPh Only", "Patient Outreach" };
    private readonly string[] jpapReshipPlanCodes = new string[] { "OTHPAR" };
    private readonly string[] setAddressPrograms = new string[] { XARELTO_PROGRAM_TYPE, JPAP_PROGRAM_TYPE, ONCOLOGY_PROGRAM_TYPE, DELAY_AND_DENIAL_PROGRAM_TYPE };
    private readonly string[] setNewReferralStatusPrograms = new string[] { JPAP_PROGRAM_TYPE, DELAY_AND_DENIAL_PROGRAM_TYPE };

    private bool ShouldSendExceptionEmail { get; set; } = false;

    public AstuteAdherenceHelperImp(
        ILogger<AstuteAdherenceHelperImp> logger,
        IOptions<EmplifiConfig> config,
        ICaseService caseService,
        IAddressService addressService,
        ICaseStreamService caseStreamService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _caseService = caseService ?? throw new ArgumentNullException(nameof(caseService));
        _addressService = addressService ?? throw new ArgumentNullException(nameof(addressService));
        _caseStreamService = caseStreamService ?? throw new ArgumentNullException(nameof(caseStreamService));

        RetryPolicy = Policy
            .Handle<HttpRequestException>(e => e.StatusCode == System.Net.HttpStatusCode.InternalServerError)
            .Or<SocketException>() // Network error
            .WaitAndRetryAsync(
                retryCount: 10,
                retryAttempt => TimeSpan.FromSeconds(30 * retryAttempt), // retry after 30s, 60s, ... 5 minutes
                onRetry: (ex, waitTime) => _logger.LogWarning(ex, $"Retrying in {waitTime.Seconds} seconds")
            );
    }

    /// <inheritdoc/>
    public CaseGetRequest CreateCaseGetRequest(
        string programType)
    {
        return new CaseGetRequest()
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = CaseGetType.Get,
            Case = new Case()
            {
                company_id = "SYS",
                b07_code = programType,
                case_status = OPEN
            },
            ResponseFormat = new ResponseFormat()
            {
                CaseList = new CaseListFormat()
                {
                    Case = new CaseFormat()
                    {
                        AllAttributes = CaseServiceWrapper.TrueFalseType.Item1,
                        AddressList = new CaseServiceWrapper.AddressListFormat()
                        {
                            Address = new CaseServiceWrapper.AddressFormat()
                            {
                                AllAttributes = CaseServiceWrapper.TrueFalseType.Item1,
                                PhoneList = new CaseServiceWrapper.PhoneFormat[]
                                {
                                        new CaseServiceWrapper.PhoneFormat()
                                        {
                                            AllAttributes = CaseServiceWrapper.TrueFalseType.Item1
                                        }
                                }
                            }
                        },
                        IssueList = new IssueListFormat()
                        {
                            Issue = new IssueFormat()
                            {
                                AllAttributes = CaseServiceWrapper.TrueFalseType.Item1
                            }
                        }
                    }
                }
            }
        };
    }

    /// <inheritdoc />
    public async Task ReleaseCaseAsync(
        EmplifiRecordReportingStatus successfullyExportedRecord,
        CancellationToken c)
    {
        var request = new CaseUserListRequest()
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = CaseUserListType.Update,
            CaseUser = new CaseUser()
            {
                APCWEditMode = CaseServiceWrapper.APCWEditModeType.Delete,
                case_id = Convert.ToInt32(successfullyExportedRecord.CaseId),
                company_id = "SYS",
                system_user_id = _config.Value.Username
            }
        };

        CaseUserListResponse response = await RetryPolicy.ExecuteAsync(() =>
            _caseService.ReleaseCaseAsync(request)).ConfigureAwait(false);

        if (response != null)
        {
            if (response.Valid == CaseServiceWrapper.ValidState.Ok)
            {
                successfullyExportedRecord.CaseWasReleasedAfterUpdateApiCall = true;
            }
            else
            {
                _logger.LogDebug($"API to release CaseId [{successfullyExportedRecord.CaseId}], IssueSeq [{successfullyExportedRecord.IssueSeq}] response status: [{response.Valid}], and message is: [{string.Join(',', response.MessageList.Select(m => m.Text))}]");
            }
        }
    }

    /// <summary>
    /// Find open cases in CRM application that match the dispense data
    /// </summary>
    /// <param name = "dispense" ></ param >
    /// < returns ></ returns >
    public async Task<IEnumerable<Case>> FindOpenCasesAsync(
        AstuteAdherenceDispenseReportRow dispense,
        CancellationToken c)
    {
        IEnumerable<Case> temp;

        temp = await FindCaseUsingPatientIdDobProgramTypeAsync(dispense, c).ConfigureAwait(false);
        if (temp.Any())
        {
            return temp;
        }

        temp = await FindCaseUsingPatientNameDobProgramTypeAsync(dispense, c).ConfigureAwait(false);
        if (temp.Any())
        {
            return temp;
        }

        temp = await FindCaseUsingPatientNamePidProgramTypeAsync(dispense, c).ConfigureAwait(false);
        if (temp.Any())
        {
            return temp;
        }

        temp = await FindCaseUsingEnterpriseRxPatientNumProgramTypeAsync(dispense, c).ConfigureAwait(false);
        if (temp.Any())
        {
            return temp;
        }

        dispense.PatientLookupStatus = PATIENT_NOT_FOUND;
        return [];
    }

    /// <summary>
    /// Apply the business rules for matching and updating CRM cases for the dispense
    /// </summary>
    /// <param name="dispense">The dispense record being processed</param>
    /// <param name="crmCases">The list of CRM cases found for the dispense</param>
    public async Task<List<string>> ProcessCasesAsync(
        AstuteAdherenceDispenseReportRow dispense,
        IEnumerable<Case> crmCases,
        CancellationToken c)
    {
        var caseIdList = new List<int>();
        List<string> exceptions = new List<string>();

        foreach (var caseRecord in crmCases)
        {
            try
            {
                // For re-run purposes, skip the dispense if it was already applied to the case
                var issuesMatchingDispense = caseRecord.IssueList.Issue
                    .Where(x => x.c08_code == dispense.StoreNumber.ToString()
                        && x.c11_code == dispense.rawDataRow.RxNumber
                        && DateTime.TryParse(x.c15_code, out DateTime soldDate)
                        && soldDate >= dispense.rawDataRow.SoldDate
                        && !(x.product_code is null) && !string.IsNullOrEmpty(dispense.BaseProductName)
                        && x.product_code.Contains(dispense.BaseProductName));

                if (issuesMatchingDispense.Count() > 0)
                {
                    dispense.CasesMatchedInCrm++;
                    continue;
                }

                int? addressId = null;
                var caseId = Convert.ToInt32(caseRecord.case_id);
                var patientAddresses = caseRecord.AddressList?.Address?.Where(x => x.address_type_code == PATIENT_ADDRESS_TYPE);
                if (patientAddresses != null && patientAddresses.Any())
                {
                    addressId = Convert.ToInt32(patientAddresses.First().address_id);
                }

                // Program-specific business rules for updating the case
                switch (dispense.ProgramType)
                {
                    case TRADITIONAL_PROGRAM_TYPE:
                        var traditionalIssues = caseRecord.IssueList.Issue
                            .Where(x => x.c88_code == OPEN &&
                            !string.IsNullOrEmpty(x.product_code) &&
                            !string.IsNullOrEmpty(dispense.BaseProductName) &&
                            x.product_code.Contains(dispense.BaseProductName) &&
                            !programHeadersToExclude.Contains(x.c47_code, StringComparer.OrdinalIgnoreCase));

                        if (traditionalIssues.Count() > 0)
                        {
                            var mostRecentIssue = traditionalIssues.OrderBy(s => Convert.ToInt32(s.issue_seq)).Last();
                            var issueSequence = Convert.ToInt32(mostRecentIssue.issue_seq);
                            if (string.IsNullOrEmpty(mostRecentIssue.c15_code))
                            {
                                caseIdList.Add(await UpdateCaseAsync(dispense: dispense, caseId: caseId, addressId: addressId, issueSequence: issueSequence, c97_code: mostRecentIssue.c97_code, c).ConfigureAwait(false));
                            }
                            else
                            {
                                await UpdateIssueAsync(caseId, issueSequence, SHIPPED_WORKFLOW_STATUS, CLOSED, c).ConfigureAwait(false);
                                caseIdList.Add(await UpdateCaseAsync(dispense: dispense, caseId: caseId, addressId: addressId, issueSequence: null, c97_code: string.Empty, c).ConfigureAwait(false));
                            }
                        }
                        else
                        {
                            caseIdList.Add(await UpdateCaseAsync(dispense: dispense, caseId: caseId, addressId: addressId, issueSequence: null, c97_code: string.Empty, c).ConfigureAwait(false));
                        }
                        break;
                    case DELAY_AND_DENIAL_PROGRAM_TYPE:
                        var delayAndDenialIssues = caseRecord.IssueList.Issue
                            .Where(x => x.c88_code == OPEN &&
                            !string.IsNullOrEmpty(x.product_code) &&
                            !string.IsNullOrEmpty(dispense.BaseProductName) &&
                            (x.product_code.Contains(dispense.BaseProductName) &&
                            !programHeadersToExclude.Contains(x.c47_code, StringComparer.OrdinalIgnoreCase)));

                        if (delayAndDenialIssues.Count() > 0)
                        {
                            var mostRecentIssue = delayAndDenialIssues.OrderBy(s => Convert.ToInt32(s.issue_seq)).Last();
                            var issueSequence = Convert.ToInt32(mostRecentIssue.issue_seq);
                            if (string.IsNullOrEmpty(mostRecentIssue.c15_code))
                            {
                                caseIdList.Add(await UpdateCaseAsync(dispense: dispense, caseId: caseId, addressId: addressId, issueSequence: issueSequence, c97_code: mostRecentIssue.c97_code, c).ConfigureAwait(false));
                            }
                            else
                            {
                                await UpdateIssueAsync(caseId, issueSequence, SHIPPED_WORKFLOW_STATUS, CLOSED, c).ConfigureAwait(false);
                                caseIdList.Add(await UpdateCaseAsync(dispense: dispense, caseId: caseId, addressId: addressId, issueSequence: null, c97_code: string.Empty, c).ConfigureAwait(false));
                            }
                        }
                        break;
                    case JPAP_PROGRAM_TYPE:
                        if (jpapReshipPlanCodes.Contains(dispense.rawDataRow.PlanCode, StringComparer.OrdinalIgnoreCase))
                        {
                            var jpapDrugIssues = caseRecord.IssueList.Issue
                                .Where(x => !string.IsNullOrEmpty(x.c11_code) &&
                                x.c11_code == dispense.rawDataRow.RxNumber &&
                                !string.IsNullOrEmpty(x.c47_code) &&
                                x.c47_code.ToUpper() == (dispense.ProgramHeader?.ToUpper()) &&
                                !string.IsNullOrEmpty(x.c09_code) &&
                                x.c09_code == (dispense.FillNumber - 1).ToString());

                            if (jpapDrugIssues.Count() > 0)
                            {
                                var mostRecentIssue = jpapDrugIssues.OrderBy(s => Convert.ToInt32(s.issue_seq)).Last();
                                var issueSequence = Convert.ToInt32(mostRecentIssue.issue_seq);
                                caseIdList.Add(await UpdateCaseAsync(dispense: dispense, caseId: caseId, addressId: addressId, issueSequence: issueSequence, c97_code: mostRecentIssue.c97_code, c).ConfigureAwait(false));
                            }
                            else
                            {
                                await PostActionAsync(PROGRAM_HEADER_MISMATCH, ADMIN_QUEUE_USER, caseId, DateTime.Now, c).ConfigureAwait(false);
                                ShouldSendExceptionEmail = true;
                            }
                        }
                        else
                        {
                            var jpapDrugIssues = caseRecord.IssueList.Issue
                                .Where(x => !string.IsNullOrEmpty(x.product_code) &&
                                !string.IsNullOrEmpty(dispense.BaseProductName) &&
                                x.product_code.Contains(dispense.BaseProductName) &&
                                !programHeadersToExclude.Contains(x.c47_code, StringComparer.OrdinalIgnoreCase));

                            if (jpapDrugIssues.Count() > 0)
                            {
                                var mostRecentIssue = jpapDrugIssues.OrderBy(s => Convert.ToInt32(s.issue_seq)).Last();
                                var issueSequence = Convert.ToInt32(mostRecentIssue.issue_seq);
                                if (mostRecentIssue.c88_code == CLOSED)
                                {
                                    caseIdList.Add(await UpdateCaseAsync(dispense: dispense, caseId: caseId, addressId: addressId, issueSequence: null, c97_code: string.Empty, c).ConfigureAwait(false));
                                }
                                else if (string.IsNullOrEmpty(mostRecentIssue.c15_code))
                                {
                                    caseIdList.Add(await UpdateCaseAsync(dispense: dispense, caseId: caseId, addressId: addressId, issueSequence: issueSequence, c97_code: mostRecentIssue.c97_code, c).ConfigureAwait(false));
                                }
                                else
                                {
                                    await UpdateIssueAsync(caseId, issueSequence, SHIPPED_WORKFLOW_STATUS, CLOSED, c).ConfigureAwait(false);
                                    caseIdList.Add(await UpdateCaseAsync(dispense: dispense, caseId: caseId, addressId: addressId, issueSequence: null, c97_code: string.Empty, c).ConfigureAwait(false));
                                }
                            }
                            else
                            {
                                await PostActionAsync(PROGRAM_HEADER_MISMATCH, ADMIN_QUEUE_USER, caseId, DateTime.Now, c).ConfigureAwait(false);
                                ShouldSendExceptionEmail = true;
                            }
                        }
                        break;
                    case ONCOLOGY_PROGRAM_TYPE:
                        var oncologyIssues = caseRecord.IssueList.Issue
                            .Where(x => x.c88_code == OPEN &&
                            !string.IsNullOrEmpty(x.product_code) &&
                            x.product_code.ToUpper() == dispense?.rawDataRow.ProductName?.ToUpper() &&
                            !programHeadersToExclude.Contains(x.c47_code, StringComparer.OrdinalIgnoreCase) &&
                            string.IsNullOrEmpty(x.c15_code));
                        if (oncologyIssues.Count() > 0)
                        {
                            var mostRecentIssue = oncologyIssues.OrderBy(s => Convert.ToInt32(s.issue_seq)).Last();
                            var issueSequence = Convert.ToInt32(mostRecentIssue.issue_seq);
                            caseIdList.Add(await UpdateCaseAsync(dispense: dispense, caseId: caseId, addressId: addressId, issueSequence: issueSequence, c97_code: mostRecentIssue.c97_code, c).ConfigureAwait(false));
                        }
                        else
                        {
                            await PostActionAsync(PROGRAM_HEADER_MISMATCH, ADMIN_QUEUE_USER, caseId, DateTime.Now, c).ConfigureAwait(false);
                            ShouldSendExceptionEmail = true;
                        }
                        break;
                    default:
                        // Xarelto dispenses will be handled in this section
                        var xareltoIssues = caseRecord.IssueList.Issue.Where(x => !programHeadersToExclude.Contains(x.c47_code, StringComparer.OrdinalIgnoreCase));
                        if (xareltoIssues.Count() > 0)
                        {
                            var mostRecentIssue = xareltoIssues.OrderBy(s => Convert.ToInt32(s.issue_seq)).Last();
                            var issueSequence = Convert.ToInt32(mostRecentIssue.issue_seq);

                            if (mostRecentIssue.c88_code == OPEN &&
                                string.IsNullOrEmpty(mostRecentIssue.c09_code) // fill number
                                && string.IsNullOrEmpty(mostRecentIssue.c15_code)) // sold date
                            {
                                caseIdList.Add(await UpdateCaseAsync(dispense: dispense, caseId: caseId, addressId: addressId, issueSequence: issueSequence, c97_code: mostRecentIssue.c97_code, c).ConfigureAwait(false));
                            }
                            else
                            {
                                caseIdList.Add(await UpdateCaseAsync(dispense: dispense, caseId: caseId, addressId: addressId, issueSequence: null, c97_code: string.Empty, c).ConfigureAwait(false));
                            }
                        }
                        else
                        {
                            await PostActionAsync(PROGRAM_HEADER_MISMATCH, ADMIN_QUEUE_USER, caseId, DateTime.Now, c).ConfigureAwait(false);
                            ShouldSendExceptionEmail = true;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                exceptions.Add(ex.Message);
            }
        }

        // If more than one case was updated for the dispense, post an action to each
        if (caseIdList.Count() > 1)
        {
            foreach (var caseId in caseIdList)
            {
                try
                {
                    await PostActionAsync(MULTIPLE_CASES_FOUND, ADMIN_QUEUE_USER, caseId, DateTime.Now, c).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex.Message);
                }
            }
        }

        return exceptions;
    }

    /// <summary>
    /// Update a case in the CRM with the dispense data
    /// </summary>
    /// <param name="dispense">The dispense record being processed</param>
    /// <param name="caseId">The case ID</param>
    /// <param name="addressId">The address ID</param>
    /// <param name="issueSequence">The issue sequence to update</param>
    /// <returns>The case ID that was updated, if successful</returns>
    private async Task<int> UpdateCaseAsync(
        AstuteAdherenceDispenseReportRow dispense,
        int caseId,
        int? addressId,
        int? issueSequence,
        string? c97_code,
        CancellationToken c)
    {
        dispense.CasesMatchedInCrm++;

        int followUpIssuesCreatedCount = 0;

        var request = new CaseListUpdateRequest
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = CaseListUpdateType.Update,
            Case = new Case[]
            {
                    new Case()
                    {
                        APCWEditMode = CaseServiceWrapper.APCWEditModeType.Modified,
                        company_id = "SYS",
                        case_id = caseId.ToString(),
                    }
            },
            ResponseFormat = new ResponseFormat()
            {
                CaseList = new CaseListFormat()
                {
                    Case = new CaseFormat()
                    {
                        AllAttributes = CaseServiceWrapper.TrueFalseType.Item1,
                        IssueList = new IssueListFormat()
                        {
                            Issue = new IssueFormat()
                            {
                                AllAttributes = CaseServiceWrapper.TrueFalseType.Item1
                            }
                        }
                    }
                }
            }
        };

        if (setAddressPrograms.Contains(dispense.ProgramType, StringComparer.OrdinalIgnoreCase))
        {
            request.Case[0].b20_code = dispense.rawDataRow.ShipAddress1;
            request.Case[0].b19_code = dispense.rawDataRow.ShipAddress2;
            request.Case[0].b06_code = dispense.rawDataRow.ShipCity;
            request.Case[0].b59_code = dispense.rawDataRow.ShipState;
            request.Case[0].b60_code = dispense.rawDataRow.ShipZipCode;
        }

        if (!(addressId is null))
        {
            request.Case[0].AddressList = new AddressList()
            {
                Address = new CaseServiceWrapper.Address[]
                {
                        new CaseServiceWrapper.Address()
                        {
                            APCWEditMode = CaseServiceWrapper.APCWEditModeType.Modified,
                            company_id = "SYS",
                            address_id = addressId.ToString(),
                            active = CaseServiceWrapper.YesNo.Y,
                            a35_code = dispense.rawDataRow.PatientNum
                        }
                }
            };

            if (setAddressPrograms.Contains(dispense.ProgramType, StringComparer.OrdinalIgnoreCase))
            {
                request.Case[0].AddressList.Address[0].a42_code = dispense.rawDataRow.PrescriberFirstName;
                request.Case[0].AddressList.Address[0].a43_code = dispense.rawDataRow.PrescriberLastName;
                request.Case[0].AddressList.Address[0].a44_code = dispense.rawDataRow.PrescriberAddress1;
                request.Case[0].AddressList.Address[0].a45_code = dispense.rawDataRow.PrescriberAddress2;
                request.Case[0].AddressList.Address[0].a47_code = dispense.rawDataRow.PrescriberCity;
                request.Case[0].AddressList.Address[0].a48_code = dispense.rawDataRow.PrescriberState;
                request.Case[0].AddressList.Address[0].a49_code = dispense.rawDataRow.PrescriberZip;
                request.Case[0].AddressList.Address[0].a50_code = dispense.rawDataRow.PrescriberPhone;
                request.Case[0].AddressList.Address[0].a56_code = dispense.rawDataRow.PrescriberNpi;
                request.Case[0].AddressList.Address[0].a54_code = dispense.rawDataRow.PrescriberDea;
            }
        }

        var issueList = new List<Issue>();

        if (issueSequence is null)
        {
            // Add a new dispense issue
            issueList.Add(new Issue()
            {
                APCWEditMode = CaseServiceWrapper.APCWEditModeType.New,
                product_code = dispense.CrmProductName,
                c06_code = dispense.rawDataRow.TotalRefillsRemaining.ToString(),
                c07_code = dispense.rawDataRow.Quantity.ToString(),
                c08_code = dispense.StoreNumber.ToString(),
                c09_code = dispense.FillNumber.ToString(),
                c10_code = dispense.rawDataRow.WrittenDate.ToString(),
                c11_code = dispense.rawDataRow.RxNumber,
                c12_code = dispense.SoldDateTime.ToString(),
                c15_code = dispense.SoldDateTime.ToString(),
                c16_code = (string.IsNullOrEmpty(dispense.rawDataRow.ProductName) || dispense.rawDataRow.ProductName.Contains(NO_DISPENSE)) ? null : SHIPPED_PATIENT_STATUS,
                c19_code = SHIPPED_WORKFLOW_STATUS,
                c21_code = dispense.rawDataRow.DaysSupply.ToString(),
                c26_code = DateTime.Now.ToString(), // Patient status change date/time
                c39_code = dispense.rawDataRow.DrugNDC,
                c40_code = dispense.rawDataRow.RxFillSequence.ToString(),
                c42_code = dispense.rawDataRow.DateOfService.ToString(),
                c43_code = dispense.rawDataRow.PersonCode,
                c46_code = dispense.rawDataRow.ShipState,
                c47_code = dispense.ProgramHeader,
                c53_code = jpapReshipPlanCodes.Contains(dispense.rawDataRow.PlanCode, StringComparer.OrdinalIgnoreCase) ? dispense.rawDataRow.SoldDate.ToString() : null,
                c56_code = dispense.rawDataRow.PlanCode,
                c75_code = CLINICAL_QUESTION_DECLINED,
                c82_code = dispense.rawDataRow.OrderNumber,
                c83_code = dispense.rawDataRow.TrackingNumber,
                c84_code = dispense.rawDataRow.CourierName,
                c86_code = dispense.rawDataRow.TotalRefillsAllowed.ToString(),
                c88_code = CLOSED,
                c89_code = ISSUE_SOURCE,
                ca8_code = DateTime.Now.ToString()
            });
        }
        else
        {
            // Update an existing dispense issue
            issueList.Add(new Issue()
            {
                APCWEditMode = CaseServiceWrapper.APCWEditModeType.Modified,
                issue_seq = issueSequence.ToString(),
                product_code = dispense.CrmProductName,
                c06_code = dispense.rawDataRow.TotalRefillsRemaining.ToString(),
                c07_code = dispense.rawDataRow.Quantity.ToString(),
                c08_code = dispense.StoreNumber.ToString(),
                c09_code = dispense.FillNumber.ToString(),
                c10_code = dispense.rawDataRow.WrittenDate.ToString(),
                c11_code = dispense.rawDataRow.RxNumber,
                c12_code = dispense.SoldDateTime.ToString(),
                c15_code = dispense.SoldDateTime.ToString(),
                c16_code = (string.IsNullOrEmpty(dispense.rawDataRow.ProductName) || dispense.rawDataRow.ProductName.Contains(NO_DISPENSE)) ? null : SHIPPED_PATIENT_STATUS,
                c19_code = SHIPPED_WORKFLOW_STATUS,
                c21_code = dispense.rawDataRow.DaysSupply.ToString(),
                c25_code = "", // clear Fill By Date
                c26_code = DateTime.Now.ToString(), // Patient status change date/time
                c35_code = "", // clear Follow-Up Date
                c37_code = "", // clear Follow-Up Time
                c39_code = dispense.rawDataRow.DrugNDC,
                c40_code = dispense.rawDataRow.RxFillSequence.ToString(),
                c42_code = dispense.rawDataRow.DateOfService.ToString(),
                c43_code = dispense.rawDataRow.PersonCode,
                c46_code = dispense.rawDataRow.ShipState,
                c47_code = dispense.ProgramHeader,
                c53_code = jpapReshipPlanCodes.Contains(dispense.rawDataRow.PlanCode, StringComparer.OrdinalIgnoreCase) ? dispense.rawDataRow.SoldDate.ToString() : null,
                c56_code = dispense.rawDataRow.PlanCode,
                c82_code = dispense.rawDataRow.OrderNumber,
                c83_code = dispense.rawDataRow.TrackingNumber,
                c84_code = dispense.rawDataRow.CourierName,
                c86_code = dispense.rawDataRow.TotalRefillsAllowed.ToString(),
                c88_code = CLOSED,
                ca8_code = DateTime.Now.ToString()
            });
        }

        // Add a new follow-up issue
        if ((string.IsNullOrEmpty(dispense.rawDataRow.ProductName) || !dispense.rawDataRow.ProductName.Contains(NO_DISPENSE)) &&
            (!jpapReshipPlanCodes.Contains(dispense.rawDataRow.PlanCode, StringComparer.OrdinalIgnoreCase)) &&
            dispense.ProgramType != ONCOLOGY_PROGRAM_TYPE)
        {
            issueList.Add(new Issue()
            {
                APCWEditMode = CaseServiceWrapper.APCWEditModeType.New,
                product_code = dispense.CrmProductName,
                c08_code = dispense.StoreNumber.ToString(),
                c11_code = dispense.rawDataRow.RxNumber,
                c16_code = setNewReferralStatusPrograms.Contains(dispense.ProgramType, StringComparer.OrdinalIgnoreCase) ? NEW_REFERRAL_PATIENT_STATUS : null,
                c19_code = dispense.NextWorkflowStatus,
                c25_code = dispense.FillDate.ToString(),
                c35_code = dispense.CallDate?.ToString("MM/dd/yyyy HH:mm:ss"),
                c39_code = dispense.rawDataRow.DrugNDC,
                c47_code = dispense.ProgramHeader,
                c75_code = CLINICAL_QUESTION_DECLINED,
                c88_code = OPEN,
                c89_code = ISSUE_SOURCE,
                c97_code = c97_code

            });
            followUpIssuesCreatedCount++;
        }

        request.Case[0].IssueList = new IssueList()
        {
            Issue = issueList.ToArray()
        };

        var response = await RetryPolicy.ExecuteAsync(() =>
                    _caseService.UpdateCaseAsync(request)).ConfigureAwait(false);

        if (!(response is null) && response.Valid != CaseServiceWrapper.ValidState.Ok)
        {
            string subs = "\t" + string.Join("\n\t", response.MessageList[0].Substitution.Select(s => s.Text));
            //TODO: Log.LogWarn($"UpdateCase API response status [{response.Valid}]");
            //TODO: LogApiResponseMessages(response.MessageList);
            throw new GeneralApiException($"CaseId = {caseId} had API Status: [{response.Valid}], {response.MessageList[0].Text} - {subs}");
        }

        if (response is null || response.Case is null)
        {
            //TODO: Consider a custom excpetion to handle this without failing the job. 
            throw new Exception($"Update case failed for case [{caseId}]");
        }
        else
        {
            if (!string.IsNullOrEmpty(dispense.PatientLookupStatus))
            {
                // Post an action to the admin queue
                await PostActionAsync(dispense.PatientLookupStatus, ADMIN_QUEUE_USER, caseId, DateTime.Now, c).ConfigureAwait(false);
            }

            // Release the case from the api user
            await ReleaseCaseAsync(caseId, c).ConfigureAwait(false);

            // Clear the patient status change date on any follow-up issues that were created, so they won't be sent as status updates to the vendors
            if (followUpIssuesCreatedCount > 0)
            {
                var followUpIssueSequence = Convert.ToInt32(response.Case[0].IssueList?.Issue?.OrderBy(s => Convert.ToInt32(s.issue_seq))?.Last()?.issue_seq);
                for (int i = 0; i < followUpIssuesCreatedCount; i++)
                {
                    await ClearPatientStatusChangeDateAsync(caseId, followUpIssueSequence.ToString(), c).ConfigureAwait(false);
                    followUpIssueSequence--;
                }
            }

            return caseId;
        }
    }

    public async Task CreateAdminQueueCaseAsync(
        AstuteAdherenceDispenseReportRow dispense,
        CancellationToken c)
    {
        var adminQueueAction = DISPENSE_LOGIC_MISMATCH;

        if (!string.IsNullOrEmpty(dispense.PatientLookupStatus) && dispense.PatientLookupStatus == PATIENT_NOT_FOUND)
        {
            adminQueueAction = PATIENT_NOT_FOUND;
        }

        var request = new CaseListUpdateRequest
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = CaseListUpdateType.Update,
            Case = new Case[]
            {
                    new Case()
                    {
                        APCWEditMode = CaseServiceWrapper.APCWEditModeType.New,
                        company_id = "SYS",
                        b07_code = dispense.ProgramType,
                        IssueList = new IssueList()
                        {
                            Issue = new Issue[]
                            {
                                new Issue()
                                {
                                    APCWEditMode = CaseServiceWrapper.APCWEditModeType.New,
                                    product_code = dispense.CrmProductName,
                                    c06_code = dispense.rawDataRow.TotalRefillsRemaining.ToString(),
                                    c07_code = dispense.rawDataRow.Quantity.ToString(),
                                    c08_code = dispense.StoreNumber.ToString(),
                                    c09_code = dispense.FillNumber.ToString(),
                                    c10_code = dispense.rawDataRow.WrittenDate.ToString(),
                                    c11_code = dispense.rawDataRow.RxNumber,
                                    c12_code = dispense.SoldDateTime.ToString(),
                                    c15_code = dispense.SoldDateTime.ToString(),
                                    c16_code = (string.IsNullOrEmpty(dispense?.rawDataRow.ProductName) || dispense.rawDataRow.ProductName.Contains(NO_DISPENSE)) ? null : SHIPPED_PATIENT_STATUS,
                                    c19_code = SHIPPED_WORKFLOW_STATUS,
                                    c21_code = dispense?.rawDataRow.DaysSupply.ToString(),
                                    c39_code = dispense?.rawDataRow.DrugNDC,
                                    c40_code = dispense?.rawDataRow.RxFillSequence.ToString(),
                                    c42_code = dispense?.rawDataRow.DateOfService.ToString(),
                                    c43_code = dispense?.rawDataRow.PersonCode,
                                    c46_code = dispense?.rawDataRow.ShipState,
                                    c47_code = dispense?.ProgramHeader,
                                    c56_code = dispense?.rawDataRow.PlanCode,
                                    c82_code = dispense?.rawDataRow.OrderNumber,
                                    c83_code = dispense?.rawDataRow.TrackingNumber,
                                    c84_code = dispense?.rawDataRow.CourierName,
                                    c86_code = dispense?.rawDataRow.TotalRefillsAllowed.ToString(),
                                    c75_code = CLINICAL_QUESTION_DECLINED,
                                    c88_code = OPEN,
                                    c89_code = ISSUE_SOURCE,
                                    ca8_code = DateTime.Now.ToString(),
                                    ca9_code = adminQueueAction == DISPENSE_LOGIC_MISMATCH ? DISPENSE_LOGIC_MISMATCH : null
                                }
                            }
                        },
                        CaseTextList = new CaseTextList()
                        {
                            CaseText = new CaseText[]
                            {
                                new CaseText()
                                {
                                    text_type_code = REP_NOTES_TYPE,
                                    description = CASE_TEXT_DESCRIPTION,
                                    case_text = $"{adminQueueAction.ToUpper()} - First Name: {dispense?.rawDataRow.PatientFirstName}, Last Name: {dispense?.rawDataRow.PatientLastName}, " +
                                    $"DOB: {dispense?.rawDataRow.PatientDateOfBirth}, Patient ID: {dispense?.rawDataRow.CardholderOrPatientId}"
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
                        AllAttributes = CaseServiceWrapper.TrueFalseType.Item1
                    }
                }
            }
        };

        var response = await RetryPolicy.ExecuteAsync(() =>
                    _caseService.UpdateCaseAsync(request)).ConfigureAwait(false);

        if (!(response is null) && response.Valid != CaseServiceWrapper.ValidState.Ok)
        {
            string subs = "\t" + string.Join("\n\t", response.MessageList[0].Substitution.Select(s => s.Text));
            throw new GeneralApiException($"RxFillSequence = {dispense?.rawDataRow.RxFillSequence} had API Status: [{response.Valid}] - {response.MessageList[0].Text} - {subs}");
            //TODO: Log.LogError($"CreateCase status [{response.Valid}]");
            //LogApiResponseMessages(response.MessageList);
        }

        if (response is null || response.Case is null)
        {
            // TODO: Consider throwing a custom exception to handle this and allow good data to continue rather than stopping the whole job.
            // Ex from Sharon: Unable to process request due to internal error, please try again.  If the problem persists, contact the system administrator


            throw new Exception("Case creation failed");
        }
        else
        {
            var caseId = Convert.ToInt32(response.Case[0].case_id);

            // Post an action to send the new case to the admin queue
            await PostActionAsync(PATIENT_NOT_FOUND, ADMIN_QUEUE_USER, caseId, DateTime.Now, c).ConfigureAwait(false);

            // Release the case from the api user
            await ReleaseCaseAsync(caseId, c).ConfigureAwait(false);
        }
    }


    /// <summary>
    /// Post an action in the CRM application
    /// </summary>
    /// <param name="actionTypeCode">type of action to post</param>
    /// <param name="referredToUserCode">user to refer action to</param>
    /// <param name="caseId">case ID to post action to</param>
    /// <param name="responseDue">date the action response is due</param>
    private async Task PostActionAsync(
        String actionTypeCode,
        string referredToUserCode,
        int caseId,
        DateTime responseDue,
        CancellationToken c)
    {
        var request = new PostActionListRequest()
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = PostActionListType.Update,
            PostAction = new PostAction()
            {
                APCWEditMode = CaseServiceWrapper.APCWEditModeType.New,
                company_id = "SYS",
                case_id = caseId,
                action_type_code = actionTypeCode,
                referred_to_user_code = referredToUserCode,
                response_due = responseDue.ToString()
            },
            ResponseFormat = new ResponseFormat()
            {
                CaseList = new CaseListFormat()
                {
                    Case = new CaseFormat()
                    {
                        AllAttributes = CaseServiceWrapper.TrueFalseType.Item1
                    }
                }
            }
        };

        var response = await _caseService.PostActionAsync(request).ConfigureAwait(false);

        if (response is null || response.Valid != CaseServiceWrapper.ValidState.Ok)
        {
            if (!(response is null))
            {
                //TODO: Log.LogWarn($"PostAction status [{response.Valid}] for case [{caseId}]");
                //TODO: LogApiResponseMessages(response.MessageList);
            }

            //TODO: Consider throwing a custom exception to handle this and allow good data to continue rather than stopping the whole job.
            throw new Exception($"Post action failed for case [{caseId}]");
        }
    }


    /// <summary>
    /// Clear the patient status changed date on the CRM issue so it will not trigger an outbound status to a vendor
    /// </summary>
    /// <param name="caseId">The ID of the CRM case to be updated</param>
    /// <param name="issueSequence">The sequence ID of the CRM issue to be updated</param>
    private async Task ClearPatientStatusChangeDateAsync(
        int caseId,
        string issueSequence,
        CancellationToken c)
    {
        if (!string.IsNullOrEmpty(issueSequence))
        {
            var request = new CaseListUpdateRequest
            {
                UserName = _config.Value.Username,
                Password = _config.Value.Password,
                Type = CaseListUpdateType.Update,
                Case = new Case[]
                {
                        new Case()
                        {
                            APCWEditMode = CaseServiceWrapper.APCWEditModeType.Modified,
                            company_id = "SYS",
                            case_id = caseId.ToString(),
                            IssueList = new IssueList()
                            {
                                Issue = new Issue[]
                                {
                                    new Issue()
                                    {
                                        APCWEditMode = CaseServiceWrapper.APCWEditModeType.Modified,
                                        issue_seq = issueSequence,
                                        c26_code = "", // clear Patient status change date/time
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
                            AllAttributes = CaseServiceWrapper.TrueFalseType.Item1,
                        }
                    }
                }
            };

            var response = await RetryPolicy.ExecuteAsync(() =>
                    _caseService.UpdateCaseAsync(request)).ConfigureAwait(false);

            if (!(response is null) && response.Valid != CaseServiceWrapper.ValidState.Ok)
            {
                string subs = "\t" + string.Join("\n\t", response.MessageList[0].Substitution.Select(s => s.Text));
                throw new GeneralApiException($"CaseId = {caseId} had API Status: [{response.Valid}], {response.MessageList[0].Text} - {subs}");
                //TODO: Log.LogWarn($"UpdateIssue API response status [{response.Valid}]");
                //TODO: LogApiResponseMessages(response.MessageList);
            }

            if (response is null || response.Case is null)
            {
                //TODO: Log.LogWarn($"ClearPatientStatusChangeDate failed for case id [{caseId}], issue sequence [{issueSequence}]");
                //TODO: Consider throwing a custom exception to handle this and allow good data to continue rather than stopping the whole job.
                throw new Exception("ClearPatientStatusChangeDate failed");
            }
            else
            {
                // Release the case from the api user
                await ReleaseCaseAsync(caseId, c).ConfigureAwait(false);
            }
        }
    }


    /// <summary>
    /// Update the workflow status of an issue in the CRM application
    /// </summary>
    /// <param name="caseId">The ID of the case to be updated</param>
    /// <param name="issueSequence">The sequence ID of the issue to be updated</param>
    /// <param name="workflowStatus">The workflow status to set</param>
    /// <param name="issueStatus">The issue status to set</param>
    private async Task UpdateIssueAsync(
        int caseId,
        int issueSequence,
        string workflowStatus,
        string issueStatus,
        CancellationToken c)
    {
        var request = new CaseListUpdateRequest
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = CaseListUpdateType.Update,
            Case = new Case[]
            {
                    new Case()
                    {
                        APCWEditMode = CaseServiceWrapper.APCWEditModeType.Modified,
                        company_id = "SYS",
                        case_id = caseId.ToString(),
                        IssueList = new IssueList()
                        {
                            Issue = new Issue[]
                            {
                                new Issue()
                                {
                                    APCWEditMode = CaseServiceWrapper.APCWEditModeType.Modified,
                                    issue_seq = issueSequence.ToString(),
                                    c19_code = workflowStatus,
                                    c25_code = "", // clear Fill By Date
                                    c35_code = "", // clear Follow-Up Date
                                    c37_code = "", // clear Follow-Up Time
                                    c88_code = issueStatus
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
                        AllAttributes = CaseServiceWrapper.TrueFalseType.Item1,
                    }
                }
            }
        };

        var response = await RetryPolicy.ExecuteAsync(() =>
                    _caseService.UpdateCaseAsync(request)).ConfigureAwait(false);

        if (!(response is null) && response.Valid != CaseServiceWrapper.ValidState.Ok)
        {
            string subs = "\t" + string.Join("\n\t", response.MessageList[0].Substitution.Select(s => s.Text));
            throw new GeneralApiException($"CaseId = {caseId} had API Status: [{response.Valid}], {response.MessageList[0].Text} - {subs}");
            //TODO: Log.LogWarn($"UpdateIssue API response status [{response.Valid}]");
            //TODO: LogApiResponseMessages(response.MessageList);
        }

        if (response is null || response.Case is null)
        {

            //TODO: Log.LogWarn($"UpdateIssue failure for case [{caseId}]");

            throw new Exception("Issue update failed");

            //TODO in KBA: After job failure, then Restart job, but also inform MH if persistent so we can esclate to Network Team, Windows, Vendors, etc.....VERIFY with Sharon C.
        }
        else
        {
            // Release the case from the api user
            await ReleaseCaseAsync(caseId, c).ConfigureAwait(false);
        }
    }


    /// <summary>
    /// Release a case in the CRM application from edit
    /// </summary>
    /// <param name="caseId">case ID to release</param>
    /// <returns></returns>
    public async Task ReleaseCaseAsync(
        int caseId,
        CancellationToken c)
    {
        var request = new CaseUserListRequest()
        {
            UserName = _config.Value.Username,
            Password = _config.Value.Password,
            Type = CaseUserListType.Update,
            CaseUser = new CaseUser()
            {
                APCWEditMode = CaseServiceWrapper.APCWEditModeType.Delete,
                case_id = caseId,
                company_id = "SYS",
                system_user_id = _config.Value.Username
            }
        };

        var response = await _caseService.ReleaseCaseAsync(request).ConfigureAwait(false);

        if (response is null || response.Valid != CaseServiceWrapper.ValidState.Ok)
        {
            if (!(response is null))
            {
                //TODO: Log.LogWarn($"ReleaseCase status [{response.Valid}] for case [{caseId}]");
                //TODO: LogApiResponseMessages(response.MessageList);
            }

            //TODO: Consider custom exception to all job to continue.
            throw new Exception($"Release case failed for case [{caseId}]");
        }
    }


    private async Task<IEnumerable<Case>> FindCaseUsingPatientIdDobProgramTypeAsync(
        AstuteAdherenceDispenseReportRow dispense,
        CancellationToken c)
    {

        // First look for cases by patient ID, date of birth, and program type
        if (!string.IsNullOrEmpty(dispense.PatientIdType) &&
            !string.IsNullOrEmpty(dispense.rawDataRow.CardholderOrPatientId) &&
            !string.IsNullOrEmpty(dispense.ProgramType))
        {
            var request = CreateCaseGetRequest(dispense.ProgramType);

            request.Case.AddressList = new AddressList()
            {
                Address = new CaseServiceWrapper.Address[]
                {
                        new CaseServiceWrapper.Address()
                        {
                            PhoneList = new CaseServiceWrapper.PhoneList()
                            {
                                Phone = new CaseServiceWrapper.Phone[]
                                {
                                    new CaseServiceWrapper.Phone()
                                    {
                                        phone_type_code = dispense.PatientIdType,
                                        // Use asterisks to do an exact match in the phone number field used for patient ID's, otherwise a partial match is done
                                        phone = $"*{dispense.rawDataRow.CardholderOrPatientId}*"
                                    }
                                }
                            },
                            a05_code = dispense.rawDataRow.PatientDateOfBirth?.ToString("MM/dd/yyyy")
                        }
                }
            };

            var response = await _caseService.GetCaseAsync(request).ConfigureAwait(false);

            if (!(response is null) && !(response.Case is null))
            {
                return response.Case;
            }
        }
        return Array.Empty<Case>();
    }

    private async Task<IEnumerable<Case>> FindCaseUsingPatientNameDobProgramTypeAsync(
        AstuteAdherenceDispenseReportRow dispense,
        CancellationToken c)
    {
        if (!string.IsNullOrEmpty(dispense.rawDataRow.PatientLastName) &&
            !string.IsNullOrEmpty(dispense.rawDataRow.PatientFirstName) &&
            !string.IsNullOrEmpty(dispense.ProgramType))
        {
            var request = CreateCaseGetRequest(dispense.ProgramType);

            request.Case.AddressList = new AddressList()
            {
                Address = new CaseServiceWrapper.Address[]
                {
                        new CaseServiceWrapper.Address()
                        {
                            last_name = dispense.rawDataRow.PatientLastName,
                            given_names = dispense.rawDataRow.PatientFirstName,
                            a05_code = dispense.rawDataRow.PatientDateOfBirth?.ToString("MM/dd/yyyy")
                        }
                }
            };

            var response = await _caseService.GetCaseAsync(request).ConfigureAwait(false);

            if (!(response is null) && !(response.Case is null))
            {
                if (!string.IsNullOrEmpty(dispense.PatientIdType))
                {
                    dispense.PatientLookupStatus = ID_NOT_FOUND;
                }
                return response.Case;
            }
        }
        return Array.Empty<Case>();
    }

    private async Task<IEnumerable<Case>> FindCaseUsingPatientNamePidProgramTypeAsync(
        AstuteAdherenceDispenseReportRow dispense,
        CancellationToken c)
    {
        // If no case is found by patient last name, first name, and date of birth, search on patient last name, first name, patient ID, and program type
        if (!string.IsNullOrEmpty(dispense.rawDataRow.PatientLastName) &&
            !string.IsNullOrEmpty(dispense.rawDataRow.PatientFirstName) &&
            !string.IsNullOrEmpty(dispense.PatientIdType) &&
            !string.IsNullOrEmpty(dispense.rawDataRow.CardholderOrPatientId) &&
            !string.IsNullOrEmpty(dispense.ProgramType))
        {
            var request = CreateCaseGetRequest(dispense.ProgramType);

            request.Case.AddressList = new AddressList()
            {
                Address = new CaseServiceWrapper.Address[]
                {
                        new CaseServiceWrapper.Address()
                        {
                            last_name = dispense.rawDataRow.PatientLastName,
                            given_names = dispense.rawDataRow.PatientFirstName,
                            PhoneList = new CaseServiceWrapper.PhoneList()
                            {
                                Phone = new CaseServiceWrapper.Phone[]
                                {
                                    new CaseServiceWrapper.Phone()
                                    {
                                        phone_type_code = dispense.PatientIdType,
                                        // Use asterisks to do an exact match in the phone number field used for patient ID's, otherwise a partial match is done
                                        phone = $"*{dispense.rawDataRow.CardholderOrPatientId}*"
                                    }
                                }
                            }
                        }
                }
            };

            var response = await _caseService.GetCaseAsync(request).ConfigureAwait(false);

            if (!(response is null) && !(response.Case is null))
            {
                if (!string.IsNullOrEmpty(dispense.PatientIdType))
                {
                    dispense.PatientLookupStatus = DOB_NOT_FOUND;
                }
                return response.Case;
            }
        }
        return Array.Empty<Case>();
    }

    private async Task<IEnumerable<Case>> FindCaseUsingEnterpriseRxPatientNumProgramTypeAsync(
        AstuteAdherenceDispenseReportRow dispense,
        CancellationToken c)
    {
        // If no case is found by patient last name, first name, patient ID, and program type, search on EnterpriseRx patient num and program type
        if (!string.IsNullOrEmpty(dispense.rawDataRow.PatientNum) &&
            !string.IsNullOrEmpty(dispense.ProgramType))
        {
            var request = CreateCaseGetRequest(dispense.ProgramType);

            request.Case.AddressList = new AddressList()
            {
                Address = new CaseServiceWrapper.Address[]
                {
                        new CaseServiceWrapper.Address()
                        {
                            a35_code = dispense.rawDataRow.PatientNum
                        }
                }
            };

            var response = await _caseService.GetCaseAsync(request).ConfigureAwait(false);

            if (!(response is null) && !(response.Case is null))
            {
                dispense.PatientLookupStatus = NAME_DOB_NOT_FOUND;
                return response.Case;
            }
        }
        return Array.Empty<Case>();
    }
}
