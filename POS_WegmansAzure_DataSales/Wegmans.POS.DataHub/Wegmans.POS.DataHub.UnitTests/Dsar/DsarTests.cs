using FluentAssertions;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.TranslationEnumModel;
using Wegmans.POS.DataHub.Util;
using Wegmans.POS.DataHub.ACETransactionModel;
using Xunit;
using System.Xml;
using System.Xml.Schema;
using FluentAssertions.Xml;

namespace Wegmans.POS.DataHub.UnitTests.Dsar
{
    public class DsarTests
    {
        private const string _rawTlogContainerName = "raw-tlog";
        //readonly XmlDocument doc = new();

        [Theory]
        [InlineData("https://possalesdatahub.blob.core.windows.net/raw-tlog/2021/12/23/0111_0052_0025_1640263900772_transaction.xml")]
        public void GetTlogUriFromBlobContent_ShouldReturnPath_WhenStringIsValidTlogUri(string stringToTest)
        {

            var response = stringToTest.GetTlogUriFromBlobContent();

            response.Should().Be("2021/12/23/0111_0052_0025_1640263900772_transaction.xml");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("123456789")]
        [InlineData("https://possalesdatahub.blob.core.windows.net/raw-tlog/2021/12/23/07/0111_0052_0025.json")]
        [InlineData("https://possalesdatahub.blob.core.windows.net/raw-tlog/2021/12/23/0111_0052_0025.json")]
        public void GetTlogUriFromBlobContent_ShouldReturnEmptyString_WhenStringIsNotValid(string stringToTest)
        {

            var response = stringToTest.GetTlogUriFromBlobContent();

            response.Should().Be(String.Empty);
        }


        [Fact]
        public void DeidentifyCustomerIdFromTlog_ShouldReplaceCustomerAccountId_WhenItExists()
        {
            var doc = new XmlDocument();

            doc.LoadXml("<Transaction>\t\r\n\t<TransactionRecord>\r\n\t\t<StringType>11</StringType>\r\n\t\t<SubStringType>EE</SubStringType>\r\n\t\t<CustomerAccountID>1111114</CustomerAccountID>\r\n\t</TransactionRecord>\r\n</Transaction>");

            var response = doc.DeidentifyCustomerIdFromTlog();

            response.Should().NotBeNull();

            var testNode = response.SelectSingleNode("CustomerAccountId");

            if (testNode != null)
            testNode.InnerText.Should().BeEquivalentTo("1111111");
                
        }

        [Fact]
        public void DeidentifyCustomerIdFromTlog_ShouldDoNothing_WhenCustomerAccountIdDoesNotExist()
        {
            var doc = new XmlDocument();

            doc.LoadXml("<Transaction>\t\r\n\t<TransactionRecord>\r\n\t\t<StringType>11</StringType>\r\n\t\t<SubStringType>EE</SubStringType>\r\n\t\t<Number>1111114</Number>\r\n\t</TransactionRecord>\r\n</Transaction>");

            var response = doc.DeidentifyCustomerIdFromTlog();

            response.Should().BeEquivalentTo(doc);

        }

        [Fact]
        public void DeidentifyCustomerIdFromTlog_ShouldReplaceCustomerNumberAsEntered_WhenItExists()
        {
            var doc = new XmlDocument();

            doc.LoadXml("<Transaction>\t\r\n\t<TransactionRecord>\r\n\t\t<StringType>11</StringType>\r\n\t\t<SubStringType>EE</SubStringType>\r\n\t\t<CustomerNumberAsEntered>12345678900</CustomerNumberAsEntered>\r\n\t</TransactionRecord>\r\n</Transaction>");

            var response = doc.DeidentifyCustomerIdFromTlog();

            response.Should().NotBeNull();

            var testNode = response.SelectSingleNode("CustomerNumberAsEntered");

            if (testNode != null)
                testNode.InnerText.Should().BeEquivalentTo("11111111111");

        }

        [Fact]
        public void DeidentifyCustomerIdFromTlog_ShouldDoNothing_WhenCustomerNumberAsEnteredDoesNotExist()
        {
            var doc = new XmlDocument();

            doc.LoadXml("<Transaction>\t\r\n\t<TransactionRecord>\r\n\t\t<StringType>11</StringType>\r\n\t\t<SubStringType>EE</SubStringType>\r\n\t\t<Number>1111114</Number>\r\n\t</TransactionRecord>\r\n</Transaction>");

            var response = doc.DeidentifyCustomerIdFromTlog();

            response.Should().BeEquivalentTo(doc);

        }

        [Fact]
        public void DeidentifyCustomerIdFromTlog_ShouldReplaceCustomerNumberAsEnteredAndCustomerAccountId_WhenBothExist()
        {
            var doc = new XmlDocument();

            doc.LoadXml("<Transaction>\t\r\n\t<TransactionRecord>\r\n\t\t<StringType>11</StringType>\r\n\t\t<SubStringType>EE</SubStringType>\r\n\t\t<CustomerNumberAsEntered>12345678900</CustomerNumberAsEntered>\r\n\t</TransactionRecord>\r\n<TransactionRecord>\r\n\t\t<StringType>11</StringType>\r\n\t\t<SubStringType>EE</SubStringType>\r\n\t\t<CustomerAccountID>1111114</CustomerAccountID>\r\n\t</TransactionRecord>\r\n</Transaction>");

            var response = doc.DeidentifyCustomerIdFromTlog();

            response.Should().NotBeNull();

            var testNode = response.SelectSingleNode("CustomerNumberAsEntered");

            if (testNode != null)
                testNode.InnerText.Should().BeEquivalentTo("11111111111");

            var testNode2 = response.SelectSingleNode("CustomerAccountID");

            if (testNode2 != null)
                testNode2.InnerText.Should().BeEquivalentTo("1111111");

        }

        [Fact]
        public void DeidentifyCustomerIdFromTlog_ShouldDoNothing_WhenNeitherNodeExists()
        {
            var doc = new XmlDocument();

            doc.LoadXml("<Transaction>\t\r\n\t<TransactionRecord>\r\n\t\t<StringType>11</StringType>\r\n\t\t<SubStringType>EE</SubStringType>\r\n\t\t<Number>1111114</Number>\r\n\t</TransactionRecord>\r\n</Transaction>");

            var response = doc.DeidentifyCustomerIdFromTlog();

            response.Should().BeEquivalentTo(doc);

        }
    }
}