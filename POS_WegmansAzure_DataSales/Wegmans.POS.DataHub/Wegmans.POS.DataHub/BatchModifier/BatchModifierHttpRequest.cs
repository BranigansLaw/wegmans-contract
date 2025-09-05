using System;
using System.Collections.Generic;

namespace Wegmans.POS.DataHub.BatchModifier
{
    public class BatchModifierHttpRequest
    {
        public IEnumerable<Uri> ItemPaths { get; set; }

        public IEnumerable<ModificationPlan> ModifiersToRun { get; set; }
    }
}
