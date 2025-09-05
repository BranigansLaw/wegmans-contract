using Cocona;

namespace INN.JobRunner.CommonParameters;

public class RunForFileNameParameter : ICommandParameterSet
{
    /// <summary>
    /// Example usage: --run-for-filename filename
    /// </summary>
    [Option("run-for-filename", Description = "The file name to run this job for, or an empty string if not specified.")]
    [HasDefaultValue]
    public string RunFor { get; set; } = string.Empty;
}
