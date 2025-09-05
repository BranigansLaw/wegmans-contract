namespace INN.JobRunner.Exceptions
{
    public class InvalidRunDateException : Exception
    {
        public InvalidRunDateException(DateOnly runFor) : base($"Invalid run date: {runFor}")
        {
            RunFor = runFor;
        }

        public DateOnly RunFor { get; }
    }
}
