namespace Library.EmplifiInterface.Exceptions
{
    public class InvalidRunDateException : Exception
    { 
        public InvalidRunDateException(string? message, DateTime runFor, DayOfWeek dayOfWeek) : base(message)
        {
            RunFor = runFor;
            RunForDayOfWeek = dayOfWeek;
        }

        public DateTime RunFor { get; set; }

        public DayOfWeek RunForDayOfWeek { get; set; }
    }
}
