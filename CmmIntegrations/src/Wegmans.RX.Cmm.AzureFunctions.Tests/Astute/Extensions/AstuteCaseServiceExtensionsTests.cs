using FluentAssertions;
using System;
using System.Collections.Generic;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Extensions;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models;
using Wegmans.RX.Cmm.AzureFunctions.Astute.Models.Outbound;
using Xunit;

namespace Wegmans.RX.Cmm.AzureFunctions.Tests.Astute.Extensions
{
    public class AstuteCaseServiceExtensionsTests
    {
        private static readonly DateTime now = DateTime.Now;

        private static Dictionary<string, AstuteCaseService.Case> astuteCases = new Dictionary<string, AstuteCaseService.Case>
        {
            { "Blank Case", new AstuteCaseService.Case() },
            { "Multiple Issue No c16", new AstuteCaseService.Case { IssueList = new AstuteCaseService.IssueList { Issue = new AstuteCaseService.Issue[] { new AstuteCaseService.Issue(), new AstuteCaseService.Issue() } } } },
            { "Multiple Issue one c16 empty string", new AstuteCaseService.Case { 
                IssueList = new AstuteCaseService.IssueList { Issue = new AstuteCaseService.Issue[] { new AstuteCaseService.Issue { c16_code = string.Empty, c47_code = "SoSimple" }, new AstuteCaseService.Issue() } },
                AddressList = new AstuteCaseService.AddressList { Address = new AstuteCaseService.Address[] { new AstuteCaseService.Address { address_type_code = "PATIENT" } } } } },
            { "Multiple Issue one c16 no equals", new AstuteCaseService.Case {
                IssueList = new AstuteCaseService.IssueList { Issue = new AstuteCaseService.Issue[] { new AstuteCaseService.Issue { c16_code = "Test", c47_code = "SoSimple" }, new AstuteCaseService.Issue() } },
                AddressList = new AstuteCaseService.AddressList { Address = new AstuteCaseService.Address[] { new AstuteCaseService.Address { address_type_code = "PATIENT" } } } } },
            { "Multiple Issue one c16", new AstuteCaseService.Case { 
                IssueList = new AstuteCaseService.IssueList { Issue = new AstuteCaseService.Issue[] { new AstuteCaseService.Issue { c16_code = "Test=Test2", c47_code = "Link" }, new AstuteCaseService.Issue() } },
                AddressList = new AstuteCaseService.AddressList { Address = new AstuteCaseService.Address[] { new AstuteCaseService.Address { address_type_code = "PATIENT" } } }} },
            { "Multiple Issue one c16 no program header", new AstuteCaseService.Case { 
                IssueList = new AstuteCaseService.IssueList { Issue = new AstuteCaseService.Issue[] { new AstuteCaseService.Issue { c16_code = "Test=Test3", c47_code = "RPh Only" }, new AstuteCaseService.Issue() } },
                AddressList = new AstuteCaseService.AddressList { Address = new AstuteCaseService.Address[] { new AstuteCaseService.Address { address_type_code = "PATIENT" } } } } },
            { "Single Issue Good Data", new AstuteCaseService.Case { b18_code = "B18", b38_code = "Yes", b33_code = now.Date.ToString(),
                IssueList = new AstuteCaseService.IssueList { Issue = new AstuteCaseService.Issue[] { new AstuteCaseService.Issue { c12_code = now.ToString(), c16_code = "Test=Test2", c47_code = "SoSimple" } } },
                AddressList = new AstuteCaseService.AddressList { Address = new AstuteCaseService.Address[] { new AstuteCaseService.Address { address_type_code = "PATIENT", a07_code = "PbmName", a09_code = "PbmBin", a10_code = "PbmPcn", a11_code = "PbmGroup", a08_code = "PbmPlan" } } }} },
            { "Single Issue Bad Shipped Date", new AstuteCaseService.Case { 
                IssueList = new AstuteCaseService.IssueList { Issue = new AstuteCaseService.Issue[] { new AstuteCaseService.Issue { c12_code = "BadDateString", c16_code = "Test=Test2", c47_code = "Link" } } },
                AddressList = new AstuteCaseService.AddressList { Address = new AstuteCaseService.Address[] { new AstuteCaseService.Address { address_type_code = "PATIENT" } } }} },
            { "Single Issue b15 null", new AstuteCaseService.Case { b18_code = "B18", b16_code = "Phone", 
                IssueList = new AstuteCaseService.IssueList { Issue = new AstuteCaseService.Issue[] { new AstuteCaseService.Issue { c16_code = "Test=Test2", c47_code = "SoSimple" } } },
                AddressList = new AstuteCaseService.AddressList { Address = new AstuteCaseService.Address[] { new AstuteCaseService.Address { address_type_code = "PATIENT" } } }} },
            { "Single Issue b16 null", new AstuteCaseService.Case { b18_code = "B18", b15_code = "Name", 
                IssueList = new AstuteCaseService.IssueList { Issue = new AstuteCaseService.Issue[] { new AstuteCaseService.Issue { c16_code = "Test=Test2", c47_code = "Link" } } },
                AddressList = new AstuteCaseService.AddressList { Address = new AstuteCaseService.Address[] { new AstuteCaseService.Address { address_type_code = "PATIENT" } } }} },
        };

        [Theory]
        [MemberData(nameof(CreateAstutePatientStatusData))]
        public void CreateAstutePatientStatus(AstuteCaseService.Case astuteCase, PatientStatus expected)
        {
            var result = astuteCase.CreateAstutePatientStatus(DateTime.Now.AddMinutes(-5), DateTime.Now);

            result.Should().BeEquivalentTo(expected);
        }

        [Theory]
        [MemberData(nameof(CreatePatientCaseData))]
        public void CreatePatientCase(AstuteCaseService.Case astuteCase, PatientCase expected)
        {
            var result = astuteCase.CreatePatientCase();

            result.Should().BeEquivalentTo(expected);
        }

        public static IEnumerable<object[]> CreateAstutePatientStatusData()
        {
            return new List<object[]>
            {
                new object[] { astuteCases["Blank Case"], null },
                new object[] { astuteCases["Multiple Issue No c16"], null },
                new object[] { astuteCases["Multiple Issue one c16 empty string"], new PatientStatus { PatientStatusCode = string.Empty, LinkEnrollmentFlag = string.Empty } },
                new object[] { astuteCases["Multiple Issue one c16 no equals"], new PatientStatus { PatientStatusCode = "Test", LinkEnrollmentFlag = string.Empty } },
                new object[] { astuteCases["Multiple Issue one c16"], new PatientStatus { PatientStatusCode = "Test", LinkEnrollmentFlag = string.Empty } },
                new object[] { astuteCases["Multiple Issue one c16 no program header"], null },
                new object[] { astuteCases["Single Issue Good Data"], 
                    new PatientStatus { PatientStatusCode = "Test", TransferPharmacyNpi = "B18", LinkEnrollmentFlag = "y", LastShipmentDate = DateTime.Parse(now.ToString()), 
                        PrimaryPbmName = "PbmName", PrimaryPbmBin = "PbmBin", PrimaryPbmPcn = "PbmPcn", PrimaryPbmGroupId = "PbmGroup", PrimaryPbmPlanId = "PbmPlan", ReverificationStartDate = DateTime.Parse(now.Date.ToString()) } },
                new object[] { astuteCases["Single Issue Bad Shipped Date"], new PatientStatus { PatientStatusCode = "Test", LinkEnrollmentFlag = string.Empty } },
                new object[] { astuteCases["Single Issue b15 null"], new PatientStatus { PatientStatusCode = "Test", TransferPharmacyPhoneNumber = "Phone", TransferPharmacyNpi = string.Empty, LinkEnrollmentFlag = string.Empty } },
                new object[] { astuteCases["Single Issue b16 null"], new PatientStatus { PatientStatusCode = "Test", TransferPharmacyName = "Name", TransferPharmacyNpi = string.Empty, LinkEnrollmentFlag = string.Empty } },
            };
        }

        public static IEnumerable<object[]> CreatePatientCaseData()
        {
            return new List<object[]>
            {
                new object[] { astuteCases["Blank Case"], new PatientCase() }
            };
        }
    }
}
