using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.EmplifiImportJpapEligibilityFromIbmHelper;

public interface IHelper
{
    JpapEligibilityRow MapJpapEligibility(
        IbmJpapEligibilityRow ibmJpapEligibilityRow);
}
