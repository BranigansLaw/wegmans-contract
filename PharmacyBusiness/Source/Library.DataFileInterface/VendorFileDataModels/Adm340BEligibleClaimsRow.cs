using Library.DataFileInterface.DataFileReader;
using Library.DataFileInterface.Exceptions;
using System.Collections.ObjectModel;

namespace Library.DataFileInterface.VendorFileDataModels
{
    public class Adm340BEligibleClaimsRow : IDataRecord
    {
        #region Original properties from the vendor data file.
        public string? ClaimId { get; set; }
        public string? Type { get; set; }
        public DateTime? ClaimDate { get; set; }
        public DateTime? DateOfService { get; set; }
        public int? RefillNum { get; set; }
        public int? RxNum { get; set; }
        public string? DrugName { get; set; }
        public decimal? DrugPackSize { get; set; }
        public int? PackageQty { get; set; }
        public decimal? QtyPerUoi { get; set; }
        public string? DrugNdcWo { get; set; }
        public decimal? QtyDispensed { get; set; }
        public decimal? DaysSupply { get; set; }
        public string? BrandGeneric { get; set; }
        public string? CashThirdParty { get; set; }
        public string? ClaimType { get; set; }
        public decimal? SalesTax { get; set; }
        public decimal? CoPay { get; set; }
        public decimal? TpPay { get; set; }
        public decimal? HcFacilityFee { get; set; }
        public decimal? AmtDueHcFacility { get; set; }
        public string? PharmName { get; set; }
        public string? HcFacility { get; set; }
        public string? UniqueClaimId { get; set; }
        public string? ContractId { get; set; }
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
            Type classType = typeof(Adm340BEligibleClaimsRow);
            int numberOfDataFileProperties = classType.GetProperties().Where(r => r.Name.IndexOf("Derived", 0) == -1).ToArray().Length;
            if (dataFileColumns.Count() != numberOfDataFileProperties)
            {
                throw new DataIntegrityException($"Number of columns [{dataFileColumns.Count()}] in the data file does not match number the expected number [{numberOfDataFileProperties}].");
            }

            var constraintViolations = new Collection<string>();
            constraintViolations.Clear();

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(0)))
                constraintViolations.Add("Claim Id cannot be blank.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(1)))
                constraintViolations.Add("Type cannot be blank.");

            if (!DateTime.TryParse(dataFileColumns.ElementAt(2), out DateTime claimDate))
                constraintViolations.Add("Claim Date is not a date.");

            if (!DateTime.TryParse(dataFileColumns.ElementAt(3), out DateTime dateOfService))
                constraintViolations.Add("Date Of Serviceis not a date.");

            if (!int.TryParse(dataFileColumns.ElementAt(4), out int refillNum))
                constraintViolations.Add("Refill Num Size is not an integer.");

            if (!int.TryParse(dataFileColumns.ElementAt(5), out int rxNum))
                constraintViolations.Add("Rx Num is not an integer.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(6)))
                constraintViolations.Add("Drug Name cannot be blank.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(7), out decimal drugPackSize))
                constraintViolations.Add("Drug Pack Size is not a decimal.");

            if (!int.TryParse(dataFileColumns.ElementAt(8), out int packageQty))
                constraintViolations.Add("Package Qty is not an integer.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(9), out decimal qtyPerUoi))
                constraintViolations.Add("Qty Per Uoi is not a decimal.");

            string derivedDrugNdc = dataFileColumns.ElementAt(10);
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

            if (!decimal.TryParse(dataFileColumns.ElementAt(11), out decimal qtyDispensed))
                constraintViolations.Add("Qty Dispensed is not a decimal.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(12), out decimal daysSupply))
                constraintViolations.Add("Days Supply is not a decimal.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(13)))
                constraintViolations.Add("Brand Generic cannot be blank.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(14)))
                constraintViolations.Add("Cash Third Party cannot be blank.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(15)))
                constraintViolations.Add("Claim Type cannot be blank.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(16), out decimal salesTax))
                constraintViolations.Add("Sales Tax is not a decimal.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(17), out decimal coPay))
                constraintViolations.Add("Co Pay is not a decimal.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(18), out decimal tpPay))
                constraintViolations.Add("Tp Pay is not a decimal.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(19), out decimal hcFacilityFee))
                constraintViolations.Add("Hc Facility Fee is not a decimal.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(20), out decimal amtDueHcFacility))
                constraintViolations.Add("Amt Due Hc Facility is not a decimal.");

            int derivedStoreNum = 0;
            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(21)))
                constraintViolations.Add("Pharm Name cannot be blank.");
            else
            {
                //Example: Derived Store Number is 63 from Contract Name "Wegmans Pharmacy #063 Penfield"
                int storeValueIndexStart = dataFileColumns.ElementAt(21).IndexOf("#", 0) + 1;
                int storeValueIndexEnd = dataFileColumns.ElementAt(21).IndexOf(" ", storeValueIndexStart);
                int storeValueLength = storeValueIndexEnd - storeValueIndexStart;
                if (!int.TryParse(dataFileColumns.ElementAt(21).Substring(storeValueIndexStart, storeValueLength), out derivedStoreNum))
                    constraintViolations.Add("Pharm Name does not contain a Store Number value (#000).");
                else if (derivedStoreNum <= 0 || derivedStoreNum > 9999)
                    constraintViolations.Add("Invalid Store Number within Pharm Name.");
            }

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(22)))
                constraintViolations.Add("Hc Facility cannot be blank.");

            //Unique Claim ID is blank in all data rows, but no requirements given yet, so commented out.
            //if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(23)))
            //    constraintViolations.Add("Unique Claim Id cannot be blank.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(24)))
                constraintViolations.Add("Contract Id cannot be blank.");

            if (constraintViolations.ToArray().Length > 0)
            {
                throw new DataIntegrityException(string.Join(" ", constraintViolations.ToArray()));
            }
            else
            {
                ClaimId = dataFileColumns.ElementAt(0);
                Type = dataFileColumns.ElementAt(1);
                ClaimDate = claimDate;
                DateOfService = dateOfService;
                RefillNum = refillNum;
                RxNum = rxNum;
                DrugName = dataFileColumns.ElementAt(6);
                DrugPackSize = drugPackSize;
                PackageQty = packageQty;
                QtyPerUoi = qtyPerUoi;
                DrugNdcWo = dataFileColumns.ElementAt(10);
                QtyDispensed = qtyDispensed;
                DaysSupply = daysSupply;
                BrandGeneric = dataFileColumns.ElementAt(13);
                CashThirdParty = dataFileColumns.ElementAt(14);
                ClaimType = dataFileColumns.ElementAt(15);
                SalesTax = salesTax;
                CoPay = coPay;
                TpPay = tpPay;
                HcFacilityFee = hcFacilityFee;
                AmtDueHcFacility = amtDueHcFacility;
                PharmName = dataFileColumns.ElementAt(21);
                HcFacility = dataFileColumns.ElementAt(22);
                UniqueClaimId = dataFileColumns.ElementAt(23);
                ContractId = dataFileColumns.ElementAt(24);
                DerivedStoreNum = derivedStoreNum;
                DerivedDrugNdc = derivedDrugNdc;
                DerivedRunDate = runFor;
                DerivedProcessDate = runFor.AddDays(-1);
            }
        }
    }
}
