using Library.DataFileInterface;
using Library.DataFileInterface.VendorFileDataModels;
using Library.EmplifiInterface.DataModel;
using Library.EmplifiInterface.Helper;
using Library.TenTenInterface.DataModel;

namespace INN.JobRunner.Commands.CommandHelpers.EmplifiImportJpapEligibilityFromIbmHelper;

public class HelperImp : IHelper
{
    private readonly IJpapEligibilityHelper _jpapEligibilityHelper;
    private readonly IDataFileInterface _dataFileInterface;

    public HelperImp(
        IJpapEligibilityHelper jpapEligibilityHelper, 
        IDataFileInterface dataFileInterface)
    {
        _jpapEligibilityHelper = jpapEligibilityHelper ?? throw new ArgumentNullException(nameof(jpapEligibilityHelper));
        _dataFileInterface = dataFileInterface ?? throw new ArgumentNullException(nameof(dataFileInterface));
    }

    public JpapEligibilityRow MapJpapEligibility(
        IbmJpapEligibilityRow ibmJpapEligibilityRow)
    {
        var derivedDateOfBirth = _jpapEligibilityHelper.DeriveDate(ibmJpapEligibilityRow.DateOfBirth);

        return new JpapEligibilityRow
        {
            RecordTimestamp = ibmJpapEligibilityRow.RecordTimestamp,
            PatientProgramEnrollmentName = ibmJpapEligibilityRow.PatientProgramEnrollmentName,
            Status = ibmJpapEligibilityRow.Status,
            EnrollmentStatus = ibmJpapEligibilityRow.EnrollmentStatus,
            Outcome = ibmJpapEligibilityRow.Outcome,
            Product = ibmJpapEligibilityRow.Product,
            DateOfBirth = derivedDateOfBirth,
            Gender = ibmJpapEligibilityRow.Gender,
            StartDate = ibmJpapEligibilityRow.StartDate,
            EndDate = ibmJpapEligibilityRow.EndDate,
            PatientId = ibmJpapEligibilityRow.PatientId,
            
            DerivedCaseText = _jpapEligibilityHelper.DeriveCaseText(
                ibmJpapEligibilityRow,
                derivedDateOfBirth
                )
        };
    }

}
