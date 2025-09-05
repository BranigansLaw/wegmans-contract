namespace INN.JobRunner.Commands.CommandHelpers.Generic
{
    /// <summary>
    /// An interface for interacting with <see cref="ICoconaContextWrapper"/> since it is difficult to setup in a unit test.
    /// </summary>
    public interface ICoconaContextWrapper
    {
        /// <summary>
        /// Get the cancellation token from the <see cref="ICoconaContextWrapper"/>
        /// </summary>
        CancellationToken CancellationToken { get; }
    }
}
