using Library.EmplifiInterface.Exceptions;
using Library.EmplifiInterface.Helper;
using Xunit;

namespace ZZZTest.Library.EmplifiInterface.Helper;

public class JpapDispenseAndStatusHelperImpTests
{
    private readonly JpapDispenseAndStatusHelperImp _sut;

    public JpapDispenseAndStatusHelperImpTests()
    {
        _sut = new JpapDispenseAndStatusHelperImp();
    }

    [Theory]
    [InlineData("06/17/2024 08:30:00", "06/14/2024 14:30:00", "06/17/2024 08:30:00")]
    [InlineData("06/17/2024 09:00:00", "06/14/2024 14:30:00", "06/17/2024 08:30:00")]
    [InlineData("06/17/2024 14:30:00", "06/17/2024 08:30:00", "06/17/2024 14:30:00")]
    [InlineData("06/17/2024 15:00:00", "06/17/2024 08:30:00", "06/17/2024 14:30:00")]
    [InlineData("06/17/2024 23:59:59", "06/17/2024 08:30:00", "06/17/2024 14:30:00")]
    [InlineData("06/18/2024 00:00:01", "06/17/2024 08:30:00", "06/17/2024 14:30:00")]
    [InlineData("06/18/2024 08:29:59", "06/17/2024 08:30:00", "06/17/2024 14:30:00")]
    [InlineData("06/18/2024 08:30:00", "06/17/2024 14:30:00", "06/18/2024 08:30:00")]
    [InlineData("06/18/2024 14:31:00", "06/18/2024 08:30:00", "06/18/2024 14:30:00")]
    [InlineData("06/19/2024 00:00:01", "06/18/2024 08:30:00", "06/18/2024 14:30:00")]
    [InlineData("06/19/2024 08:30:00", "06/18/2024 14:30:00", "06/19/2024 08:30:00")]
    [InlineData("06/19/2024 14:31:00", "06/19/2024 08:30:00", "06/19/2024 14:30:00")]
    [InlineData("06/20/2024 00:00:01", "06/19/2024 08:30:00", "06/19/2024 14:30:00")]
    [InlineData("06/20/2024 08:30:00", "06/19/2024 14:30:00", "06/20/2024 08:30:00")]
    [InlineData("06/20/2024 14:31:00", "06/20/2024 08:30:00", "06/20/2024 14:30:00")]
    [InlineData("06/21/2024 00:00:01", "06/20/2024 08:30:00", "06/20/2024 14:30:00")]
    [InlineData("06/21/2024 08:30:00", "06/20/2024 14:30:00", "06/21/2024 08:30:00")]
    [InlineData("06/21/2024 14:31:00", "06/21/2024 08:30:00", "06/21/2024 14:30:00")]
    public void GetDateRangeForOutboundStatus_ShouldReturnExpectedResult(string runForDateTime, string expectedStartDateTime, string expectedEndDateTime)
    {
        // Setup
        DateTime runFor = DateTime.Parse(runForDateTime);
        DateTime expectedStart = DateTime.Parse(expectedStartDateTime);
        DateTime expectedEnd = DateTime.Parse(expectedEndDateTime);

        // Act
        var actualResult = _sut.GetDateRangeForOutboundStatus(runFor);

        // Assert
        Assert.Equal(expectedStart, actualResult.startDateTime);
        Assert.Equal(expectedEnd, actualResult.endDateTime);
    }

    [Theory]
    [InlineData("06/16/2024 00:00:01", DayOfWeek.Sunday)]
    [InlineData("06/16/2024 08:00:00", DayOfWeek.Sunday)]
    [InlineData("06/16/2024 08:30:00", DayOfWeek.Sunday)]
    [InlineData("06/16/2024 09:00:00", DayOfWeek.Sunday)]
    [InlineData("06/16/2024 14:30:00", DayOfWeek.Sunday)]
    [InlineData("06/16/2024 15:00:00", DayOfWeek.Sunday)]
    [InlineData("06/22/2024 00:00:00", DayOfWeek.Saturday)]
    [InlineData("06/22/2024 08:00:00", DayOfWeek.Saturday)]
    [InlineData("06/22/2024 08:30:00", DayOfWeek.Saturday)]
    [InlineData("06/22/2024 09:00:00", DayOfWeek.Saturday)]
    [InlineData("06/22/2024 14:30:00", DayOfWeek.Saturday)]
    [InlineData("06/22/2024 15:00:00", DayOfWeek.Saturday)]
    public void GetDateRangeForOutboundStatus_ShouldThrowExceptionOnWeekends(string runForDateTime, DayOfWeek expectedDayOfWeek)
    {
        // Setup
        DateTime runFor = DateTime.Parse(runForDateTime);

        // Act
        InvalidRunDateException thrown = Assert.Throws<InvalidRunDateException>(() => _sut.GetDateRangeForOutboundStatus(runFor));

        // Assert
        Assert.Equal("Jpap Outbound Status job only runs M-F.", thrown.Message);
        Assert.Equal(expectedDayOfWeek, thrown.RunForDayOfWeek);
    }

    [Theory]
    [InlineData("06/17/2024 00:00:01", DayOfWeek.Monday)]
    [InlineData("06/17/2024 08:29:00", DayOfWeek.Monday)]
    public void GetDateRangeForOutboundStatus_ShouldThrowExceptionTooEarlyMondays(string runForDateTime, DayOfWeek expectedDayOfWeek)
    {
        // Setup
        DateTime runFor = DateTime.Parse(runForDateTime);

        // Act
        InvalidRunDateException thrown = Assert.Throws<InvalidRunDateException>(() => _sut.GetDateRangeForOutboundStatus(runFor));

        // Assert
        Assert.Equal("Cannot run before 8:30 AM on Mondays.", thrown.Message);
        Assert.Equal(expectedDayOfWeek, thrown.RunForDayOfWeek);
    }

    [Theory]
    [InlineData("06/20/2024 08:29:00", 1, 4)]
    public void GetOutboundStatusRequests_ShouldReturnExpectedResult(string runForDateTime, int expectedRequestListCount, int expectedFilterCount)
    {
        // Setup
        DateTime runFor = DateTime.Parse(runForDateTime);
        var dateRange = _sut.GetDateRangeForOutboundStatus(runFor);

        // Act
        var actualRequestList = _sut.GetOutboundStatusRequests(dateRange.startDateTime, dateRange.endDateTime);

        // Assert
        Assert.Equal(expectedRequestListCount, actualRequestList.Count());
        foreach (var actualRequest in actualRequestList)
        {
            Assert.Equal(expectedFilterCount, actualRequest.Count());
        }
    }
}
