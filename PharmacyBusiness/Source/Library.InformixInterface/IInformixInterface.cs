using Library.InformixInterface.DataModel;

namespace Library.InformixInterface
{
    public interface IInformixInterface
    {
        /// <summary>
        /// Get all of the CSQ Agent records for <paramref name="runFor"/>
        /// </summary>
        Task<IEnumerable<CsqAgentRow>> GetCsqAgentAsync(DateOnly runFor, CancellationToken c);

        /// <summary>
        /// Returns all the recording details for all calls for <paramref name="runFor"/>
        /// </summary>
        Task<IEnumerable<RecDetailRow>> GetRecDetailAsync(DateOnly runFor, CancellationToken c);

        /// <summary>
        /// Returns all the state details for <paramref name="runFor"/>
        /// </summary>
        Task<IEnumerable<StateDetailRow>> GetStateDetailAsync(DateOnly runFor, CancellationToken c);
    }
}
