using Library.DataFileInterface.DataFileReader;
using Library.DataFileInterface.Exceptions;
using System.Collections.ObjectModel;

namespace Library.DataFileInterface.VendorFileDataModels
{
    public class Adm340BOpportunityRow : IDataRecord
    {
        #region Original properties from the vendor data file.
        public string? ContractId { get; set; }
        public string? AccountId { get; set; }
        public string? ContractName { get; set; }
        public string? IsPool { get; set; }
        public string? WholesalerNum { get; set; }
        public string? WholesalerName { get; set; }
        public string? NdcWo { get; set; }
        public string? DrugName { get; set; }
        public string? DrugStrength { get; set; }
        public string? DrugPackSize { get; set; }
        public decimal? OrdPkg { get; set; }
        public decimal? ApprPkg { get; set; }
        public DateTime? OrderDate { get; set; }
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
            Type classType = typeof(Adm340BOpportunityRow);
            int numberOfDataFileProperties = classType.GetProperties().Where(r => r.Name.IndexOf("Derived", 0) == -1).ToArray().Length;
            if (dataFileColumns.Count() != numberOfDataFileProperties)
            {
                throw new DataIntegrityException($"Number of columns [{dataFileColumns.Count()}] in the data file does not match number the expected number [{numberOfDataFileProperties}].");
            }

            var constraintViolations = new Collection<string>();
            constraintViolations.Clear();

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(0)))
                constraintViolations.Add("Contract Id cannot be blank.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(1)))
                constraintViolations.Add("Account Id cannot be blank.");

            int derivedStoreNum = 0;
            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(2)))
                constraintViolations.Add("Contract Name cannot be blank.");
            else
            {
                //Example: Derived Store Number is 25 from Contract Name "Wegmans Pharmacy #025 (Highland Hospital of Rochester) - 340B Shelter"
                int storeValueIndexStart = dataFileColumns.ElementAt(2).IndexOf("#", 0) + 1;
                int storeValueIndexEnd = dataFileColumns.ElementAt(2).IndexOf(" ", storeValueIndexStart);
                int storeValueLength = storeValueIndexEnd - storeValueIndexStart;
                if (!int.TryParse(dataFileColumns.ElementAt(2).Substring(storeValueIndexStart, storeValueLength), out derivedStoreNum))
                    constraintViolations.Add("Contract Name does not contain a Store Number value (#000).");
                else if (derivedStoreNum <= 0 || derivedStoreNum > 9999)
                    constraintViolations.Add("Invalid Store Number within Contract Name.");
            }

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(3)))
                constraintViolations.Add("Is Pool cannot be blank.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(4)))
                constraintViolations.Add("Wholesaler Num cannot be blank.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(5)))
                constraintViolations.Add("Wholesaler Name cannot be blank.");

            string derivedDrugNdc = dataFileColumns.ElementAt(6);
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

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(7)))
                constraintViolations.Add("Drug Name cannot be blank.");

            string? drugStrength_nullable = string.Empty;
            if (!string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(8)))
                drugStrength_nullable = dataFileColumns.ElementAt(8);

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(9)))
                constraintViolations.Add("Drug Pack Size cannot be blank.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(10), out decimal ordPkg))
                constraintViolations.Add("Ord Pkg is not a decimal.");

            decimal? apprPkg_nullable = new Nullable<decimal>();
            if (!string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(11)))
            {
                if (!decimal.TryParse(dataFileColumns.ElementAt(11), out decimal apprPkg))
                    constraintViolations.Add("Appr Pkg is not required, but when a value is provided it must be a decimal.");
                else
                    apprPkg_nullable = apprPkg;
            }

            DateTime? orderDate_nullable = new Nullable<DateTime>();
            if (!string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(12)))
            { 
                if (!DateTime.TryParse(dataFileColumns.ElementAt(12), out DateTime orderDate))
                    constraintViolations.Add("Order Date is not required, but when a value is provided it must be a date.");
                else
                    orderDate_nullable = orderDate;
            }

            if (constraintViolations.ToArray().Length > 0)
            {
                throw new DataIntegrityException(string.Join(" ", constraintViolations.ToArray()));
            }
            else
            {
                ContractId = dataFileColumns.ElementAt(0);
                AccountId = dataFileColumns.ElementAt(1);
                ContractName = dataFileColumns.ElementAt(2);
                IsPool = dataFileColumns.ElementAt(3);
                WholesalerNum = dataFileColumns.ElementAt(4);
                WholesalerName = dataFileColumns.ElementAt(5);
                NdcWo = dataFileColumns.ElementAt(6);
                DrugName = dataFileColumns.ElementAt(7);
                DrugStrength = drugStrength_nullable;
                DrugPackSize = dataFileColumns.ElementAt(9);
                OrdPkg = ordPkg;
                ApprPkg = apprPkg_nullable;
                OrderDate = orderDate_nullable;
                DerivedStoreNum = derivedStoreNum;
                DerivedDrugNdc = derivedDrugNdc;
                DerivedRunDate = runFor;
                DerivedProcessDate = runFor.AddDays(-1);
            }
        }
    }
}
