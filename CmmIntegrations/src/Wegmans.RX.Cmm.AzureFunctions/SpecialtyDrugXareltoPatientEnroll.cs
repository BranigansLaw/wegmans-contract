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
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web.Http;
using Wegmans.RX.Cmm.AzureFunctions.Astute;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Extensions;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Validation;

namespace Wegmans.RX.Cmm.AzureFunctions
{
    /// <summary>
    /// Xarelto specific patient enrollment
    /// </summary>
    public class SpecialtyDrugXareltoPatientEnroll
    {
        private AuthorizationOptions _adSettings;
        private AstuteSoapProxy _astuteSoapProxy;

        public SpecialtyDrugXareltoPatientEnroll(IOptions<AuthorizationOptions> adSettings, IAstuteSoapProxy astuteSoapProxy)
        {
            _adSettings = adSettings?.Value ?? throw new ArgumentNullException(nameof(adSettings));
            _astuteSoapProxy = (AstuteSoapProxy)astuteSoapProxy ?? throw new ArgumentNullException(nameof(astuteSoapProxy));
        }

        [FunctionName(nameof(SpecialtyDrugXareltoPatientEnroll))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "specialtydrugs/patients/xarelto/enroll")] HttpRequest request,
            ILogger log
            )
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
                var options = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = new JsonSnakeCaseNamingPolicy()
                };

                // Cmm.Models.EnrollmentXarelto is where all the json deserialization magic happens
                Cmm.Models.EnrollmentXarelto enrollment = JsonSerializer.Deserialize<Cmm.Models.EnrollmentXarelto>(await new StreamReader(request.Body).ReadToEndAsync().ConfigureAwait(false), options);

                var validator = new EnrollmentXareltoValidator();

                ValidationResult result = validator.Validate(enrollment);

                if (!result.IsValid)
                {
                    return new BadRequestErrorMessageResult(result.ToString());
                }

                log.LogDebug("Before Translating Xarelto CMM Enrollment object into Astute Enrollment object.");

   //             var astuteEnrollment = enrollment.CreateAstuteEnrollment();

                log.LogDebug("After Translating Xarelto CMM Enrollment object into Astute Enrollment object.");

                try
                {
                    //var patientCase = await _astuteSoapProxy.FindPatientAsync(
                    //    astuteEnrollment.Patient.JcpId,
                    //    astuteEnrollment.Patient.Id,
                    //    astuteEnrollment.Patient.FirstName,
                    //    astuteEnrollment.Patient.LastName,
                    //    astuteEnrollment.Patient.DateOfBirth).ConfigureAwait(false);

                    //if (patientCase is null)
                    //{
                    //    // Create new patient
                    //    patientCase = await _astuteSoapProxy.CreatePatientAsync(astuteEnrollment.Patient).ConfigureAwait(false);
                    //}

                    //astuteEnrollment.Case.AstuteAddressId = patientCase.PatientId;

                    //// Look for existing cases for the patient
                    //var openCases = await _astuteSoapProxy.FindOpenCasesAsync(astuteEnrollment.Case.AstuteAddressId, null).ConfigureAwait(false);

                    //// Determine status for new case
                    //var caseStatus = await _astuteSoapProxy.CreateNewCaseStatus(astuteEnrollment.Case, openCases).ConfigureAwait(false);

                    //astuteEnrollment.Case.CaseStatus = caseStatus.CaseStatus;
                    //astuteEnrollment.Case.ExceptionReason = caseStatus.ExceptionReason;

                    //// Create a new case for patient
                    //var newCase = await _astuteSoapProxy.CreateCaseAsync(astuteEnrollment.Case).ConfigureAwait(false);

                    //var caseId = newCase.CaseId;

                    //// Release case from api user
                    //await _astuteSoapProxy.ReleaseCaseAsync(caseId).ConfigureAwait(false);

                  //  return new CreatedResult($"/specialtydrugs/patients/{enrollment.Patient.Id}", null);

                    // Delete me: This just returns a dummy success result.
                    return new CreatedResult($"/specialtydrugs/patients/12345", null);
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
