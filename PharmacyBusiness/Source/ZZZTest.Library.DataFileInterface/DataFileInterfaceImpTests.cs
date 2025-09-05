using Library.DataFileInterface;
using Library.DataFileInterface.DataFileReader;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace ZZZTest.Library.DataFileInterface
{
    public class DataFileInterfaceImpTests
    {
        private readonly DataFileInterfaceImp _sut;
        private readonly IDataFileReader _dataFileReaderMock = Substitute.For<IDataFileReader>();
        private readonly IOptions<DataFileConfig> _configMock = Substitute.For<IOptions<DataFileConfig>>();
        private readonly ILogger<DataFileInterfaceImp> _loggerMock = Substitute.For<ILogger<DataFileInterfaceImp>>();

        public DataFileInterfaceImpTests()
        {
            _configMock.Value.Returns(new DataFileConfig());
            _sut = new DataFileInterfaceImp(_dataFileReaderMock, _configMock, _loggerMock);
        }

        [Theory]
        [InlineData("Wegmans_Medicare_Autoship_DNC_PhoneNum_20240903.csv", "Wegmans_Medicare_Autoship_DNC_PhoneNum_*.csv", "yyyyMMdd", "09/03/2024 00:00:00")]
        [InlineData("Wegmans_tocall_20241004_042803.txt", "Wegmans_tocall_*.txt", "yyyyMMdd_HHmmss", "10/04/2024 04:28:03")]
        public void ExtractDateFromFileName_ShouldReturnValidDateTimes(string actualFileName, string fileNamePattern, string dateParsePattern, string expectedReturnedDateTimeString)
        {
            // Setup
            DateTime expectedReturnedDateTime = DateTime.Parse(expectedReturnedDateTimeString);

            // Act
            DateTime? actualFileNameDate = _sut.ExtractDateFromFileName(actualFileName, fileNamePattern, dateParsePattern);

            // Assert
            Assert.Equal(expectedReturnedDateTime, actualFileNameDate);
        }

        [Theory]
        [InlineData("Wegmans_Medicare_Autoship_DNC_PhoneNum_.csv", "Wegmans_Medicare_Autoship_DNC_PhoneNum_*.csv", "yyyyMMdd")]
        [InlineData("Wegmans_tocall_2024-10-04_04:28:03.txt", "Wegmans_tocall_*.txt", "yyyyMMdd_HHmmss")]
        public void ExtractDateFromFileName_ShouldReturnNullableDateTimes(string actualFileName, string fileNamePattern, string dateParsePattern)
        {
            // Setup
            DateTime? expectedReturnedDateTime = default;

            // Act
            DateTime? actualFileNameDate = _sut.ExtractDateFromFileName(actualFileName, fileNamePattern, dateParsePattern);

            // Assert
            Assert.Equal(expectedReturnedDateTime, actualFileNameDate);
        }
    }
}
