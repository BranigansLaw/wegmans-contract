using System.Text.RegularExpressions;

namespace INN.JobRunner.ApplicationInsights
{
    public static class SensitiveDataRedactionUtilities
    {
        public const string RedactedDataMessage = "[REDACTED]";

        public static string RedactSensitiveData(string message)
        {
            message = Regex.Replace(message, @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", RedactedDataMessage);
            message = Regex.Replace(message, @"pswd=[^&]*", $"pswd={RedactedDataMessage}");

            return message;
        }
    }
}
