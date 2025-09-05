using FluentAssertions;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.TranslationEnumModel;
using Wegmans.POS.DataHub.Util;
using Wegmans.POS.DataHub.ACETransactionModel;
using Xunit;

namespace Wegmans.POS.DataHub.UnitTests.TlogSubscriber
{
    public class TlogSubscriberExtensionsTest
    {
        [Theory]
        [InlineData("51")]
        [InlineData("9999")]
        [InlineData("")]
        public void ConvertToTerminalType_ShouldReturnNull_WhenLaneIdIsNotValid(string idToTest)
        {
            var response = idToTest.ConvertToTerminalType();
            response.Should().BeNull();
        }

        [Theory]
        [InlineData("1", TerminalType.FrontEnd)]
        [InlineData("41", TerminalType.Wine)]
        [InlineData("261", TerminalType.CartToCurb)]
        [InlineData("281", TerminalType.BurgerBar)]
        [InlineData("301", TerminalType.FrontEndWireless)]
        [InlineData("701", TerminalType.Payment)]
        public void ConvertToTerminalType_ShouldReturnCorrectTerminalType_WhenLaneIdIsPassed(string idToTest, TerminalType expectedTerminalType)
        {
            var actualTerminalType = idToTest.ConvertToTerminalType();
            actualTerminalType.Should().Be(expectedTerminalType);
        }


        [Theory]
        [InlineData(5)]
        [InlineData(9999)]
        [InlineData(0)]
        public void ConvertToRefundReason_ShouldReturnNull_WhenIdIsNotValid(int idToTest)
        {
            var response = idToTest.ConvertToRefundReason();
            response.Should().BeNull();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(94)]
        public void ConvertToRefundReason_ShouldReturnCorrectTerminalType_WhenIdIsPassed(int idToTest)
        {
            var response = idToTest.ConvertToRefundReason();

            List<RefundReason> myListRR = new List<RefundReason>() { RefundReason.ReShopped, RefundReason.NoReshopCredits, RefundReason.NoReshopShrink, RefundReason.NoReshopRecalls, RefundReason.FullRefundRxChange };

            response.Should().BeOneOf(myListRR);
        }

        [Theory]
        [InlineData("0")]
        public void ToNullableString_ShouldReturnNull_WhenStringIsZero(string stringToTest)
        {
            var response = stringToTest.ToNullableString();
            response.Should().BeNull();
        }

        [Theory]
        [InlineData("51")]
        [InlineData("test")]
        [InlineData(" ")]
        public void ToNullableString_ShouldReturnString_WhenStringIsNotZero(string stringToTest)
        {
            var response = stringToTest.ToNullableString();
            response.Should().BeEquivalentTo(stringToTest);
        }

        [Theory]
        [InlineData(0)]
        public void ToNullableInt_ShouldReturnNull_WhenIntIsZero(int intToTest)
        {
            var response = intToTest.ToNullableInt();
            response.Should().BeNull();
        }

        [Theory]
        [InlineData(-51)]
        [InlineData(9999)]
        [InlineData(1.0)]
        public void ToNullableInt_ShouldReturnInt_WhenIntIsNotZero(int intToTest)
        {
            var response = intToTest.ToNullableInt();
            response.Should().Be(intToTest);
        }

        [Theory]
        [InlineData(0)]
        public void Divide_ShouldReturnNull_WhenDecimalIsZero(decimal decimalToTest)
        {
            var response = decimalToTest.Divide();
            response.Should().BeNull();
        }

        [Theory]
        [InlineData(-51.2)]
        [InlineData(9999)]
        [InlineData(1.0)]
        public void Divide_ShouldReturnDecimal_WhenDecimalIsNotZero(decimal decimalToTest)
        {
            var response = decimalToTest.Divide();
            Assert.NotEqual(0, decimalToTest);
            response.Should().Be(decimalToTest/100);
        }

        [Theory]
        [InlineData(0)]
        public void DivideDouble_ShouldReturnNull_WhenDoubleIsZero(double doubleToTest)
        {
            var response = doubleToTest.DivideDouble();
            response.Should().BeNull();
        }

        [Theory]
        [InlineData(-51.2)]
        [InlineData(9999)]
        [InlineData(1.0)]
        public void DivideDouble_ShouldReturnDouble_WhenDoubleIsNotZero(double doubleToTest)
        {
            var response = doubleToTest.DivideDouble();
            Assert.NotEqual(0,doubleToTest);
            response.Should().Be(doubleToTest / (double)100);
        }

        [Theory]
        [InlineData("0123xxx")]
        public void getStoreNumber_ShouldReturnStoreNumber_WhenStringIsValid(string stringToTest)
        {

            var response = stringToTest.getStoreNumber();
            response.Should().Be(Int32.Parse(stringToTest.Substring(0, 4)));
        }



        [Theory]
        [InlineData("0123")]
        [InlineData("0")]
        public void getStoreNumber_ShouldReturnException_WhenStringIsNotValid(string stringToTest)
        {
            Action response = () => stringToTest.getStoreNumber();

            response.Should().Throw<ArgumentNullException>(nameof(stringToTest));

        }

        [Theory]
        [InlineData("2")]
        public void getConvertedSeconds_ShouldReturnSeconds_WhenStringIsValid(string timeToTest)
        {
            var response = timeToTest.getConvertedSeconds();
            Double.TryParse(timeToTest, out var myseconds);
            response.Should().Be(TimeSpan.FromSeconds(myseconds));
        }

        [Theory]
        [InlineData("a")]
        [InlineData(" ")]
        [InlineData("")]
        public void getConvertedSeconds_ShouldReturnNull_WhenStringIsNotValid(string timeToTest)
        {
            var response = timeToTest.getConvertedSeconds();
            response.Should().BeNull();
        }

        [Theory]
        [InlineData(false)]
        public void ToNullableBoolean_ShouldReturnNull_WhenBooleanIsFalse(Boolean BooleanToTest)
        {
            var response = BooleanToTest.ToNullableBoolean();
            response.Should().BeNull();
        }

        [Theory]
        [InlineData(true)]
        public void ToNullableBoolean_ShouldReturnTrue_WhenBooleanIsTrue(Boolean BooleanToTest)
        {
            var response = BooleanToTest.ToNullableBoolean();
            response.Should().BeTrue();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]

        public void getTenderTypeName_ShouldReturnNonAvailable_WhenStringCanNotBeParsedAsInt(string stringToTest)
        {
            var response = stringToTest.getTenderTypeName();

            response.Should().Be(String05TenderTypeNameEnum.NotAvailable);
        }

        [Theory]
        [InlineData("11")]
        [InlineData("16")]
        [InlineData("-1")]

        public void getTenderTypeName_ShouldReturnName_WhenStringIsInEnum(string stringToTest)
        {
            var response = stringToTest.getTenderTypeName();
            Assert.True(int.TryParse(stringToTest, out var number));
            response.Should().Be((String05TenderTypeNameEnum)number);
        }
 
        [Theory]
        [InlineData(true,"-1")]
        [InlineData(true, "0")]
        [InlineData(true, "9999")]
        public void getQuantityValue_ShouldReturnInt_WhenBooleanIsTrue(bool booleanToTest, string quantity) //returns quantity value if QtyKey was pressed otherwise return 
        {
            TransactionRecord02 recordToTest = new TransactionRecord02();
            recordToTest.QuantityOrWeightOrVolume = quantity;
            var response = booleanToTest.ToNullableBoolean().getQuantityValue(recordToTest);
            Assert.True(int.TryParse(quantity, out var number));
            response.Should().Be(number);
        }

        [Theory]
        [InlineData(false, "-51")]
        [InlineData(false, "9999")]
        [InlineData(false, "0")]
        public void getQuantityValue_ShouldReturn1_WhenBooleanIsFalse(bool booleanToTest, string quantity)
        {
            TransactionRecord02 recordToTest = new TransactionRecord02();
            recordToTest.QuantityOrWeightOrVolume = quantity;
            var response = booleanToTest.ToNullableBoolean().getQuantityValue(recordToTest);
            Assert.True(int.TryParse(quantity, out var number));
            response.Should().Be(1);
        }

        [Theory]
        [InlineData(true, "-1")]
        [InlineData(true, "0")]
        [InlineData(true, "9999")]
        public void getWeightValue_ShouldReturnDecimal_WhenBooleanIsTrue(bool booleanToTest, string weight) //returns weight value if QtyKey was pressed otherwise return null
        {
            TransactionRecord02 recordToTest = new TransactionRecord02();
            recordToTest.QuantityOrWeightOrVolume = weight;
            var response = booleanToTest.ToNullableBoolean().getWeightValue(recordToTest);
            response.Should().Be(Decimal.Divide(Int32.Parse(weight), 100));
        }

        [Theory]
        [InlineData(false, "-51")]
        [InlineData(false, "9999")]
        [InlineData(false, "0")]
        public void getWeightValue_ShouldReturnNull_WhenBooleanIsFalse(bool booleanToTest, string weight)
        {
            TransactionRecord02 recordToTest = new TransactionRecord02();
            recordToTest.QuantityOrWeightOrVolume = weight;
            var response = booleanToTest.ToNullableBoolean().getWeightValue(recordToTest);
            response.Should().BeNull();
        }

        [Theory]
        [InlineData("123456789")]
        public void removeCustomerData_ShouldRemoveLoyaltyNumber_WhenPresent(string loyaltyNumber)
        {
            Transaction recordToTest = new Transaction();
            recordToTest.CustomerIdentification = new CustomerIdentification() { LoyaltyNumber = loyaltyNumber };
           
            var response = recordToTest.removeCustomerData();
            response.CustomerIdentification?.LoyaltyNumber.Should().Be("0");
        }
    }
}