using Library.DataFileInterface.Exceptions;
using Library.DataFileInterface.DataFileReader;

namespace Library.DataFileInterface.VendorFileDataModels;

public class IbmJpapEligibilityRow : IDataRecord
{
    public string? RecordTimestamp { get; set; }
    public string? PatientProgramEnrollmentName { get; set; }
    public string? Status { get; set; }
    public string? EnrollmentStatus { get; set; }
    public string? Outcome { get; set; }
    public string? Product { get; set; }
    public string? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? StartDate { get; set; }
    public string? EndDate { get; set; }
    public string? PatientId { get; set; }

    /// <inheritdoc />
    public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns, DateOnly runFor)
    {
        Type classType = typeof(IbmJpapEligibilityRow);
        int numberOfDataFileProperties = classType.GetProperties().Where(r => r.Name.IndexOf("Derived", 0) == -1).ToArray().Length;
        if (dataFileColumns.Count() != numberOfDataFileProperties)
        {
            throw new DataIntegrityException($"Number of columns [{dataFileColumns.Count()}] in the data file does not match number the expected number [{numberOfDataFileProperties}].");
        }

        RecordTimestamp = dataFileColumns.ElementAt(0).Trim();
        PatientProgramEnrollmentName = dataFileColumns.ElementAt(1).Trim();
        Status = dataFileColumns.ElementAt(2).Trim();
        EnrollmentStatus = dataFileColumns.ElementAt(3).Trim();
        Outcome = dataFileColumns.ElementAt(4).Trim();
        Product = dataFileColumns.ElementAt(5).Trim();
        DateOfBirth = dataFileColumns.ElementAt(6).Trim();
        Gender = dataFileColumns.ElementAt(7).Trim();
        StartDate = dataFileColumns.ElementAt(8).Trim();
        EndDate = dataFileColumns.ElementAt(9).Trim();
        PatientId = dataFileColumns.ElementAt(10).Trim();
    }
}
