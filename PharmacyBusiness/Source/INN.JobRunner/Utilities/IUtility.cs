namespace INN.JobRunner.Utility
{
    public interface IUtility
    {
        /// <summary>
        /// Returns the current executable directory (with Control-M, this isn't always the same with other GetExecutableDirectory methods)
        /// </summary>
        /// <returns></returns>
        string GetCurrentExecutableDirectory();

        /// <summary>
        /// Wait for <paramref name="milliseconds"/> milliseconds. This method encapsulates the standard Task.Delay method for easier testing
        /// </summary>
        Task Delay(int milliseconds);

        /// <summary>
        /// Wait for <paramref name="delay"/>. This method encapsulates the standard Task.Delay method for easier testing
        /// </summary>
        Task Delay(TimeSpan delay);
    }
}
