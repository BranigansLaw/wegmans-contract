using Library.DataFileInterface.DataFileReader;
using Library.DataFileInterface.Exceptions;
using System.Collections.ObjectModel;

namespace Library.DataFileInterface.VendorFileDataModels
{
    public class MedicareAutoshipRow : IDataRecord
    {
        #region Original properties from the vendor data file.
        public string? PhoneNumber { get; set; }
        #endregion

        #region Derived properties based on portions of the vendor data file.
        public DateOnly DerivedFileNameDate { get; set; }
        public DateOnly DerivedProcessDate { get; set; }
        #endregion

        /// <inheritdoc />
        public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns, DateOnly fileNameDate)
        {
            Type classType = typeof(MedicareAutoshipRow);
            int numberOfDataFileProperties = classType.GetProperties().Where(r => r.Name.IndexOf("Derived", 0) == -1).ToArray().Length;
            if (dataFileColumns.Count() != numberOfDataFileProperties)
            {
                throw new DataIntegrityException($"Number of columns [{dataFileColumns.Count()}] in the data file does not match number the expected number [{numberOfDataFileProperties}].");
            }

            var constraintViolations = new Collection<string>();
            constraintViolations.Clear();

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(0)))
                constraintViolations.Add("Phone Number cannot be blank.");

            if (constraintViolations.ToArray().Length > 0)
            {
                throw new DataIntegrityException(string.Join(" ", constraintViolations.ToArray()));
            }
            else
            {
                PhoneNumber = dataFileColumns.ElementAt(0);
                DerivedFileNameDate = fileNameDate;
                DerivedProcessDate = DateOnly.FromDateTime(DateTime.Now);
            }
        }
    }
}
