using FluentAssertions;
using System;
using System.Collections.Generic;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Models;
using Wegmans.RX.Cmm.AzureFunctions.Cmm.Validation;
using Xunit;

namespace Wegmans.RX.Cmm.AzureFunctions.Tests.Cmm.Validation
{
    public class CaseStatusValidatorTests
    {
        [Theory]
        [MemberData(nameof(CreateNewCaseStatusValidationData))]
        public void CaseStatusValidation(CaseStatus caseStatus, IEnumerable<string> expectedErrorList)
        {
            CaseStatusValidator validator = new CaseStatusValidator();

            var result = validator.Validate(caseStatus).ToString();

            var expectedErrorMessage = string.Join(Environment.NewLine, expectedErrorList);

            result.Should().BeEquivalentTo(expectedErrorMessage);
        }

        public static IEnumerable<object[]> CreateNewCaseStatusValidationData()
        {
            var now = DateTime.Now;

            var errorMessages = new Dictionary<string, string>
            {
                { "EmptyCaseId", "'Case Id' must not be empty." },
                { "EmptyIsCaseClosed", "'Is Case Closed' must not be empty." },
                { "EmptyNdc", "'Ndc' must not be empty." },
                { "EmptyCaseClosureReason", "'Case Closure Reason' must not be empty." },
                { "InvalidCaseClosureReason", "'Case Closure Reason' value is invalid." },
                { "EmptyLinkStatus", "'Link Status' must not be empty." },
                { "InvalidLinkStatus", "'Link Status' value is invalid." },
                { "InvalidLinkStatusReason", "'Link Status Reasons' collection has invalid value(s)." },
                { "Empty", "'' must not be empty." },
            };

            var cases = new Dictionary<string, CaseStatus>
            {
                { "Empty", new CaseStatus() },
                { "PassesAllValidationTests", new CaseStatus { CaseId = "Abc1234", IsCaseClosed = true, Ndc = "123456798", CaseClosureReason = "Case is duplicate", LinkStatus = "Link Pending" } },
                { "NullCaseId", new CaseStatus { IsCaseClosed = false, Ndc = "123456798", LinkStatus = "Link Pending" } },
                { "NullIsCaseClosed", new CaseStatus { CaseId = "Abc1234", Ndc = "123456798", LinkStatus = "Link Pending" } },
                { "NullNdc", new CaseStatus { CaseId = "Abc1234", IsCaseClosed = false, LinkStatus = "Link Pending" } },
                { "NullCaseClosureReasonTrueIsCaseClosed", new CaseStatus { CaseId = "Abc1234", IsCaseClosed = true, Ndc = "123456798", LinkStatus = "Link Pending" } },
                { "NullCaseClosureReasonFalseIsCaseClosed", new CaseStatus { CaseId = "Abc1234", IsCaseClosed = false, Ndc = "123456798", LinkStatus = "Link Pending" } },
                { "NullLinkStatus", new CaseStatus { CaseId = "Abc1234", IsCaseClosed = false, Ndc = "123456798" } },
                { "InvalidCaseClosureReason", new CaseStatus { CaseId = "Abc1234", IsCaseClosed = true, Ndc = "123456798", CaseClosureReason = "Invalid", LinkStatus = "Link Pending" } },
                { "InvalidLinkStatus", new CaseStatus { CaseId = "Abc1234", IsCaseClosed = false, Ndc = "123456798", LinkStatus = "Invalid" } },
                { "InvalidLinkStatusReasons", new CaseStatus { CaseId = "Abc1234", IsCaseClosed = false, Ndc = "123456798", LinkStatus = "Link Pending", LinkStatusReasons = new List<string> { "Challenge Open", "Invalid" } } },
                { "ValidLinkStatusReasons", new CaseStatus { CaseId = "Abc1234", IsCaseClosed = false, Ndc = "123456798", LinkStatus = "Link Pending", LinkStatusReasons = new List<string> { "Challenge Open", "Challenge Open" } } },
            };

            return new List<object[]>
            {
                //passes validation                
                new object[] { cases["PassesAllValidationTests"], new List<string> () },

                //empty Case                
                new object[] { cases["Empty"], new List<string> { errorMessages["EmptyCaseId"], errorMessages["EmptyIsCaseClosed"], errorMessages["EmptyNdc"], errorMessages["EmptyLinkStatus"] } },

                //null CaseId
                new object[] { cases["NullCaseId"], new List<string> { errorMessages["EmptyCaseId"] } },

                //null IsCaseClosed
                new object[] { cases["NullIsCaseClosed"], new List<string> { errorMessages["EmptyIsCaseClosed"] } },

                //null Ndc
                new object[] { cases["NullNdc"], new List<string> { errorMessages["EmptyNdc"] } },

                //null CaseClosureReason
                new object[] { cases["NullCaseClosureReasonTrueIsCaseClosed"], new List<string> { errorMessages["EmptyCaseClosureReason"] } },
                new object[] { cases["NullCaseClosureReasonFalseIsCaseClosed"], new List<string> () },

                //null LinkStatus
                new object[] { cases["NullLinkStatus"], new List<string> { errorMessages["EmptyLinkStatus"] } },

                //invalid CaseClosureReason
                new object[] { cases["InvalidCaseClosureReason"], new List<string> { errorMessages["InvalidCaseClosureReason"] } },

                //invalid LinkStatus
                new object[] { cases["InvalidLinkStatus"], new List<string> { errorMessages["InvalidLinkStatus"] } },

                //invalid LinkStatusReasons
                new object[] { cases["InvalidLinkStatusReasons"], new List<string> { errorMessages["InvalidLinkStatusReason"] } },

                //valid LinkStatusReasons
                new object[] { cases["ValidLinkStatusReasons"], new List<string>() },
            };
        }
    }
}
