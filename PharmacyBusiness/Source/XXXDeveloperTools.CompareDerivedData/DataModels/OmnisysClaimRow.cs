using System.Collections.ObjectModel;

namespace XXXDeveloperTools.CompareDerivedData.DataModels
{
    public class OmnisysClaimRow : IDataRow
    {
        public string? PharmacyNpi { get; set; }

        public string? PrescriptionNbr { get; set; }

        public string? RefillNumber { get; set; }

        public DateOnly? SoldDate { get; set; }

        public DateOnly? DateOfService { get; set; }

        public string? NdcNumber { get; set; }

        public string? CardholderId { get; set; }

        public string? AuthorizationNumber { get; set; }

        public string? ReservedForFutureUse { get; set; }

        /// <inheritdoc />
        public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns)
        {
            var constraintViolations = new Collection<string>();
            constraintViolations.Clear();

            if (!DateOnly.TryParseExact(dataFileColumns.ElementAt(3), "yyyyMMdd", out DateOnly soldDate))
                constraintViolations.Add("SoldDate is not a DateOnly type.");
            if (!DateOnly.TryParseExact(dataFileColumns.ElementAt(4), "yyyyMMdd", out DateOnly dateOfService))
                constraintViolations.Add("DateOfService is not a DateOnly type.");

            if (constraintViolations.ToArray().Length > 0)
            {
                //Note: This would only be a failure point during initial setup of a new job.
                throw new Exception(string.Join(" ", constraintViolations.ToArray()));
            }
            else
            {
                PharmacyNpi = dataFileColumns.ElementAt(0);
                PrescriptionNbr = dataFileColumns.ElementAt(1);
                RefillNumber = dataFileColumns.ElementAt(2);
                SoldDate = soldDate;
                DateOfService = dateOfService;
                NdcNumber = dataFileColumns.ElementAt(5);
                CardholderId = dataFileColumns.ElementAt(6);
                AuthorizationNumber = dataFileColumns.ElementAt(7);
                ReservedForFutureUse = dataFileColumns.ElementAt(8);
            }
        }

        /// <inheritdoc />
        public bool IsMatchingOnAllProperties(IDataRow recordToCompare)
        {
            if (recordToCompare is OmnisysClaimRow row)
            {
                //NOTE: For Omnisys Claims we do not have any derived data properties. All properties values come straight from the database.
                return PharmacyNpi == row.PharmacyNpi &&
                       PrescriptionNbr == row.PrescriptionNbr &&
                       RefillNumber == row.RefillNumber &&
                       SoldDate == row.SoldDate &&
                       DateOfService == row.DateOfService &&
                       NdcNumber == row.NdcNumber &&
                       CardholderId == row.CardholderId &&
                       AuthorizationNumber == row.AuthorizationNumber &&
                       ReservedForFutureUse == row.ReservedForFutureUse;
            }
            return false;
        }

        /// <inheritdoc />
        public bool IsMatchingOnAllNonderivedProperties(IDataRow recordToCompare)
        {
            //NOTE: For Omnisys Claims we do not have any derived data properties. All properties values come straight from the database.
            return IsMatchingOnAllProperties(recordToCompare);
        }

        /// <inheritdoc />
        public List<string> GetListOfDerivedDataPropertyMismatches(IDataRow recordToCompare, bool endUsersApproveDecimalDifferencesWithinTolerances = false)
        {
            //NOTE: For Omnisys Claims we do not have any derived data properties. All properties values come straight from the database.
            return new List<string>(); ;
        }
    }
}
