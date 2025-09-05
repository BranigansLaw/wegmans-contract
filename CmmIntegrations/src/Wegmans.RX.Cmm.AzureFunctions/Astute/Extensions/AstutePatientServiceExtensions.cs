using AstutePatientService;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models.Outbound;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Extensions
{
    public static class AstutePatientServiceExtensions
    {
        [ExcludeFromCodeCoverage]
        public static AddressResponseFormat CreateDefaultAddressResponseFormat(this AddressResponseFormat addressResponseFormat)
        {
            addressResponseFormat.AddressList = new AddressListFormat
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
            };

            return addressResponseFormat;
        }

        [ExcludeFromCodeCoverage]
        public static void CreatePatientIdSearch(this AddressListSearch addressListSearch, string patientId, string patientIdType)
        {
            addressListSearch.UTCOffset = "0";
            addressListSearch.Type = RequestType.Get;
            addressListSearch.Address = new SearchAddress
            {
                company_id = AstuteConstants.CompanyId,
                allow_survey = SearchYesNo.B,
                PhoneList = new PhoneList()
                {
                    Phone = new Phone[]
                    {
                            new Phone()
                            {
                                phone_type_code = patientIdType,
                                phone = string.Format("*{0}*", patientId) // Use asterisks to do an exact match for patient IDs, otherwise a partial match happens for longer values (> 16?)
                            }
                    }
                }
            };
            addressListSearch.ResponseFormat = (new AddressResponseFormat()).CreateDefaultAddressResponseFormat();
        }

        [ExcludeFromCodeCoverage]
        public static void CreateFirstNameLastNameDobSearch(this AddressListSearch addressListSearch, string firstName, string lastName, DateTimeOffset? dateOfBirth)
        {
            addressListSearch.UTCOffset = "0";
            addressListSearch.Type = RequestType.Get;
            addressListSearch.Address = new AstutePatientService.SearchAddress
            {
                company_id = AstuteConstants.CompanyId,
                allow_survey = SearchYesNo.B,
                address_type_code = AstuteConstants.AddressTypeCode,
                given_names = firstName,
                last_name = lastName,
                a05_code = dateOfBirth.Value.ToString("MM/dd/yyyy")
            };
            addressListSearch.ResponseFormat = (new AddressResponseFormat()).CreateDefaultAddressResponseFormat();
        }

        public static PatientCase CreatePatientCase(this Address address)
        {
            _ = address.a05_code ?? throw new ArgumentNullException(nameof(address.a05_code));

            DateTime? dateOfBirth = null;

            if (DateTime.TryParse(address.a05_code, out DateTime parseDateOfBirth))
            {
                dateOfBirth = parseDateOfBirth;
            }

            return new PatientCase()
            {
                PatientId = Convert.ToInt32(address.address_id),
                CmmPatientId = address.PhoneList?.Phone?.FirstOrDefault(x => x.phone_type_code == AstuteConstants.CmmPatientType)?.phone,
                FirstName = address.given_names,
                LastName = address.last_name,
                DateOfBirth = dateOfBirth,
                Gender = address.a20_code,
                Address1 = address.address1,
                Address2 = address.address2,
                City = address.city,
                State = address.state,
                ZipCode = address.postal_code,
                PrescriberFirstName = address.a22_code,
                PrescriberLastName = address.a23_code,
                PrescriberNpi = address.a36_code,
                PrescriberPhone = address.a30_code,
                PrescriberFax = address.a31_code,
                PrescriberAddress1 = address.a24_code,
                PrescriberAddress2 = address.a25_code,
                PrescriberCity = address.a27_code,
                PrescriberState = address.a28_code,
                PrescriberZipCode = address.a29_code,
                PrimaryPhone = address.PhoneList?.Phone?.FirstOrDefault(x => x.phone_type_code == AstuteConstants.PrimaryPhone)?.phone,
                JcpId = address.PhoneList?.Phone?.FirstOrDefault(x => x.phone_type_code == AstuteConstants.JcpPatientId)?.phone,
                ExtendedPatientIdList = address.Extended?.p03_code
            };
        }
    }
}
