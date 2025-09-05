using System.Collections.ObjectModel;

namespace XXXDeveloperTools.CompareDerivedData.DataModels
{
    /// <summary>
    /// The demo sales row data model has three properties that together make up the primary key for the data row, and then several other properties that are raw data fields and derived data fields.
    /// </summary>
    public class Demo01_SalesRow : IDataRow
    {
        public int? RawData1_StoreNbr { get; set; }
        public string? RawData2_RegisterId { get; set; }
        public DateTime? RawData3_TransactionDateTime { get; set; }
        public string? RawData4_ItemUpc { get; set; }
        public decimal? RawData5_ItemWeightInPounds { get; set; }
        public decimal? RawData6_ItemPricePerPound { get; set; }
        public string? DerivedData1_ItemPromotionDescription { get; set; }
        public decimal? DerivedData2_ItemFinalCostAfterDiscountApplied { get; set; }

        /// <inheritdoc />
        public void SetRecordPropertiesFromFileRow(IEnumerable<string> dataFileColumns)
        {
            var constraintViolations = new Collection<string>();
            constraintViolations.Clear();

            if (!int.TryParse(dataFileColumns.ElementAt(0), out int storeNbr))
                constraintViolations.Add("StoreNbr is not an int.");
            if (string.IsNullOrEmpty(dataFileColumns.ElementAt(1)))
                constraintViolations.Add("RegisterId cannot be blank.");
            if (!DateTime.TryParse(dataFileColumns.ElementAt(2), out DateTime transactionDateTime))
                constraintViolations.Add("TransactionDateTime is not a date.");
            if (string.IsNullOrEmpty(dataFileColumns.ElementAt(3)))
                constraintViolations.Add("ItemUpc cannot be blank.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(4), out decimal itemWeightInPounds))
                constraintViolations.Add("ItemWeightInPounds is not a decimal.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(5), out decimal itemPricePerPound))
                constraintViolations.Add("ItemPricePerPound is not a decimal.");
            if (string.IsNullOrEmpty(dataFileColumns.ElementAt(6)))
                constraintViolations.Add("ItemPromotionDescription cannot be blank.");
            if (!decimal.TryParse(dataFileColumns.ElementAt(7), out decimal itemPromotionDiscountPercent))
                constraintViolations.Add("ItemPromotionDiscountPercent is not a decimal.");

            if (constraintViolations.ToArray().Length > 0)
            {
                //Note: This would only be a failure point during initial setup of a new job.
                throw new Exception(string.Join(" ", constraintViolations.ToArray()));
            }
            else
            {
                RawData1_StoreNbr = storeNbr;
                RawData2_RegisterId = dataFileColumns.ElementAt(1).Trim('"');
                RawData3_TransactionDateTime = transactionDateTime;
                RawData4_ItemUpc = dataFileColumns.ElementAt(3).Trim('"');
                RawData5_ItemWeightInPounds = itemWeightInPounds;
                RawData6_ItemPricePerPound = itemPricePerPound;
                DerivedData1_ItemPromotionDescription = dataFileColumns.ElementAt(6).Trim('"');
                DerivedData2_ItemFinalCostAfterDiscountApplied = itemPromotionDiscountPercent;
            }
        }

        /// <inheritdoc />
        public bool IsMatchingOnAllProperties(IDataRow recordToCompare)
        {
            if (recordToCompare is Demo01_SalesRow Demo01_SalesRow)
            {
                return RawData1_StoreNbr == Demo01_SalesRow.RawData1_StoreNbr &&
                       RawData2_RegisterId == Demo01_SalesRow.RawData2_RegisterId &&
                       RawData3_TransactionDateTime == Demo01_SalesRow.RawData3_TransactionDateTime &&
                       RawData4_ItemUpc == Demo01_SalesRow.RawData4_ItemUpc &&
                       RawData5_ItemWeightInPounds == Demo01_SalesRow.RawData5_ItemWeightInPounds &&
                       RawData6_ItemPricePerPound == Demo01_SalesRow.RawData6_ItemPricePerPound &&
                       DerivedData1_ItemPromotionDescription == Demo01_SalesRow.DerivedData1_ItemPromotionDescription &&
                       DerivedData2_ItemFinalCostAfterDiscountApplied == Demo01_SalesRow.DerivedData2_ItemFinalCostAfterDiscountApplied;
            }
            return false;
        }

        /// <inheritdoc />
        public bool IsMatchingOnAllNonderivedProperties(IDataRow recordToCompare)
        {
            if (recordToCompare is Demo01_SalesRow Demo01_SalesRow)
            {
                //These fields can be considered as the primary key fields for this data row, and are essentially populated from the data source rather than derived data elements.
                return RawData1_StoreNbr == Demo01_SalesRow.RawData1_StoreNbr &&
                       RawData2_RegisterId == Demo01_SalesRow.RawData2_RegisterId &&
                       RawData3_TransactionDateTime == Demo01_SalesRow.RawData3_TransactionDateTime &&
                       RawData4_ItemUpc == Demo01_SalesRow.RawData4_ItemUpc &&
                       RawData5_ItemWeightInPounds == Demo01_SalesRow.RawData5_ItemWeightInPounds &&
                       RawData6_ItemPricePerPound == Demo01_SalesRow.RawData6_ItemPricePerPound;
            }
            return false;
        }

        /// <inheritdoc />
        public List<string> GetListOfDerivedDataPropertyMismatches(IDataRow recordToCompare, bool endUsersAgreeToDecimalDifferenceThreshold = false)
        {
            List<string> returnDerivedDataProperties = new List<string>();
            decimal maxDecimalDifference = endUsersAgreeToDecimalDifferenceThreshold ? 0.001M : 0M;

            if (recordToCompare is Demo01_SalesRow Demo01_SalesRow)
            {
                if (IsMatchingOnAllNonderivedProperties(recordToCompare))
                {
                    //All of these fields below are derived data fields that were populated by complex business logic programming that may need to be under development during the QA process.
                    if (DerivedData1_ItemPromotionDescription != Demo01_SalesRow.DerivedData1_ItemPromotionDescription)
                    {
                        returnDerivedDataProperties.Add($"DerivedData1_ItemPromotionDescription=[{DerivedData1_ItemPromotionDescription}] CompareToValue=[{Demo01_SalesRow.DerivedData1_ItemPromotionDescription}]");
                    }

                    decimal decimalDifference = Math.Abs(Math.Abs(DerivedData2_ItemFinalCostAfterDiscountApplied ?? 0M) - Math.Abs(Demo01_SalesRow.DerivedData2_ItemFinalCostAfterDiscountApplied ?? 0M));
                    bool isDecimalDifferenceWithinTolerance = (decimalDifference <= maxDecimalDifference);
                    bool haveSameSign = (DerivedData2_ItemFinalCostAfterDiscountApplied ?? 0M) * (Demo01_SalesRow.DerivedData2_ItemFinalCostAfterDiscountApplied ?? 0M) > 0;
                    if (!isDecimalDifferenceWithinTolerance)
                    {
                        returnDerivedDataProperties.Add($"DerivedData2_ItemFinalCostAfterDiscountApplied=[{DerivedData2_ItemFinalCostAfterDiscountApplied}] CompareToValue=[{Demo01_SalesRow.DerivedData2_ItemFinalCostAfterDiscountApplied}] difference is [{decimalDifference}] while maxDecimalDifference=[{maxDecimalDifference}].");
                    }
                    if (!haveSameSign)
                    {
                        returnDerivedDataProperties.Add($"DerivedData2_ItemFinalCostAfterDiscountApplied=[{DerivedData2_ItemFinalCostAfterDiscountApplied}] CompareToValue=[{Demo01_SalesRow.DerivedData2_ItemFinalCostAfterDiscountApplied}]");
                    }
                }
            }

            return returnDerivedDataProperties;
        }
    }
}
