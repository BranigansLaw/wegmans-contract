namespace INN.JobRunner.ErrorFormatter
{
    public interface IExceptionMapper
    {
        /// <summary>
        /// Maps the given <paramref name="ex"/> for job name <paramref name="jobName"/> (the name of the job as it is defined 
        /// in the <see cref="CommandAttribute"/>) to a human-readable console error message with steps for support handling.
        /// </summary>
        Task ExceptionToConsoleErrorAsync(string jobName, Exception ex);
    }
}
