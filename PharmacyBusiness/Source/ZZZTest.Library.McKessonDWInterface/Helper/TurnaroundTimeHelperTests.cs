using Library.McKessonDWInterface.Helper;

namespace ZZZTest.Library.McKessonDWInterface.Helper
{
    public class TurnaroundTimeHelperTests
    {
        private readonly TurnaroundTimeHelperImp _sut;

        public TurnaroundTimeHelperTests()
        {
            _sut = new TurnaroundTimeHelperImp();
        }

        [Theory]
        [InlineData("Adjudication", "SPECIALTY", true)]
        [InlineData("Fill on Arrival", "SPECIALTY", true)]
        [InlineData("General Exception", "SPECIALTY", true)]
        [InlineData("Payment Complete Exception", "SPECIALTY", true)]
        [InlineData("Contact Manager", "SPECIALTY", false)]
        [InlineData("Adjudication", "EXCELLUS", true)]
        [InlineData("Fill on Arrival", "EXCELLUS", true)]
        [InlineData("General Exception", "EXCELLUS", true)]
        [InlineData("Payment Complete Exception", "EXCELLUS", true)]
        [InlineData("Contact Manager", "EXCELLUS", true)]
        [InlineData("Adjudication", "IHA", true)]
        [InlineData("Fill on Arrival", "IHA", true)]
        [InlineData("General Exception", "IHA", true)]
        [InlineData("Payment Complete Exception", "IHA", true)]
        [InlineData("Contact Manager", "IHA", true)]
        [InlineData("Data Entry", "SPECIALTY", false)]
        [InlineData("Data Entry", "EXCELLUS", false)]
        [InlineData("Data Entry", "IHA", false)]
        public void DeriveIsIntervention_ShouldReturnExpectedResult(string wfsdDescription, string tatTarget, bool expectedResult)
        {
            // Setup

            // Act
            bool actualResult = _sut.DeriveIsIntervention(wfsdDescription, tatTarget);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }

        [Theory]
        [InlineData("Adjudication", false)]
        [InlineData("Fill on Arrival", false)]
        [InlineData("General Exception", false)]
        [InlineData("Payment Complete Exception", false)]
        [InlineData("Contact Manager", true)]
        [InlineData("Data Entry", false)]
        public void DeriveIsException_ShouldReturnExpectedResult(string wfsdDescription, bool expectedResult)
        {
            // Setup

            // Act
            bool actualResult = _sut.DeriveIsException(wfsdDescription);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }

        [Theory]
        [InlineData(1, 1)]
        public void DeriveDaysInStep_ShouldReturnExpectedResult1(double addDays, decimal expectedResult)
        {
            // Setup
            DateTime dateIn = DateTime.Now.Date;
            DateTime dateOut = dateIn.AddDays(addDays);

            // Act
            decimal actualResult = _sut.DeriveDaysInStep(dateIn, dateOut);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }

        [Theory]
        [InlineData("6/28/2024 1:22:59 PM", "6/28/2024 1:23:00 PM", 0.000012)]
        public void DeriveDaysInStep_ShouldReturnExpectedResult2(string dateInString, string dateOutString, decimal expectedResult)
        {
            // Setup
            DateTime dateIn = DateTime.Parse(dateInString);
            DateTime dateOut = DateTime.Parse(dateOutString);

            // Act
            decimal actualResult = _sut.DeriveDaysInStep(dateIn, dateOut);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }

        [Theory]
        [InlineData(1.1, 0)]
        [InlineData(2.1, 0.558333)]
        [InlineData(3.1, 1.558333)]
        [InlineData(4.1, 2.558333)]
        [InlineData(5.1, 3)]
        [InlineData(6.1, 3)]
        [InlineData(7.1, 3)]
        public void DeriveDaysOffHours_ShouldReturnExpectedResult(double addDays, decimal expectedResult)
        {
            // Setup
            DateTime dateIn = new DateTime(2023, 12, 28, 11, 00, 00); // Thursday
            DateTime dateOut = dateIn.AddDays(addDays);

            // Act
            decimal actualResult = _sut.DeriveDaysOffHours(dateIn, dateOut);

            // Assert
            Assert.Equal(actualResult, expectedResult);
        }
    }
}