using AstuteCaseService;
using System;
using System.Linq;
using Wegmans.RX.Orbita.Astute.Models;

namespace Wegmans.RX.Orbita.Astute.Extensions
{
    public static class AstuteCaseServiceExtensions
    {
        public static ResponseFormat CreateDefaultCaseResponseFormat(this ResponseFormat responseFormat) => new ResponseFormat()
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
        };

        public static Patient CreateAstutePatient(this AstuteCaseService.Case astuteCase)
        {
            if (astuteCase?.AddressList?.Address == null || astuteCase.AddressList?.Address?
                .Where(x => !(x.address_type_code is null) && (x.address_type_code == AstuteConstants.AddressTypeCode)).Count() == 0)
            {
                return null;
            }

            DateTime? dateOfBirth = null;
            string formattedDateOfBirth = string.Empty;

            var patientAddress = astuteCase.AddressList.Address.Where(x => x.address_type_code == AstuteConstants.AddressTypeCode).First();

            if (DateTime.TryParse(patientAddress.a05_code, out DateTime parsedDateOfBirth))
            {
                dateOfBirth = parsedDateOfBirth;
            }

            if (dateOfBirth.HasValue)
            {
                formattedDateOfBirth = dateOfBirth.Value.ToString("yyyyMMdd");
            }

            var phoneNumber = patientAddress.PhoneList?.Phone?.Where(x => x.phone_type_code == AstuteConstants.PrimaryPhone)?.FirstOrDefault().phone;

            if (!string.IsNullOrEmpty(phoneNumber))
            {
                phoneNumber = phoneNumber.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").Trim();
            }

            return new Patient
            {
                AddressId = patientAddress.address_id,
                FirstName = patientAddress.given_names,
                LastName = patientAddress.last_name,
                DateOfBirth = formattedDateOfBirth,
                PhoneNumber = phoneNumber,
                EmailAddress = patientAddress.email2 // per Eddie Scibilia: email1 doesn’t exist, email2 = primary email address, email3 = secondary email address
            };
        }
    }
}
