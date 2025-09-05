using Library.DataFileInterface.DataFileReader;
using Library.DataFileInterface.Exceptions;
using System.Collections.ObjectModel;

namespace Library.DataFileInterface.VendorFileDataModels
{
    public class Adm340BInvoicingRow : IDataRecord
    {
        #region Original properties from the vendor data file.
        public string? ClaimId { get; set; }
        public DateTime? ClaimDate { get; set; }
        public DateTime? DateOfService { get; set; }
        public int? RefillNum { get; set; }
        public int? RxNum { get; set; }
        public string? DrugName { get; set; }
        public decimal? DrugPackSize { get; set; }
        public string? DrugNdc { get; set; }
        public decimal? QtyDispensed { get; set; }
        public string? BrandGeneric { get; set; }
        public decimal? DrugCost { get; set; }
        public decimal? CoPay { get; set; }
        public decimal? TpPay { get; set; }
        public decimal? HcFacilityFee { get; set; }
        public decimal? PercentReplenished { get; set; }
        public decimal? AmtDueHcFacility { get; set; }
        public string? Ncpdp { get; set; }
        public string? PharmName { get; set; }
        public string? HcFacility { get; set; }
        public string? ContractId { get; set; }
        #endregion

        #region Derived properties based on portions of the vendor data file.
        public int? DerivedStoreNum { get; set; }
        public string? DerivedDrugNdcWo { get; set; }
        public DateOnly DerivedRunDate { get; set; }
        public DateOnly DerivedProcessDate { get; set; }
        #endregion

        /// <inheritdoc />
        public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns, DateOnly runFor)
        {
            Type classType = typeof(Adm340BInvoicingRow);
            int numberOfDataFileProperties = classType.GetProperties().Where(r => r.Name.IndexOf("Derived", 0) == -1).ToArray().Length;
            if (dataFileColumns.Count() != numberOfDataFileProperties)
            {
                throw new DataIntegrityException($"Number of columns [{dataFileColumns.Count()}] in the data file does not match number the expected number [{numberOfDataFileProperties}].");
            }

            var constraintViolations = new Collection<string>();
            constraintViolations.Clear();

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(0)))
                constraintViolations.Add("Claim Id cannot be blank.");

            if (!DateTime.TryParse(dataFileColumns.ElementAt(1), out DateTime claimDate))
                constraintViolations.Add("Claim Date is not a date.");

            if (!DateTime.TryParse(dataFileColumns.ElementAt(2), out DateTime dateOfService))
                constraintViolations.Add("Date Of Service is not a date.");

            if (!int.TryParse(dataFileColumns.ElementAt(3), out int refillNum))
                constraintViolations.Add("Refill Num Size is not an integer.");

            if (!int.TryParse(dataFileColumns.ElementAt(4), out int rxNum))
                constraintViolations.Add("Rx Num Size is not an integer.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(5)))
                constraintViolations.Add("Drug Name cannot be blank.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(6), out decimal drugPackSize))
                constraintViolations.Add("Drug Pack Size is not a decimal.");

            string derivedDrugNdcWo = dataFileColumns.ElementAt(7);
            if (string.IsNullOrWhiteSpace(derivedDrugNdcWo))
                constraintViolations.Add("Drug Ndc cannot be blank.");
            else
            {
                if (derivedDrugNdcWo.Length != 13)
                    constraintViolations.Add("Drug Ndc is not 13 characters long.");
                else
                    derivedDrugNdcWo = derivedDrugNdcWo.Replace("-", "");

                if (!long.TryParse(derivedDrugNdcWo, out long tempNdcWoNumeric))
                    constraintViolations.Add("Drug Ndc is not numeric.");
            }

            if (!decimal.TryParse(dataFileColumns.ElementAt(8), out decimal qtyDispensed))
                constraintViolations.Add("Qty Dispensed is not a decimal.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(9)))
                constraintViolations.Add("Brand Generic cannot be blank.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(10), out decimal drugCost))
                constraintViolations.Add("Drug Cost is not a decimal.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(11), out decimal coPay))
                constraintViolations.Add("Co Pay is not a decimal.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(12), out decimal tpPay))
                constraintViolations.Add("Tp Pay is not a decimal.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(13), out decimal hcFacilityFee))
                constraintViolations.Add("Hc Facility Fee is not a decimal.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(14), out decimal percentReplenished))
                constraintViolations.Add("Percent Replenished is not a decimal.");

            if (!decimal.TryParse(dataFileColumns.ElementAt(15), out decimal amtDueHcFacility))
                constraintViolations.Add("Amt Due Hc Facility is not a decimal.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(16)))
                constraintViolations.Add("Ncpdp cannot be blank.");

            int derivedStoreNum = 0;
            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(17)))
                constraintViolations.Add("Pharm Name cannot be blank.");
            else
            {
                //Example: Derived Store Number is 63 from Contract Name "Wegmans Pharmacy #063 Penfield"
                int storeValueIndexStart = dataFileColumns.ElementAt(17).IndexOf("#", 0) + 1;
                int storeValueIndexEnd = dataFileColumns.ElementAt(17).IndexOf(" ", storeValueIndexStart);
                int storeValueLength = storeValueIndexEnd - storeValueIndexStart;
                if (!int.TryParse(dataFileColumns.ElementAt(17).Substring(storeValueIndexStart, storeValueLength), out derivedStoreNum))
                    constraintViolations.Add("Pharm Name does not contain a Store Number value (#000).");
                else if (derivedStoreNum <= 0 || derivedStoreNum > 9999)
                    constraintViolations.Add("Invalid Store Number within Pharm Name.");
            }

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(18)))
                constraintViolations.Add("Hc Facility cannot be blank.");

            if (string.IsNullOrWhiteSpace(dataFileColumns.ElementAt(19)))
                constraintViolations.Add("Contract Id cannot be blank.");

            if (constraintViolations.ToArray().Length > 0)
            {
                throw new DataIntegrityException(string.Join(" ", constraintViolations.ToArray()));
            }
            else
            {
                ClaimId = dataFileColumns.ElementAt(0);
                ClaimDate = claimDate;
                DateOfService = dateOfService;
                RefillNum = refillNum;
                RxNum = rxNum;
                DrugName = dataFileColumns.ElementAt(5);
                DrugPackSize = drugPackSize;
                DrugNdc = dataFileColumns.ElementAt(7);
                QtyDispensed = qtyDispensed;
                BrandGeneric = dataFileColumns.ElementAt(9);
                DrugCost = drugCost;
                CoPay = coPay;
                TpPay = tpPay;
                HcFacilityFee = hcFacilityFee;
                PercentReplenished = percentReplenished;
                AmtDueHcFacility = amtDueHcFacility;
                Ncpdp = dataFileColumns.ElementAt(16);
                PharmName = dataFileColumns.ElementAt(17);
                HcFacility = dataFileColumns.ElementAt(18);
                ContractId = dataFileColumns.ElementAt(19);
                DerivedStoreNum = derivedStoreNum;
                DerivedDrugNdcWo = derivedDrugNdcWo;
                DerivedRunDate = runFor;
                DerivedProcessDate = runFor.AddDays(-1);
            }
        }
    }
}
