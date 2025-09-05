using AstuteCaseService;
using AstutePatientService;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wegmans.RX.Orbita.Astute.Configuration;
using Wegmans.RX.Orbita.Astute.Extensions;
using Wegmans.RX.Orbita.Astute.Models;

namespace Wegmans.RX.Orbita.Astute
{
    public class AstuteSoapProxy : IAstuteSoapProxy
    {
        private struct LoggingStruct
        {
            public string Text { get; set; }
            public IEnumerable<string> Substitutions { get; set; }
            public bool IsError { get; set; }
        };

        private readonly ICaseService _caseServiceClient;
        private readonly IAddressService _addressServiceClient;
        private readonly ILogger _log;
        private readonly AppAstuteClientSecurity _clientSecurity;

        public AstuteSoapProxy(ICaseService caseServiceClient, IAddressService addressServiceClient, ILogger<AstuteSoapProxy> log, IOptions<AppAstuteClientSecurity> clientSecurity)
        {
            _caseServiceClient = caseServiceClient ?? throw new ArgumentNullException(nameof(caseServiceClient));
            _addressServiceClient = addressServiceClient ?? throw new ArgumentNullException(nameof(addressServiceClient));
            _log = log ?? throw new ArgumentNullException(nameof(log));
            _clientSecurity = clientSecurity.Value ?? throw new ArgumentNullException(nameof(clientSecurity));
        }

        public async Task<List<Patient>> GetPatientsAsync()
        {
            var request = new CaseSearchRequest()
            {
                UserName = _clientSecurity.Username,
                Password = _clientSecurity.Password,
                UTCOffset = "0",
                Type = CaseSearchType.Get,
                Case = new SearchCase()
                {
                    company_id = AstuteConstants.CompanyId,
                    b07_code = AstuteConstants.ProgramType,
                    case_status = AstuteConstants.StatusOpen,
                },
                FilterList = new FilterList()
                {
                    Filter = new Filter[]
                    {
                        new Filter()
                        {
                            filter_seq = 1,
                            selection_operator = SelectionOperator.BETWEEN,
                            selection_category_id = "BF6", // Case Date Added (BF6)
                            selection_code = new DateTime(DateTime.Today.Year, 1, 1).ToString(),
                            selection_code2 = new DateTime(DateTime.Today.AddYears(1).Year, 1, 1).ToString()
                        },
                        new Filter()
                        {
                            filter_seq = 2,
                            selection_operator = SelectionOperator.ISNULL,
                            selection_category_id = "A60", // SentToOrbita (A60)
                            selection_code = "False"
                        },
                    }
                },
                ResponseFormat = (new ResponseFormat()).CreateDefaultCaseResponseFormat()
            };

            List<Case> caseList = (await this.SearchStatusesAsync(request).ConfigureAwait(false)).ToList();

            request.FilterList.Filter[1].selection_operator = SelectionOperator.EQUAL;
            caseList.AddRange(await this.SearchStatusesAsync(request).ConfigureAwait(false));

            List<Patient> patients = new List<Patient>();

            foreach (var caseRecord in caseList)
            {
                var patient = caseRecord.CreateAstutePatient();
                if (!(patient is null))
                {
                    patients.Add(patient);
                }
            }

            return patients;
        }

        public async Task UpdatePatientAsync(string addressId)
        {
            if (!string.IsNullOrEmpty(addressId))
            {
                var request = new AddressListUpdate()
                {
                    UserName = _clientSecurity.Username,
                    Password = _clientSecurity.Password,
                    UTCOffset = "0",
                    Type = RequestType.Update,
                    Address = new AstutePatientService.Address()
                    {
                        APCWEditMode = AstutePatientService.APCWEditModeType.Modified,
                        company_id = AstuteConstants.CompanyId,
                        address_id = addressId,
                        active = AstutePatientService.YesNo.Y,
                        a60_code = "True"
                    },
                    ResponseFormat = (new AddressResponseFormat()).CreateDefaultAddressResponseFormat()
                };

                var response = await _addressServiceClient.UpdateAddressAsync(request).ConfigureAwait(false);

                if (response is null || response.Address is null)
                {
                    throw new AstuteException("Patient could not be updated");
                }

                if (response.Valid != AstutePatientService.ValidState.Ok)
                {
                    this.LogAstuteError(response.MessageList?.Select(x => new LoggingStruct { Text = x.Text, Substitutions = x.Substitution?.Select(y => y.Text), IsError = (response.Valid == AstutePatientService.ValidState.Error) }));
                }
            }
        }


        public async Task<IEnumerable<Case>> SearchStatusesAsync(CaseSearchRequest caseSearchRequest)
        {
            List<Case> cases = new List<Case>();

            var response = await this._caseServiceClient.SearchCaseAsync(caseSearchRequest).ConfigureAwait(false);

            if (!(response is null) && !(response.Case is null))
            {
                cases.AddRange(response.Case);
            }
            else
            {
                if (!(response is null))
                {
                    if (response.Valid != AstuteCaseService.ValidState.Ok)
                    {
                        this.LogAstuteError(response.MessageList?.Select(x => new LoggingStruct { Text = x.Text, Substitutions = x.Substitution?.Select(y => y.Text), IsError = (response.Valid == AstuteCaseService.ValidState.Error) }));

                        if (response.Valid == AstuteCaseService.ValidState.Error || response.Valid == AstuteCaseService.ValidState.Unknown)
                        {
                            throw new AstuteException("Astute bad response");
                        }
                    }
                }
                else
                {
                    throw new AstuteException("Astute bad response");
                }
            }

            return cases;
        }

        private void LogAstuteError(IEnumerable<LoggingStruct> messageCollection)
        {
            string logMessage = string.Empty;
            bool throwException = false;

            foreach (var message in messageCollection)
            {
                logMessage = "Astute Error: ";
                if (!(message.Text is null))
                {
                    logMessage += message.Text.Replace('{', '(').Replace('}', ')');
                }
                else
                {
                    logMessage += "message has no Text object";
                }
                if (!(message.Substitutions is null))
                {
                    foreach (var substitution in message.Substitutions)
                    {
                        logMessage += " : " + substitution;
                    }
                }

                _log.LogError(logMessage);

                if (message.IsError == true)
                    throwException = true;
            }

            if (throwException == true)
                throw new AstuteException(logMessage);
        }





    }
}
