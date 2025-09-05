using Cocona;

namespace INN.JobRunner.CommonParameters
{
    public class RunForParameter : ICommandParameterSet
    {
        /// <summary>
        /// Example usage: --run-for MM/DD/YYYY
        /// </summary>
        [Option("run-for", Description = "The date to run this job as, or the current date if not specified.")]
        [HasDefaultValue]
        public DateOnly RunFor { get; set; } = DateOnly.FromDateTime(DateTime.Now);

        /// <summary>
        /// Example usage: --run-for MM/DD/YYYY --run-till MM/DD/YYYY
        /// </summary>
        [Option("run-till", Description = "When specified, runs for all dates between run-for until this date, inclusive.")]
        [HasDefaultValue]
        public DateOnly? RunTill { get; set; } = null;
    }
}
