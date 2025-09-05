using Library.DataFileInterface.DataFileReader;
using Library.DataFileInterface.Exceptions;
using System.Collections.ObjectModel;

namespace Library.DataFileInterface.VendorFileDataModels
{
    public class IvrOutboundCallsRow : IDataRecord
    {
        public string? PhoneNbr { get; set; }
        public DateOnly? CallMadeDate { get; set; }
        public string? CallMadeTime { get; set; }
        public string? CallStatus { get; set; }
        public string? AnsweredBy { get; set; }
        public int? RxNum { get; set; }
        public int? NbrAttempts { get; set; }
        public int? StoreNum { get; set; }
        public string? CallType { get; set; }

        /// <inheritdoc />
        public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns, DateOnly runFor)
        {
            Type classType = typeof(IvrOutboundCallsRow); 
            int numberOfDataFileProperties = classType.GetProperties().Length;
            if (dataFileColumns.Count() != numberOfDataFileProperties)
            {
                throw new DataIntegrityException($"Number of columns [{dataFileColumns.Count()}] in the data file does not match number the expected number [{numberOfDataFileProperties}].");
            }

            var constraintViolations = new Collection<string>();
            constraintViolations.Clear();

            if (!DateOnly.TryParse(dataFileColumns.ElementAt(1), out DateOnly callMadeDate))
                constraintViolations.Add("CallMadeDate is not a DateOnly.");

            if (!int.TryParse(dataFileColumns.ElementAt(5), out int rxNum))
                constraintViolations.Add("RxNbr is not an integer.");

            if (!int.TryParse(dataFileColumns.ElementAt(6), out int nbrAttempts))
                constraintViolations.Add("NbrAttempts is not an integer.");

            if (!int.TryParse(dataFileColumns.ElementAt(7), out int storeNum))
                constraintViolations.Add("StoreNum is not an integer.");

            if (constraintViolations.ToArray().Length > 0)
            {
                throw new DataIntegrityException(string.Join(" ", constraintViolations.ToArray()));
            }
            else
            {
                PhoneNbr = dataFileColumns.ElementAt(0);
                CallMadeDate = callMadeDate;
                CallMadeTime = dataFileColumns.ElementAt(2);
                CallStatus = dataFileColumns.ElementAt(3);
                AnsweredBy = dataFileColumns.ElementAt(4);
                RxNum = rxNum;
                NbrAttempts = nbrAttempts;
                StoreNum = storeNum;
                CallType = dataFileColumns.ElementAt(8);
            }
        }
    }
}
