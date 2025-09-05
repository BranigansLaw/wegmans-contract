namespace XXXDeveloperTools.CompareDerivedData.Exceptions
{
    public class UnableToCompareException : Exception
    {
        /// <summary>
        /// Unable to continue with file compares or data record compares.
        /// </summary>
        /// <param name="dataRowConstraintViolations"></param>
        public UnableToCompareException(string message) : base(message)
        {
        }
    }
}
