using AstuteAttachmentService;
using AstuteCaseService;
using AstutePatientService;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wegmans.RX.Cmm.AzureFunctions.Astute;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Configuration;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Extensions;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models.Outbound;
using Xunit;

namespace Wegmans.RX.Cmm.AzureFunctions.Tests.Astute
{
    public class AstuteSoapProxyTests
    {
        private static Dictionary<string, AddressListResponse> SearchAddressAsyncResponses = new Dictionary<string, AddressListResponse>
        {
            { "One Address Id:1", new AddressListResponse { Address = new AstutePatientService.Address[] { new AstutePatientService.Address { address_id = "1", a05_code = string.Empty } } } },
            { "One Address Id:2", new AddressListResponse{ Address = new AstutePatientService.Address[] { new AstutePatientService.Address { address_id = "2", a05_code = string.Empty } } } }
        };

        [Theory]
        [MemberData(nameof(CreateNewCaseStatusData))]
        public async Task CreateNewCaseStatus(AzureFunctions.Astute.Models.Case caseRecord, IEnumerable<PatientCase> existingCases, dynamic expectedResult)
        {
            var cs = new Mock<ICaseService>();
            _ = cs.Setup(x => x.UpdateCaseAsync(It.IsAny<CaseListUpdateRequest>())).ReturnsAsync(new CaseListResponse { Valid = AstuteCaseService.ValidState.Ok });

            var sec = Options.Create(new AppAstuteClientSecurity { Username = string.Empty, Password = string.Empty });

            var systemUnderTest = new AstuteSoapProxy(cs.Object, new Mock<IAddressService>().Object, new Mock<ICaseStreamService>().Object, new Mock<ILogger<AstuteSoapProxy>>().Object, sec);

            var result = await systemUnderTest.CreateNewCaseStatus(caseRecord, existingCases).ConfigureAwait(false);

            string exceptionReason = string.IsNullOrEmpty(expectedResult.ExceptionReason) ? null : expectedResult.ExceptionReason;

            using (new AssertionScope())
            {
                result.CaseStatus.Should().Be(expectedResult.CaseStatus);

                result.ExceptionReason.Should().Be(exceptionReason);
            }
        }

        [Theory]
        [MemberData(nameof(FindPatientAsyncData))]
        public async Task FindPatientAsync(string demographicId, string patientId, string firstName, string lastName, DateTimeOffset? dateOfBirth, PatientCase expectedResult)
        {
            var cs = new Mock<IAddressService>();
            _ = cs.Setup(x => x.SearchAddressAsync(It.IsAny<AddressListSearch>())).Returns(() => null);
            _ = cs.Setup(x => x.SearchAddressAsync(It.Is<AddressListSearch>(x => x.Address.PhoneList.Phone[0].phone == string.Format("*{0}*", patientId)))).ReturnsAsync(SearchAddressAsyncResponses["One Address Id:1"]);
            _ = cs.Setup(x => x.SearchAddressAsync(It.Is<AddressListSearch>(x => string.IsNullOrEmpty(demographicId) && string.IsNullOrEmpty(patientId) && !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName)))).ReturnsAsync(SearchAddressAsyncResponses["One Address Id:2"]);


            var sec = Options.Create(new AppAstuteClientSecurity { Username = string.Empty, Password = string.Empty });

            var systemUnderTest = new AstuteSoapProxy(new Mock<ICaseService>().Object, cs.Object, new Mock<ICaseStreamService>().Object, new Mock<ILogger<AstuteSoapProxy>>().Object, sec);

            var result = await systemUnderTest.FindPatientAsync(demographicId, patientId, firstName, lastName, dateOfBirth).ConfigureAwait(false);

            result.Should().BeEquivalentTo(expectedResult);
        }

        // We are purposely throwing an exception in the Astute error logging function now
        //[Fact]
        //public async Task TestNullSubstitutionsLogging()
        //{
        //    var cs = new Mock<ICaseService>();
        //    _ = cs.Setup(x => x.UpdateCaseAsync(It.IsAny<CaseListUpdateRequest>())).ReturnsAsync(new CaseListResponse
        //    {
        //        Case = new AstuteCaseService.Case[] { new AstuteCaseService.Case() },
        //        Valid = AstuteCaseService.ValidState.Warning,
        //        MessageList = new AstuteCaseService.Message[] { new AstuteCaseService.Message { LCID = "test", Text = "test", Type = AstuteCaseService.MessageType.Warning } }
        //    });

        //    var sec = Options.Create(new AppAstuteClientSecurity { Username = string.Empty, Password = string.Empty });

        //    var systemUnderTest = new AstuteSoapProxy(cs.Object, new Mock<IAddressService>().Object, new Mock<ICaseStreamService>().Object, new Mock<ILogger<AstuteSoapProxy>>().Object, sec);

        //    var patientCase = new PatientCase { PatientEvents = new List<PatientEvent> { new PatientEvent() } };

        //    var caseStatus = new CaseStatus { PriorAuthorization = new PriorAuthorization(), AdditionalDetails = new AdditionalDetails { PharmacyBenefits = new PharmacyBenefits { PrimaryInsurance = new PharmacyBenefitsPrimaryInsurance() } } };

        //    var result = await systemUnderTest.UpdateCaseAsync(patientCase, caseStatus).ConfigureAwait(false);
        //}

        public static IEnumerable<object[]> FindPatientAsyncData()
        {
            return new List<object[]>
            {
                new object[] { "1", "1", null, null, null, SearchAddressAsyncResponses["One Address Id:1"].Address[0].CreatePatientCase() },
                new object[] { null, "1", null, null, null, SearchAddressAsyncResponses["One Address Id:1"].Address[0].CreatePatientCase() },
                new object[] { null, null, "First", "Last", DateTimeOffset.Now, SearchAddressAsyncResponses["One Address Id:2"].Address[0].CreatePatientCase() },
                new object[] { null, null, null, null, null, null },
            };
        }

        public static IEnumerable<object[]> CreateNewCaseStatusData()
        {
            var now = DateTime.Now;

            Provider dummyProvider = new Provider
            {
                FirstName = "FirstName",
                LastName = "LastName",
                Npi = "DummyNPI",
                Phone = "1234567890",
                Fax = "0987654321",
                Address = new AzureFunctions.Astute.Models.Address
                {
                    Address1 = "1313 Mockingbird Lane",
                    Address2 = "",
                    City = "Mockingbird Heights",
                    State = "CA",
                    ZipCode = "90210"
                }
            };

            var astuteInput = new Dictionary<string, AzureFunctions.Astute.Models.Case>
            {
                { "Link:TREMFYA 100 MG/ML SYRINGE", new AzureFunctions.Astute.Models.Case { ProgramName = "Link", Ndc = "57894064001", Provider = dummyProvider} },
                { "Link:TREMFYA 100 MG/ML INJECTOR", new AzureFunctions.Astute.Models.Case { ProgramName = "Link", Ndc = "57894064011", Provider = dummyProvider } },
                { "Link:SIMPONI 50 MG/0.5 ML SYRINGE", new AzureFunctions.Astute.Models.Case { ProgramName = "Link", Ndc = "57894007001", Provider = dummyProvider } },
                { "Link:SIMPONI 50 MG/0.5 ML PEN INJECT", new AzureFunctions.Astute.Models.Case { ProgramName = "Link", Ndc = "57894007002", Provider = dummyProvider } },
                { "Link:SIMPONI 100 MG/ML SYRINGE", new AzureFunctions.Astute.Models.Case { ProgramName = "Link", Ndc = "57894007101", Provider = dummyProvider } },
                { "Link:SIMPONI 100 MG/ML PEN INJECTOR", new AzureFunctions.Astute.Models.Case { ProgramName = "Link", Ndc = "57894007102", Provider = dummyProvider } },
                { "Link:STELARA 45 MG/0.5 ML VIAL", new AzureFunctions.Astute.Models.Case { ProgramName = "Link", Ndc = "57894006002", Provider = dummyProvider } },
                { "Link:STELARA 45 MG/0.5 ML SYRINGE", new AzureFunctions.Astute.Models.Case { ProgramName = "Link", Ndc = "57894006003", Provider = dummyProvider } },
                { "Link:STELARA 90 MG/ML SYRINGE", new AzureFunctions.Astute.Models.Case { ProgramName = "Link", Ndc = "57894006103", Provider = dummyProvider } },
                { "So Simple:TREMFYA 100 MG/ML SYRINGE", new AzureFunctions.Astute.Models.Case { ProgramName = "So Simple", Ndc = "57894064001", Provider = dummyProvider } },
                { "So Simple:TREMFYA 100 MG/ML INJECTOR", new AzureFunctions.Astute.Models.Case { ProgramName = "So Simple", Ndc = "57894064011", Provider = dummyProvider } },
                { "So Simple:SIMPONI 50 MG/0.5 ML SYRINGE", new AzureFunctions.Astute.Models.Case { ProgramName = "So Simple", Ndc = "57894007001", Provider = dummyProvider } },
                { "So Simple:SIMPONI 50 MG/0.5 ML PEN INJECT", new AzureFunctions.Astute.Models.Case { ProgramName = "So Simple", Ndc = "57894007002", Provider = dummyProvider } },
                { "So Simple:SIMPONI 100 MG/ML SYRINGE", new AzureFunctions.Astute.Models.Case { ProgramName = "So Simple", Ndc = "57894007101", Provider = dummyProvider } },
                { "So Simple:SIMPONI 100 MG/ML PEN INJECTOR", new AzureFunctions.Astute.Models.Case { ProgramName = "So Simple", Ndc = "57894007102", Provider = dummyProvider } },
                { "So Simple:STELARA 45 MG/0.5 ML VIAL", new AzureFunctions.Astute.Models.Case { ProgramName = "So Simple", Ndc = "57894006002", Provider = dummyProvider } },
                { "So Simple:STELARA 45 MG/0.5 ML SYRINGE", new AzureFunctions.Astute.Models.Case { ProgramName = "So Simple", Ndc = "57894006003", Provider = dummyProvider } },
                { "So Simple:STELARA 90 MG/ML SYRINGE", new AzureFunctions.Astute.Models.Case { ProgramName = "So Simple", Ndc = "57894006103", Provider = dummyProvider } }
            };

            var astuteServiceEvents = new Dictionary<string, PatientEvent>
            {
                { "SIMPONI 50 MG/0.5 ML SYRINGE:now:SoSimple:O", new PatientEvent { DrugName = "SIMPONI 50 MG/0.5 ML SYRINGE", DateAdded = now, ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusOpen } },
                { "SIMPONI 50 MG/0.5 ML SYRINGE:now-1:SoSimple:O", new PatientEvent { DrugName = "SIMPONI 50 MG/0.5 ML SYRINGE", DateAdded = now.AddDays(-1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusOpen } },
                { "SIMPONI 50 MG/0.5 ML SYRINGE:now+1:SoSimple:O", new PatientEvent { DrugName = "SIMPONI 50 MG/0.5 ML SYRINGE", DateAdded = now.AddDays(1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusOpen } },
                { "SIMPONI 50 MG/0.5 ML SYRINGE:now:SoSimple:C", new PatientEvent { DrugName = "SIMPONI 50 MG/0.5 ML SYRINGE", DateAdded = now, ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusClosed } },
                { "SIMPONI 50 MG/0.5 ML SYRINGE:now-1:SoSimple:C", new PatientEvent { DrugName = "SIMPONI 50 MG/0.5 ML SYRINGE", DateAdded = now.AddDays(-1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusClosed } },
                { "SIMPONI 50 MG/0.5 ML SYRINGE:now+1:SoSimple:C", new PatientEvent { DrugName = "SIMPONI 50 MG/0.5 ML SYRINGE", DateAdded = now.AddDays(1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusClosed } },
                { "STELARA 45 MG/0.5 ML VIAL:now:SoSimple:O", new PatientEvent { DrugName = "STELARA 45 MG/0.5 ML VIAL", DateAdded = now, ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusOpen } },
                { "STELARA 45 MG/0.5 ML VIAL:now-1:SoSimple:O", new PatientEvent { DrugName = "STELARA 45 MG/0.5 ML VIAL", DateAdded = now.AddDays(-1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusOpen } },
                { "STELARA 45 MG/0.5 ML VIAL:now+1:SoSimple:O", new PatientEvent { DrugName = "STELARA 45 MG/0.5 ML VIAL", DateAdded = now.AddDays(1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusOpen } },
                { "STELARA 45 MG/0.5 ML VIAL:now:SoSimple:C", new PatientEvent { DrugName = "STELARA 45 MG/0.5 ML VIAL", DateAdded = now, ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusClosed } },
                { "STELARA 45 MG/0.5 ML VIAL:now-1:SoSimple:C", new PatientEvent { DrugName = "STELARA 45 MG/0.5 ML VIAL", DateAdded = now.AddDays(-1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusClosed } },
                { "STELARA 45 MG/0.5 ML VIAL:now+1:SoSimple:C", new PatientEvent { DrugName = "STELARA 45 MG/0.5 ML VIAL", DateAdded = now.AddDays(1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusClosed } },
                { "TREMFYA 100 MG/ML SYRINGE:now:SoSimple:O", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = now, ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusOpen } },
                { "TREMFYA 100 MG/ML SYRINGE:now-1:SoSimple:O", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = now.AddDays(-1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusOpen } },
                { "TREMFYA 100 MG/ML SYRINGE:now+1:SoSimple:O", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = now.AddDays(1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusOpen } },
                { "TREMFYA 100 MG/ML SYRINGE:9-1-2020:SoSimple:O", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = new DateTime(2020, 9,1,1,1,1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusOpen } },
                { "TREMFYA 100 MG/ML SYRINGE:10-1-2020:SoSimple:O", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = new DateTime(2020, 10,1,1,1,1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusOpen } },
                { "TREMFYA 100 MG/ML SYRINGE:1-1-2021:SoSimple:O", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = new DateTime(2021, 1,1,1,1,1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusOpen } },
                { "TREMFYA 100 MG/ML SYRINGE:now:SoSimple:C", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = now, ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusClosed } },
                { "TREMFYA 100 MG/ML SYRINGE:now-1:SoSimple:C", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = now.AddDays(-1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusClosed } },
                { "TREMFYA 100 MG/ML SYRINGE:now+1:SoSimple:C", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = now.AddDays(1), ProgramHeader = AstuteConstants.SoSimple, IssueStatus = AstuteConstants.StatusClosed } },
                { "TREMFYA 100 MG/ML SYRINGE:now:Link:O", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = now, ProgramHeader = AstuteConstants.Link, IssueStatus = AstuteConstants.StatusOpen } },
                { "TREMFYA 100 MG/ML SYRINGE:now-1:Link:O", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = now.AddDays(-1), ProgramHeader = AstuteConstants.Link, IssueStatus = AstuteConstants.StatusOpen } },
                { "TREMFYA 100 MG/ML SYRINGE:now-1:Link:C", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = now.AddDays(-1), ProgramHeader = AstuteConstants.Link, IssueStatus = AstuteConstants.StatusClosed } },
                { "TREMFYA 100 MG/ML SYRINGE:now:RPh Only:O", new PatientEvent { DrugName = "TREMFYA 100 MG/ML SYRINGE", DateAdded = now, ProgramHeader = "RPh Only", IssueStatus = AstuteConstants.StatusOpen } },
            };

            var astuteServiceCases = new Dictionary<string, PatientCase>
            {
                { "TREMFYA:One Open Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:SoSimple:O"] } } },
                { "SIMPONI:One Open Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["SIMPONI 50 MG/0.5 ML SYRINGE:now:SoSimple:O"] } } } ,
                { "STELARA:One Open Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["STELARA 45 MG/0.5 ML VIAL:now:SoSimple:O"] } } } ,
                { "TREMFYA:Two Open Events", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:SoSimple:O"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:SoSimple:O"] } } } ,
                { "SIMPONI:Two Open Events", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["SIMPONI 50 MG/0.5 ML SYRINGE:now:SoSimple:O"], astuteServiceEvents["SIMPONI 50 MG/0.5 ML SYRINGE:now-1:SoSimple:O"] } } } ,
                { "STELARA:Two Open Events", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["STELARA 45 MG/0.5 ML VIAL:now:SoSimple:O"], astuteServiceEvents["STELARA 45 MG/0.5 ML VIAL:now-1:SoSimple:O"] } } } ,
                { "TREMFYA:Three Open Events", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:SoSimple:O"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:SoSimple:O"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now+1:SoSimple:O"] } } },
                { "TREMFYA:One Open Event:10-1-2020", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:10-1-2020:SoSimple:O"] } } },
                { "TREMFYA:One Open Event:1-1-2021", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:1-1-2021:SoSimple:O"] } } },
                { "TREMFYA:One Closed One Open Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:SoSimple:C"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:SoSimple:O"] } } },
                { "TREMFYA:One Closed Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:SoSimple:C"] } } },
                { "TREMFYA:Two Closed Events", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:SoSimple:C"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:SoSimple:C"] } } },
                { "TREMFYA:One Open One Closed Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:SoSimple:O"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:SoSimple:C"] } } },
                { "TREMFYA:One Open Link Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:Link:O"] } } },
                { "TREMFYA:Two Open Link Events", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:Link:O"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:Link:O"] } } },
                { "TREMFYA:One Closed Link One Open Link Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:Link:C"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:Link:O"] } } },
                { "TREMFYA:One Open SoSimple Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:SoSimple:O"] } } },
                { "TREMFYA:Two Open SoSimple Events", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:SoSimple:O"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:SoSimple:O"] } } },
                { "TREMFYA:One Open Link One Open SoSimple Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:Link:O"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:SoSimple:O"] } } },
                { "TREMFYA:One Closed One Open SoSimple Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:SoSimple:C"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:SoSimple:O"] } } },
                { "TREMFYA:One Closed Link One Open SoSimple Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:Link:C"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:SoSimple:O"] } } },
                { "TREMFYA:One Open SoSimple One Open RPh Only Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:SoSimple:O"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:RPh Only:O"] } } },
                { "TREMFYA:One Open Link One Open RPh Only Event", new PatientCase { CaseId = 2, PatientEvents = new PatientEvent[] { astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now-1:Link:O"], astuteServiceEvents["TREMFYA 100 MG/ML SYRINGE:now:RPh Only:O"] } } },
            };

            return new List<object[]>
            {

                //1
                new object[] { astuteInput["So Simple:TREMFYA 100 MG/ML SYRINGE"], null, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = string.Empty } },
                //2
                new object[] { astuteInput["So Simple:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["SIMPONI:One Open Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = string.Empty } },
                //3
                new object[] { astuteInput["So Simple:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Open Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusDuplicateCase } },
                new object[] { astuteInput["So Simple:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:Two Open Events"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusDuplicateCase } },
                new object[] { astuteInput["So Simple:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Closed One Open Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusDuplicateCase } },
                //4
                new object[] { astuteInput["So Simple:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Closed Event"] }, new { CaseStatus = AstuteConstants.StatusClosed, ExceptionReason = string.Empty } },
                new object[] { astuteInput["So Simple:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:Two Closed Events"] }, new { CaseStatus = AstuteConstants.StatusClosed, ExceptionReason = string.Empty } },
                new object[] { astuteInput["So Simple:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Open One Closed Event"] }, new { CaseStatus = AstuteConstants.StatusClosed, ExceptionReason = string.Empty } },
                //5                
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], null, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = string.Empty } },
                //6
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["SIMPONI:One Open Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = string.Empty } },
                //7
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Closed Event"] }, new { CaseStatus = AstuteConstants.StatusClosed, ExceptionReason = string.Empty } },
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:Two Closed Events"] }, new { CaseStatus = AstuteConstants.StatusClosed, ExceptionReason = string.Empty } },
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Open One Closed Event"] }, new { CaseStatus = AstuteConstants.StatusClosed, ExceptionReason = string.Empty } },
                //8
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Open Link Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusDuplicateCase } },
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:Two Open Link Events"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusDuplicateCase } },
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Closed Link One Open Link Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusDuplicateCase } },
                //9
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Open SoSimple Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusLinkReview } },
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:Two Open SoSimple Events"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusLinkReview } },
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Open Link One Open SoSimple Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusLinkReview } },
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Closed One Open SoSimple Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusLinkReview } },
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Closed Link One Open SoSimple Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusLinkReview } },

                //other tests
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Open Event:10-1-2020"], astuteServiceCases["TREMFYA:One Open Event:1-1-2021"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusLinkReview } },
                new object[] { astuteInput["So Simple:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Open SoSimple One Open RPh Only Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusDuplicateCase } },
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Open SoSimple One Open RPh Only Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusLinkReview } },
                new object[] { astuteInput["Link:TREMFYA 100 MG/ML SYRINGE"], new List<PatientCase> { astuteServiceCases["TREMFYA:One Open Link One Open RPh Only Event"] }, new { CaseStatus = AstuteConstants.StatusOpen, ExceptionReason = AstuteConstants.CaseStatusDuplicateCase } },
            };
        }
    }
}
