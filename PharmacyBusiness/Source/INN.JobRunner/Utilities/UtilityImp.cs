namespace INN.JobRunner.Utility
{
    public class UtilityImp : IUtility
    {
        /// <inheritdoc />
        public string GetCurrentExecutableDirectory() => ControlM.GetCurrentExecutableDirectory();

        /// <inheritdoc />
        public Task Delay(int milliseconds) => Task.Delay(milliseconds);

        /// <inheritdoc />
        public Task Delay(TimeSpan delay) => Task.Delay(delay);
    }
}
