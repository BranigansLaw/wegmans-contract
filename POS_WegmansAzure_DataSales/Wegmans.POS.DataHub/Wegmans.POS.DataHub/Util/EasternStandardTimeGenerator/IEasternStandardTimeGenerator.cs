using System;

namespace Wegmans.POS.DataHub.Util.EasternStandardTimeGenerator
{
    public interface IEasternStandardTimeGenerator
    {
        /// <summary>
        /// Get the current time for the eastern standard timezone
        /// </summary>
        /// <returns></returns>
        DateTimeOffset GetCurrentEasternStandardTime();
    }
}
