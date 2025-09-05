using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;
using System.Text;

namespace INN.JobRunner.Commands.CommandHelpers.EmplifiImportVerificationOfBenefitsFromIbmHelper;

public interface IHelper
{
    VerificationOfBenefits MapVerificationOfBenefits(
        IbmVerificationOfBenefitsRow ibmVerificationOfBenefitsRow);

    Task<(StringBuilder, int)> ProcessFilesAndBuildReportEmailAsync(IEnumerable<string> fileNames,
        CancellationToken c);
}
