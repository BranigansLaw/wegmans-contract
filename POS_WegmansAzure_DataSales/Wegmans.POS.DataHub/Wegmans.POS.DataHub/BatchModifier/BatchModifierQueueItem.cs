using System;
using System.Collections.Generic;

namespace Wegmans.POS.DataHub.BatchModifier
{
    public class BatchModifierQueueItem
    {
        public Uri ItemPath { get; set; }

        public IEnumerable<ModificationPlan> Plan { get; set; }
    }
}
