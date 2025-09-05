using Library.SnowflakeInterface.Data;

namespace INN.JobRunner.Commands.CommandHelpers.SnowflakeToTenTenDataSftpExportHelper
{
    public interface IMapper
    {
        /// <summary>
        /// Maps a list of <see cref="DurConflictRow"/> to a list of <see cref="DurConflictTenTenRow"/>
        /// </summary>
        IEnumerable<DurConflictTenTenRow> MapFromDurConflictRow(IEnumerable<DurConflictRow> durConflictRows);
    }
}
