using INN.JobRunner.ApplicationInsights;

namespace ZZZTest.INN.JobRunner.ApplicationInsights
{
    public class SensitiveDataRedactionUtilitiesTests
    {
        [Theory]
        [InlineData("Test Message", "Test Message")]
        [InlineData(
            "https://6e93f960-d22f-465e-96e3-321268d36982.mock.pstmn.io/SendSensitiveInfo?pswd=sdgsd9fgs9d0gf9sdf&sid=439853409&param=normal",
            $"https://6e93f960-d22f-465e-96e3-321268d36982.mock.pstmn.io/SendSensitiveInfo?pswd={SensitiveDataRedactionUtilities.RedactedDataMessage}&sid=439853409&param=normal")]
        [InlineData("Message with email something@email.com", $"Message with email {SensitiveDataRedactionUtilities.RedactedDataMessage}")]
        public void RedactSensitiveData_Transforms_GivenMessage(string input, string expectedOutput)
        {
            // Act
            string actual = SensitiveDataRedactionUtilities.RedactSensitiveData(input);

            // Assert
            Assert.Equal(expectedOutput, actual);
        }
    }
}