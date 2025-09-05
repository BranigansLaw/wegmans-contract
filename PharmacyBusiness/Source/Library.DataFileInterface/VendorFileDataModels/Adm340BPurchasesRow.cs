using Library.DataFileInterface.DataFileReader;
using Library.DataFileInterface.Exceptions;
using System.Collections.ObjectModel;

namespace Library.DataFileInterface.VendorFileDataModels
{
    public class Adm340BPurchasesRow : IDataRecord
    {
        #region Original properties from the vendor data file.
        public string? ContractId { get; set; }
        public string? ContractName { get; set; }
        public string? Ncpdp { get; set; }
        public DateTime? DatePurchased { get; set; }
        public string? InvNum { get; set; }
        public string? NdcWo { get; set; }
        public string? DrugPackSize { get; set; }
        public string? DrugName { get; set; }
        public decimal? QtyPurchased { get; set; }
        public decimal? CostPkg { get; set; }
        public decimal? ExtCost { get; set; }
        #endregion

        #region Derived properties based on portions of the vendor data file.
        public int? DerivedStoreNum { get; set; }
        public string? DerivedDrugNdc { get; set; }
        public DateOnly DerivedRunDate { get; set; }
        public DateOnly DerivedProcessDate { get; set; }
        #endregion

        /// <inheritdoc />
        public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns, DateOnly runFor)
        {
            Type classType = typeof(Adm340BPurchasesRow);
            int numberOfDataFileProperties = classType.GetProperties().Where(r => r.Name.IndexOf("Derived", 0) == -1).ToArray().Length;
            if (dataFileColumns.Count() != numberOfDataFileProperties)
            {
                throw new DataIntegrityException($"Number of columns [{dataFileColumns.Count()}] in the data file does not match number the expected number [{numberOfDataFileProperties}].");
            }

            var constraintViolations = new Collection<string>();
            constraintViolations.Clear();

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(0)))
                constraintViolations.Add("Contract Id cannot be blank.");

            int derivedStoreNum = 0;
            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(1)))
                constraintViolations.Add("Contract Name cannot be blank.");
            else
            {
                //Example: Derived Store Number is 25 from Contract Name "Wegmans Pharmacy #025 (Highland Hospital of Rochester) - 340B Shelter"
                int storeValueIndexStart = dataFileColumns.ElementAt(1).IndexOf("#", 0) + 1;
                int storeValueIndexEnd = dataFileColumns.ElementAt(1).IndexOf(" ", storeValueIndexStart);
                int storeValueLength = storeValueIndexEnd - storeValueIndexStart;
                if (!int.TryParse(dataFileColumns.ElementAt(1).Substring(storeValueIndexStart, storeValueLength), out derivedStoreNum))
                    constraintViolations.Add("Contract Name does not contain a Store Number value (#000).");
                else if (derivedStoreNum <= 0 || derivedStoreNum > 9999)
                    constraintViolations.Add("Invalid Store Number within Contract Name.");
            }

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(2)))
                constraintViolations.Add("Ncpdp cannot be blank.");

            if (!DateTime.TryParse(dataFileColumns.ElementAt(3), out DateTime datePurchased))
                constraintViolations.Add("Date Purchased is not a date.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(4)))
                constraintViolations.Add("Inv Num cannot be blank.");

            string derivedDrugNdc = dataFileColumns.ElementAt(5);
            if (string.IsNullOrWhiteSpace(derivedDrugNdc))
                constraintViolations.Add("NDC cannot be blank.");
            else
            {
                if (derivedDrugNdc.Length != 11)
                    constraintViolations.Add("NDC is not 11 characters long.");
                else if (!long.TryParse(derivedDrugNdc, out long tempNdcWoNumeric))
                    constraintViolations.Add("NDC is not numeric.");
                else
                {
                    string tempNdcWoFirstFive = derivedDrugNdc.Substring(0, 5);
                    string tempNdcWoMiddleFour = derivedDrugNdc.Substring(5, 4);
                    string tempNdcWoLastTwo = derivedDrugNdc.Substring(9, 2);
                    derivedDrugNdc = $"{tempNdcWoFirstFive}-{tempNdcWoMiddleFour}-{tempNdcWoLastTwo}";
                }
            }

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(6)))
                constraintViolations.Add("Drug Pack Size cannot be blank.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(7)))
                constraintViolations.Add("Drug Name cannot be blank.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(8), out decimal qtyPurchased))
                constraintViolations.Add("Qty Purchased is not a decimal.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(9), out decimal costPkg))
                constraintViolations.Add("Drug Pack Size is not a decimal.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(10), out decimal extCost))
                constraintViolations.Add("Ext Cost is not a decimal.");

            if (constraintViolations.ToArray().Length > 0)
            {
                throw new DataIntegrityException(string.Join(" ", constraintViolations.ToArray()));
            }
            else
            {
                ContractId = dataFileColumns.ElementAt(0);
                ContractName = dataFileColumns.ElementAt(1);
                Ncpdp = dataFileColumns.ElementAt(2);
                DatePurchased = datePurchased;
                InvNum = dataFileColumns.ElementAt(4);
                NdcWo = dataFileColumns.ElementAt(5);
                DrugPackSize = dataFileColumns.ElementAt(6);
                DrugName = dataFileColumns.ElementAt(7);
                QtyPurchased = qtyPurchased;
                CostPkg = costPkg;
                ExtCost = extCost;
                DerivedStoreNum = derivedStoreNum;
                DerivedDrugNdc = derivedDrugNdc;
                DerivedRunDate = runFor;
                DerivedProcessDate = runFor.AddDays(-1);
            }
        }
    }
}
