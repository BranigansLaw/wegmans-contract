namespace INN.JobRunner.Extensions
{
    public static class EnumExtensions
    {
        public static T ParseOrReturnDefault<T>(string? toParse, T defaultValue) where T : struct, Enum
        {
            if (Enum.TryParse(toParse, true, out T parsed))
            {
                return parsed;
            }

            return defaultValue;
        }
    }
}
