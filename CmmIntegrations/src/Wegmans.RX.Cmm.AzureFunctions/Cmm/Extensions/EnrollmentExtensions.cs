using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Models;

namespace Wegmans.RX.Cmm.AzureFunctions.Cmm.Extensions
{
    public static class EnrollmentExtensions
    {
        [ExcludeFromCodeCoverage]
        public static Astute.Models.Enrollment CreateAstuteEnrollment(this Enrollment enrollment) => new Astute.Models.Enrollment()
        {
            Patient = new Astute.Models.Patient()
            {
                Id = enrollment.Patient.Id,
                JcpId = enrollment.Patient.JcpId,
                FirstName = enrollment.Patient.FirstName,
                LastName = enrollment.Patient.LastName,
                DateOfBirth = enrollment.Patient.DateOfBirth,
                Gender = enrollment.Patient.Gender,
                Address = new Astute.Models.Address()
                {
                    Address1 = enrollment.Patient.Address.Address1,
                    Address2 = enrollment.Patient.Address.Address2,
                    City = enrollment.Patient.Address.City,
                    State = enrollment.Patient.Address.State,
                    ZipCode = enrollment.Patient.Address.ZipCode,
                },
                Phone = enrollment.Patient.PrimaryPhone,
                Provider = new Astute.Models.Provider()
                {
                    FirstName = enrollment.Patient.Case.Provider.FirstName,
                    LastName = enrollment.Patient.Case.Provider.LastName,
                    Address = new Astute.Models.Address()
                    {
                        Address1 = enrollment.Patient.Case.Provider.Address.Address1,
                        Address2 = enrollment.Patient.Case.Provider.Address.Address2,
                        City = enrollment.Patient.Case.Provider.Address.City,
                        State = enrollment.Patient.Case.Provider.Address.State,
                        ZipCode = enrollment.Patient.Case.Provider.Address.ZipCode
                    },
                    Phone = enrollment.Patient.Case.Provider.Phone,
                    Npi = enrollment.Patient.Case.Provider.Npi
                }
            },
            Case = new Astute.Models.Case()
            {
                Id = enrollment.Patient.Case.Id,
                ReferralId = enrollment.Patient.Case.ReferralId,
                DiagnosisCode = enrollment.Patient.Case.DiagnosisCode,
                Ndc = enrollment.Patient.Case.Ndc,
                ProgramName = enrollment.Patient.Case.ProgramName,
                ShipStarterDoseToProvider = enrollment.Patient.Case.ShipStarterDoseToProvider,
                ShipStarterDoseToPatient = enrollment.Patient.Case.ShipStarterDoseToPatient,
                EnrollmentPayload = JsonSerializer.Serialize(enrollment),
                Provider = new Astute.Models.Provider()
                {
                    FirstName = enrollment.Patient.Case.Provider.FirstName,
                    LastName = enrollment.Patient.Case.Provider.LastName,
                    Address = new Astute.Models.Address()
                    {
                        Address1 = enrollment.Patient.Case.Provider.Address.Address1,
                        Address2 = enrollment.Patient.Case.Provider.Address.Address2,
                        City = enrollment.Patient.Case.Provider.Address.City,
                        State = enrollment.Patient.Case.Provider.Address.State,
                        ZipCode = enrollment.Patient.Case.Provider.Address.ZipCode
                    },
                    Phone = enrollment.Patient.Case.Provider.Phone,
                    Npi = enrollment.Patient.Case.Provider.Npi
                }
            }
        };
    }
}
