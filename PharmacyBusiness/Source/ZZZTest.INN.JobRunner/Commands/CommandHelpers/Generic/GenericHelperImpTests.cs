using INN.JobRunner.Commands.CommandHelpers.Generic;
using INN.JobRunner.CommonParameters;
using INN.JobRunner.Exceptions;

namespace ZZZTest.INN.JobRunner.Commands.CommandHelpers.Generic
{
    public class GenericHelperImpTests
    {
        private readonly GenericHelperImp _sut;

        public GenericHelperImpTests()
        {
            _sut = new GenericHelperImp();
        }

        /// <summary>
        /// Tests that <see cref="ControlMConsoleApp.StartAsync(CancellationToken)"/> runs the task when <see cref="ControlMConsoleApp.CannotRunBeforeOrOn"/> is null or valid
        /// </summary>
        /// <returns></returns>
        [Theory]
        [InlineData("2024-02-16", null, "2024-02-15")]
        [InlineData("2025-01-01", null, "2024-02-15")]
        [InlineData("2024-02-20", null, "2024-02-13")]
        [InlineData("2020-02-20", "2024-02-25", "2020-02-19")]
        [InlineData("2024-02-20", "2024-02-25", "2024-02-19")]
        public void CheckRunForDate_ReturnsNothing_WhenDisableRunBeforeValIsNullOrAfterRunDateDate(string runFor, string? runTill, string cannotRunBeforeOrOn)
        {
            // Arrange
            RunForParameter runForParameter = new RunForParameter
            {
                RunFor = DateOnly.Parse(runFor),
                RunTill = runTill == null ? null : DateOnly.Parse(runTill),
            };

            // Act
            _sut.CheckRunForDate(runForParameter, DateOnly.Parse(cannotRunBeforeOrOn));
        }

        /// <summary>
        /// Tests that <see cref="ControlMConsoleApp.StartAsync(CancellationToken)"/> runs the task when <see cref="ControlMConsoleApp.CannotRunBeforeOrOn"/> is null or valid
        /// </summary>
        /// <returns></returns>
        [Theory]
        [InlineData("2024-02-15", null, "2024-02-15")]
        [InlineData("2024-02-14", null, "2024-02-15")]
        [InlineData("2020-01-01", null, "2024-02-15")]
        [InlineData("2024-02-15", "2024-02-19", "2024-02-15")]
        [InlineData("2024-02-13", "2024-02-21", "2024-02-15")]
        [InlineData("2024-02-13", "2024-02-21", "2024-02-21")]
        public void CheckRunForDate_ThrowsException_WhenDisableRunValIsBeforeRunDate(string runFor, string? runTill, string cannotRunBeforeOrOn)
        {
            // Arrange
            RunForParameter runForParameter = new RunForParameter
            {
                RunFor = DateOnly.Parse(runFor),
                RunTill = runTill == null ? null : DateOnly.Parse(runTill),
            };

            // Act
            InvalidRunDateException thrown = Assert.Throws<InvalidRunDateException>(() => _sut.CheckRunForDate(runForParameter, DateOnly.Parse(cannotRunBeforeOrOn)));

            // Assert
            Assert.Equal(DateOnly.Parse(runFor), thrown.RunFor);
        }

        [Theory]
        [InlineData("2024-02-15", null)]
        [InlineData("2024-02-14", null)]
        [InlineData("2020-01-01", null)]
        [InlineData("2024-02-15", "2024-02-19")]
        [InlineData("2024-02-13", "2024-02-21")]
        public async Task RunFromToRunTillAsync_CallsGivenFunction_ForEachDateBetween(string runFor, string? runTill)
        {
            // Arrange
            RunForParameter runForParameter = new RunForParameter
            {
                RunFor = DateOnly.Parse(runFor),
                RunTill = runTill == null ? null : DateOnly.Parse(runTill),
            };
            ISet<DateOnly> datesRunFor = new HashSet<DateOnly>();
            Func<DateOnly, Task> testFunc = (date) =>
            {
                datesRunFor.Add(date);
                return Task.CompletedTask;
            };

            // Act
            await _sut.RunFromToRunTillAsync(runForParameter, testFunc);

            // Assert
            for (DateOnly curr = runForParameter.RunFor; curr <= (runForParameter.RunTill ?? runForParameter.RunFor); curr = curr.AddDays(1))
            {
                Assert.True(datesRunFor.Contains(curr));
            }
        }
    }
}