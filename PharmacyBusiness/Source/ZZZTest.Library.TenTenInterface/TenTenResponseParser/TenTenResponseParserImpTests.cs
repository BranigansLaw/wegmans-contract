using Library.TenTenInterface.Response;
using Library.TenTenInterface.TenTenResponseParser;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ZZZTest.Library.TenTenInterface.TenTenResponseParser
{
    public class TenTenResponseParserImpTests
    {
        private readonly TenTenResponseParserImp _sut;

        private readonly ILogger<TenTenResponseParserImp> _mockLogger = Substitute.For<ILogger<TenTenResponseParserImp>>();

        public TenTenResponseParserImpTests()
        {
            _sut = new TenTenResponseParserImp(_mockLogger);
        }

        [Theory]
        [InlineData("<out><rc>0</rc><sid>[SESSION_ID]</sid><pswd>[ENCRYPTED_PSWD]</pswd><msg>Last login was: [DATETIME]</msg><version>[1010data_VERSION]</version></out>", 0, "[SESSION_ID]", "[1010data_VERSION]", "[ENCRYPTED_PSWD]", "Last login was: [DATETIME]")]
        [InlineData("<out><rc>0</rc><sid>1744347709</sid><pswd>_______5eafqf7j91ycdqd25eafqf7j91ycdqd25eafqf7j91ycdqd25eafqf7j91ycdqd25eafqf7j91ycdqd25eafqf7j91ycdqd25eafqf7j91ycdqd25eafqf7j91ycdqd2</pswd><msg>Last login was: 2024-01-17 19:05:54</msg><version>prime-18.53</version></out>", 0, "1744347709", "prime-18.53", "_______5eafqf7j91ycdqd25eafqf7j91ycdqd25eafqf7j91ycdqd25eafqf7j91ycdqd25eafqf7j91ycdqd25eafqf7j91ycdqd25eafqf7j91ycdqd25eafqf7j91ycdqd2", "Last login was: 2024-01-17 19:05:54")]
        public void LoadLoginResponse_SerializedObject_FromStream(string xml, int expectedReturnCode, string expectedSessionId, string expectedVersion, string expectedPassword, string expectedMessage)
        {
            // Arrange
            HttpResponseMessage response = new HttpResponseMessage
            {
                Content = new StringContent(xml),
                StatusCode = System.Net.HttpStatusCode.OK
            };

            // Act
            LoginResponse result = _sut.ParseTenTenResponse<LoginResponse>(response);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedReturnCode, result.ResponseCode);
            Assert.Equal(expectedSessionId, result.SessionId);
            Assert.Equal(expectedVersion, result.Version);
            Assert.Equal(expectedPassword, result.Password);
            Assert.Equal(expectedMessage, result.Message);
        }

        [Fact]
        public void LoadLogoutResponse_SerializesObject_FromStream()
        {
            // Arrange
            HttpResponseMessage response = new HttpResponseMessage
            {
                Content = new StringContent("<out><rc>0</rc><msg>Logged out</msg></out>"),
                StatusCode = System.Net.HttpStatusCode.OK
            };

            // Act
            LogoutResponse result = _sut.ParseTenTenResponse<LogoutResponse>(response);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Logged out", result.Message);
        }
    }
}