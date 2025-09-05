using Moq;
using Wegmans.POS.DataHub.ReprocessTransactionsNew;
using Wegmans.POS.DataHub.ReprocessTransactionsNew.ReprocessTransactionNewHelper;
using Wegmans.POS.DataHub.Util.EasternStandardTimeGenerator;

namespace Wegmans.POS.DataHub.UnitTests.ReprocessTransactionsNew.ReprocessTransactionNewHelper
{
    public class ReprocessTransactionHelperImpTests
    {
        private readonly ReprocessTransactionHelperImp _sut;

        private readonly Mock<IEasternStandardTimeGenerator> _mockEasternStandardTimeGenerator = new();

        public ReprocessTransactionHelperImpTests()
        {
            _sut = new(_mockEasternStandardTimeGenerator.Object);
        }

        [Theory]
        [ClassData(typeof(TestData))]
        public void GetMaxTransactionsToQueue_Returns_ExpectedResults(ReprocessingSchedule schedule, DateTimeOffset currentTime, int expectedResult)
        {
            // Arrange
            _mockEasternStandardTimeGenerator.Setup(x => x.GetCurrentEasternStandardTime())
                .Returns(currentTime);

            // Act
            int actualResult = _sut.GetMaxTransactionsToQueue(schedule);

            // Assert
            Assert.Equal(expectedResult, actualResult);
        }

        private class TestData : TheoryData<ReprocessingSchedule, DateTimeOffset, int>
        {
            public TestData()
            {
                ReprocessingSchedule schedule1 = new()
                {
                    Schedules = new List<Schedule> {
                        new Schedule {
                            StartTime = new TimeOnly(0, 0),
                            EndTime = new TimeOnly(6, 0),
                            NumberToQueuePerInterval = 10
                        },
                        new Schedule {
                            StartTime = new TimeOnly(6, 0),
                            EndTime = new TimeOnly(12, 0),
                            NumberToQueuePerInterval = 20
                        },
                        new Schedule {
                            StartTime = new TimeOnly(12, 0),
                            EndTime = TimeOnly.MaxValue,
                            NumberToQueuePerInterval = 5
                        },
                    }
                };

                AddRow(schedule1, new DateTimeOffset(2024, 10, 25, 6, 15, 0, TimeSpan.FromHours(-4)), 20);
                AddRow(schedule1, new DateTimeOffset(2024, 10, 25, 6, 0, 0, TimeSpan.FromHours(-4)), 20);
                AddRow(schedule1, new DateTimeOffset(2024, 10, 25, 5, 59, 59, TimeSpan.FromHours(-4)), 10);
                AddRow(schedule1, new DateTimeOffset(2024, 10, 25, 10, 0, 0, TimeSpan.FromHours(-4)), 20);
                AddRow(schedule1, new DateTimeOffset(2024, 10, 25, 11, 59, 59, TimeSpan.FromHours(-4)), 20);
                AddRow(schedule1, new DateTimeOffset(2024, 10, 25, 12, 0, 0, TimeSpan.FromHours(-4)), 5);
                AddRow(schedule1, new DateTimeOffset(2024, 10, 25, 0, 0, 0, TimeSpan.FromHours(-4)), 10);
                AddRow(schedule1, new DateTimeOffset(2024, 10, 25, 23, 59, 59, TimeSpan.FromHours(-4)), 5);
                AddRow(schedule1, new DateTimeOffset(2024, 10, 25, 19, 0, 0, TimeSpan.FromHours(-4)), 5);
                AddRow(schedule1, new DateTimeOffset(2024, 10, 25, 23, 0, 0, TimeSpan.FromHours(-4)), 5);

                ReprocessingSchedule schedule2 = new()
                {
                    Schedules = new List<Schedule> {
                        new Schedule {
                            StartTime = new TimeOnly(0, 0),
                            EndTime = new TimeOnly(6, 0),
                            NumberToQueuePerInterval = 10
                        },
                        new Schedule {
                            StartTime = new TimeOnly(6, 0),
                            EndTime = new TimeOnly(12, 0),
                            NumberToQueuePerInterval = 20
                        },
                    }
                };

                AddRow(schedule2, new DateTimeOffset(2024, 10, 25, 4, 15, 0, TimeSpan.FromHours(-4)), 10);
                AddRow(schedule2, new DateTimeOffset(2024, 10, 25, 6, 15, 0, TimeSpan.FromHours(-4)), 20);
                AddRow(schedule2, new DateTimeOffset(2024, 10, 25, 11, 59, 59, TimeSpan.FromHours(-4)), 20);
                AddRow(schedule2, new DateTimeOffset(2024, 10, 25, 12, 0, 0, TimeSpan.FromHours(-4)), 0);
                AddRow(schedule2, new DateTimeOffset(2024, 10, 25, 15, 0, 0, TimeSpan.FromHours(-4)), 0);

                ReprocessingSchedule schedule3 = new()
                {
                    Schedules = new List<Schedule> {
                        new Schedule {
                            StartTime = new TimeOnly(0, 0),
                            EndTime = new TimeOnly(8, 20),
                            NumberToQueuePerInterval = 10
                        },
                        new Schedule {
                            StartTime = new TimeOnly(8, 20),
                            EndTime = new TimeOnly(12, 0),
                            NumberToQueuePerInterval = 20
                        },
                    }
                };

                AddRow(schedule3, new DateTimeOffset(2024, 10, 25, 8, 10, 0, TimeSpan.FromHours(-4)), 10);
                AddRow(schedule3, new DateTimeOffset(2024, 10, 25, 8, 20, 0, TimeSpan.FromHours(-4)), 20);
                AddRow(schedule3, new DateTimeOffset(2024, 10, 25, 8, 30, 0, TimeSpan.FromHours(-4)), 20);
            }
        }
    }
}
