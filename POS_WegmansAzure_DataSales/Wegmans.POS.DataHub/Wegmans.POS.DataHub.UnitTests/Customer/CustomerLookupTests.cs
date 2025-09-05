using FluentAssertions;
using NSubstitute;
using Wegmans.Enterprise.Services;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.Customer;

namespace Wegmans.POS.DataHub.UnitTests.Customer;

public class CustomerLookupTests
{
    private readonly ICustomerClient _customerClient = Substitute.For<ICustomerClient>();
    private readonly CustomerLookup _customerLookup;

    public CustomerLookupTests()
    {
        _customerClient = Substitute.For<ICustomerClient>();
        _customerLookup = new CustomerLookup(_customerClient);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123")]
    [InlineData("1234567890")]
    [InlineData("123456b")]
    public async Task AddCuid_ShouldReturnNullCustomerIdentifiers_WhenNoValidLoyaltyNumberIsPresent(string loyaltyNumber)
    {
        // Arrange
        var transaction = new Transaction() { CustomerIdentification = new CustomerIdentification() { LoyaltyNumber = loyaltyNumber } };
        var cancellationToken = new CancellationToken();

        // Act
        var result = await _customerLookup.AddCuid(transaction, cancellationToken);

        // Assert
        Assert.Null(result.CustomerIdentification?.Cuid);
        Assert.Null(result.CustomerIdentification?.LoyaltyNumber);
        Assert.Null(result.CustomerIdentification?.CustomerNumberAsEntered);
    }

    [Fact]
    public async Task AddCuid_ShouldReturnNullCustomerIdentifiers_WhenCustomerCollectionIsEmpty()
    {
        // Arrange
        var transaction = new Transaction() { CustomerIdentification = new CustomerIdentification() { LoyaltyNumber = "1234567" } };
        var cancellationToken = new CancellationToken();
        _customerClient.FindCustomersAsync(
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            transaction.CustomerIdentification.LoyaltyNumber,
            cancellationToken)
            .Returns(new CustomerCollection());

        // Act
        var result = await _customerLookup.AddCuid(transaction, cancellationToken);

        // Assert
        Assert.Null(result.CustomerIdentification?.Cuid);
        Assert.Null(result.CustomerIdentification?.LoyaltyNumber);
        Assert.Null(result.CustomerIdentification?.CustomerNumberAsEntered);
    }

    [Fact]
    public async Task AddCuid_ShouldReturnCuidAndClearLoyalty_WhenFindCustomersAsyncReturnsOneCustomer()
    {
        // Arrange
        var transaction = new Transaction() { CustomerIdentification = new CustomerIdentification() { LoyaltyNumber = "1234567" } };
        var cancellationToken = new CancellationToken();
        var testCustomer = new Enterprise.Services.Customer() { Id = Guid.NewGuid() };
        _customerClient.FindCustomersAsync(
            string.Empty,
            string.Empty,
            string.Empty,
            string.Empty,
            Arg.Any<string>(),
            cancellationToken)
            .Returns(new CustomerCollection() { Values = new List<Enterprise.Services.Customer>() { testCustomer } });

        // Act
        var result = await _customerLookup.AddCuid(transaction, cancellationToken);

        // Assert
        result.CustomerIdentification?.Cuid.Should().Be(testCustomer.Id.ToString());
        Assert.Null(result.CustomerIdentification?.LoyaltyNumber);
        Assert.Null(result.CustomerIdentification?.CustomerNumberAsEntered);
    }

    [Fact]
    public async Task AddCuid_ShouldReturnFirstCustomerCuidAndClearLoyalty_WhenFindCustomersAsyncReturnsMultipleCustomers()
    {
        // Arrange
        var transaction = new Transaction() { CustomerIdentification = new CustomerIdentification() { LoyaltyNumber = "1234567" } };
        var cancellationToken = new CancellationToken();
        var testCustomers = new CustomerCollection() { 
            Values = new List<Enterprise.Services.Customer>() {
                new() { Id = Guid.NewGuid() },
                new() { Id = Guid.NewGuid() },
            }
        };
        _customerClient.FindCustomersAsync(
                string.Empty,
                string.Empty,
                string.Empty,
                string.Empty,
                Arg.Any<string>(),
                cancellationToken)
                .Returns(testCustomers);

        // Act
        var result = await _customerLookup.AddCuid(transaction, cancellationToken);

        // Assert
        result.CustomerIdentification?.Cuid.Should().Be(testCustomers.Values.First().Id.ToString());
        Assert.Null(result.CustomerIdentification?.LoyaltyNumber);
        Assert.Null(result.CustomerIdentification?.CustomerNumberAsEntered);
    }

    [Theory]
    [InlineData("", "")]
    [InlineData(" ", " ")]
    [InlineData("123", "123")]
    [InlineData("12345678", "12345678")]
    [InlineData("123456b", "123456b")]
    [InlineData("012345", "12345")]
    [InlineData("01234567", "1234567")]
    [InlineData("001234567", "1234567")]
    [InlineData(null, "")]
    public void ParseLoyaltyNumber_ShouldReturnExpectedResult_WhenLoyaltyNumberIsPassed(string loyaltyNumber, string expectedResult)
    {
        // Arrange

        // Act
        var result = _customerLookup.ParseLoyaltyNumber(loyaltyNumber);

        // Assert
        result.Should().Be(expectedResult);
    }
}
