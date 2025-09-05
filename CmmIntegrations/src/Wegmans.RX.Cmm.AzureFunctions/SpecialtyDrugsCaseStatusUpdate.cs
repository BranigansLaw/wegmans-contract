using FluentValidation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shared;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using Wegmans.RX.Cmm.AzureFunctions.Astute;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Extensions;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Validation;

namespace Wegmans.RX.Cmm.AzureFunctions
{
    public class SpecialtyDrugsCaseStatusUpdate
    {
        private AuthorizationOptions _adSettings;
        private AstuteSoapProxy _astuteSoapProxy;

        public SpecialtyDrugsCaseStatusUpdate(IOptions<AuthorizationOptions> adSettings, IAstuteSoapProxy astuteSoapProxy)
        {
            _adSettings = adSettings?.Value ?? throw new ArgumentNullException(nameof(adSettings));
            _astuteSoapProxy = (AstuteSoapProxy)astuteSoapProxy ?? throw new ArgumentNullException(nameof(astuteSoapProxy));
        }

        [FunctionName(nameof(SpecialtyDrugsCaseStatusUpdate))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "patch", Route = "specialtydrugs/patients/{patientId}/cases/{caseId}/update")] HttpRequest request,
            ILogger log,
            string patientId,
            string caseId)
        {
            ClaimsPrincipal principal;

            try
            {
                principal = await request.ValidateAccessToken(_adSettings).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return new UnauthorizedResult();
            }

            if (!(principal is null) && principal.IsInRole("Cmm.Writer"))
            {
                var caseStatus = JsonSerializer.Deserialize<Cmm.Models.CaseStatus>(await new StreamReader(request.Body).ReadToEndAsync().ConfigureAwait(false));

                var validator = new CaseStatusValidator();

                ValidationResult result = validator.Validate(caseStatus);

                if (!result.IsValid)
                {
                    var errorCollection = result.Errors.GroupBy(x => x.PropertyName, x => x.ErrorMessage, (key, g) => new { PropertyName = key, Errors = g.ToArray() });
                    ValidationProblemDetails validationProblemDetails = new ValidationProblemDetails(errorCollection.ToDictionary(key => key.PropertyName, value => value.Errors));
                    return new BadRequestObjectResult(validationProblemDetails);
                }

                var astuteCaseStatus = caseStatus.CreateAstuteCaseStatus();

                try
                {
                    var patientCase = await _astuteSoapProxy.FindPatientAsync(caseStatus.Patient.JcpId, caseStatus.Patient.Id, caseStatus.Patient.FirstName, caseStatus.Patient.LastName, caseStatus.Patient.DateOfBirth).ConfigureAwait(false);

                    if (patientCase is null)
                    {
                        throw new AstuteException("Patient not found");
                    }

                    astuteCaseStatus.AstuteAddressId = patientCase.PatientId;

                    // Search for existing cases by patient and program
                    var casesToUpdate = await _astuteSoapProxy.FindOpenCasesAsync(astuteCaseStatus.AstuteAddressId, astuteCaseStatus.CaseId).ConfigureAwait(false);

                    if (casesToUpdate is null)
                    {
                        throw new AstuteException("Case not found");
                    }

                    foreach (var caseRecord in casesToUpdate)
                    {
                        bool hasValidReverificationStartDate = true;
                        if (astuteCaseStatus.ReverificationStartDate.HasValue && caseRecord.ReverificationStartDate.HasValue)
                        {
                            if (astuteCaseStatus.ReverificationStartDate.Value < caseRecord.ReverificationStartDate.Value)
                            {
                                hasValidReverificationStartDate = false;
                            }
                        }

                        var openVoucherIssues = caseRecord.PatientEvents.Where(x => x.ProgramHeader == AstuteConstants.SoSimple && x.IssueStatus == AstuteConstants.StatusOpen);
                        var caseDataSource = caseRecord.PriorAuthSource ?? caseRecord.TriageSource; // Use PA Source first, Triage source if null

                        if ((openVoucherIssues.Count() == 0 || caseDataSource.Equals(AstuteConstants.EnrollmentSource, StringComparison.CurrentCultureIgnoreCase)) && hasValidReverificationStartDate)
                        {
                            var updatedCase = await _astuteSoapProxy.UpdateCaseAsync(caseRecord, astuteCaseStatus).ConfigureAwait(false);

                            var updatedCaseId = updatedCase.CaseId;

                            // Upload attachment file
                            var attachmentName = await _astuteSoapProxy.UploadFileAsync($"VerificationOfBenefits.txt", astuteCaseStatus.FormattedVerificationOfBenefitsData, updatedCaseId).ConfigureAwait(false);

                            if (string.IsNullOrEmpty(attachmentName))
                            {
                                log.LogError("Error creating attachment for case: {0}", updatedCaseId);
                            }
                            else
                            {
                                // Add the attachment to the case
                                await _astuteSoapProxy.AddAttachmentAsync(updatedCaseId, attachmentName, $"Verification of Benefits {DateTimeOffset.Now:G}").ConfigureAwait(false);
                            }

                            // Release case from api user
                            await _astuteSoapProxy.ReleaseCaseAsync(updatedCaseId).ConfigureAwait(false);
                        }
                    }

                    return new StatusCodeResult(StatusCodes.Status200OK);
                }
                catch (AstuteException ex)
                {
                    log.LogError(ex.Message);
                    return new StatusCodeResult(StatusCodes.Status502BadGateway);
                }
            }
            else
            {
                return new UnauthorizedResult();
            }
        }
    }
}
