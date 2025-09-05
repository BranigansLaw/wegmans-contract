using Cocona;

namespace INN.JobRunner.CommonParameters
{
    public class RunForDateTimeParameter : ICommandParameterSet
    {
        /// <summary>
        /// Example usage: --run-for-datetime "MM/DD/YYYY HH:MI:SS"
        /// </summary>
        [Option("run-for-datetime", Description = "The date Time to run this job as, or the current date if not specified.")]
        [HasDefaultValue]
        public DateTime RunFor { get; set; } = DateTime.Now;

        
    }
}
