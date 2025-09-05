using System;

namespace Wegmans.POS.DataHub.Util.EasternStandardTimeGenerator
{
    public class EasternStandardTimeGeneratorImp : IEasternStandardTimeGenerator
    {
        /// <inheritdoc />
        public DateTimeOffset GetCurrentEasternStandardTime()
        {
            return DateTimeOffset.Now.ToEasternStandardTime();
        }
    }
}
