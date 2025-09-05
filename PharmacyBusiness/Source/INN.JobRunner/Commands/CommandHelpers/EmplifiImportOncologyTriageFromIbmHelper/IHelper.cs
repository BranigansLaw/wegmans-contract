using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;
using Library.TenTenInterface.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INN.JobRunner.Commands.CommandHelpers.EmplifiImportOncologyTriageFromIbmHelper
{
    public interface IHelper
    {
        OncologyTriage MapOncologyTriage(
            IbmOncologyTriageRow ibmOncologyTriage,
            IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows);
    }
}
