using Microsoft.Extensions.Configuration;

namespace INN.JobRunner.Extensions
{
    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Attempt to retrieve the value of the key with name <paramref name="key"/>, or throw a <see cref="Exception"/> if not found
        /// </summary>
        /// <param name="key">The key name to search for</param>
        /// <returns>The string value of the key if found</returns>
        /// <exception cref="Exception">If <paramref name="key"/> is not found in <paramref name="config"/></exception>
        public static string GetValueOrThrowException(this IConfiguration config, string key) =>
            config.GetValue<string>(key) ?? throw new Exception($"Missing configuration '{key}'");
    }
}
