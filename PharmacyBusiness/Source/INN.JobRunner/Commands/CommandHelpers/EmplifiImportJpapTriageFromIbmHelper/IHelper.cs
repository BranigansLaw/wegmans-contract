using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;
using Library.TenTenInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.EmplifiImportJpapTriageFromIbmHelper;

public interface IHelper
{
    /// <summary>
    /// Map the JPAP Triage record from the IBM JPAP Triage record and the Complete Specialty Item List from 1010data
    /// </summary>
    /// <param name="ibmJpapTriage"></param>
    /// <param name="completeSpecialtyItemRows"></param>
    /// <returns></returns>
    JpapTriage MapJpapTriage(IbmJpapTriage ibmJpapTriage, IEnumerable<CompleteSpecialtyItemRow> completeSpecialtyItemRows);
}
