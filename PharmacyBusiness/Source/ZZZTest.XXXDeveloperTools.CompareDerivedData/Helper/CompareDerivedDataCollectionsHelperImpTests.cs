using Microsoft.VisualBasic;
using System.Drawing;
using Xunit;
using XXXDeveloperTools.CompareDerivedData.DataModels;
using XXXDeveloperTools.CompareDerivedData.Helper;

namespace ZZZTest.XXXDeveloperTools.CompareDerivedData.Helper
{
    public class CompareDerivedDataCollectionsHelperImpTests
    {
        public CompareDerivedDataCollectionsHelperImpTests()
        {
        }

        [Theory]
        [InlineData("DataValuesMatchWithSameSortOrder", true, "", "")]
        [InlineData("DataValuesMatchWithDifferentSortOrder", false, "2", "")]
        [InlineData("DataValuesMatchWithCloseDecimalRounding", false, "2", "2")]
        [InlineData("IrreconcilableDifferencesWereFound", false, "2", "2")]
        public void DataValuesMatchWithSameSortOrder_ShouldReturnExpectedResult(
            string scenario,
            bool expectedResult,
            string expectedNotMatchedNewRecordIndexesString,
            string expectedNotMatchedOldRecordIndexesString)
        {
            // Setup
            List<string> tempNewRecordIndexes = string.IsNullOrEmpty(expectedNotMatchedNewRecordIndexesString) ? [] : expectedNotMatchedNewRecordIndexesString.Split(',').ToList();
            List<string> tempOldRecordIndexes = string.IsNullOrEmpty(expectedNotMatchedOldRecordIndexesString) ? [] : expectedNotMatchedOldRecordIndexesString.Split(',').ToList();
            List<int> expectedNotMatchedNewRecordIndexes = tempNewRecordIndexes.Count() > 0 ? tempNewRecordIndexes.Select(int.Parse).ToList() : [];
            List<int> expectedNotMatchedOldRecordIndexes = tempOldRecordIndexes.Count() > 0 ? tempOldRecordIndexes.Select(int.Parse).ToList() : [];

            CompareDerivedDataCollectionsHelperImp<Demo01_SalesRow> _sut = new CompareDerivedDataCollectionsHelperImp<Demo01_SalesRow>(
                new CompareDerivedDataSpecifications
                {
                    NewFilePath = Environment.ExpandEnvironmentVariables("MockNewFilePath"),
                    OldFilePath = Environment.ExpandEnvironmentVariables("MockOldFilePath"),
                    FileName = "MockFileName",
                    RowDelimiter = Environment.NewLine,
                    ColumnDelimiter = ",",
                    HasHeaderRow = true
                });
            var (newDataRecords, oldDataRecords) = GetNewAndOldDemoSalesData(scenario);
            _sut.NewDataRecords = newDataRecords;
            _sut.OldDataRecords = oldDataRecords;

            // Act
            var actualResult = _sut.DataValuesMatchWithSameSortOrder();

            // Assert
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedNotMatchedNewRecordIndexes, _sut.NotMatchedNewRecordIndexes);
            Assert.Equal(expectedNotMatchedOldRecordIndexes, _sut.NotMatchedOldRecordIndexes);
        }

        [Theory]
        [InlineData("DataValuesMatchWithSameSortOrder", false, true, "", "", "")]
        [InlineData("DataValuesMatchWithDifferentSortOrder", false, true, "", "", "")]
        [InlineData("DataValuesMatchWithCloseDecimalRounding", false, false, "", "", "2")]
        [InlineData("DataValuesMatchWithCloseDecimalRounding", true, true, "", "", "")]
        [InlineData("IrreconcilableDifferencesWereFound", false, false, "", "", "2")]
        public void DataValuesMatchWithDifferentSortOrder_ShouldReturnExpectedResult(
            string scenario,
            bool endUsersAgreeToDecimalDifferenceThreshold,
            bool expectedResult,
            string expectedNotMatchedNewRecordIndexesString,
            string expectedNotMatchedOldRecordIndexesString,
            string expectedMatchedOnOnlyNonderivedIndexesString)
        {
            // Setup
            List<string> tempNewRecordIndexes = string.IsNullOrEmpty(expectedNotMatchedNewRecordIndexesString) ? [] : expectedNotMatchedNewRecordIndexesString.Split(',').ToList();
            List<string> tempOldRecordIndexes = string.IsNullOrEmpty(expectedNotMatchedOldRecordIndexesString) ? [] : expectedNotMatchedOldRecordIndexesString.Split(',').ToList();
            List<string> tempMatchedOnOnlyNonderivedIndexes = string.IsNullOrEmpty(expectedMatchedOnOnlyNonderivedIndexesString) ? [] : expectedMatchedOnOnlyNonderivedIndexesString.Split(',').ToList();

            List<int> expectedNotMatchedNewRecordIndexes = tempNewRecordIndexes.Count() > 0 ? tempNewRecordIndexes.Select(int.Parse).ToList() : [];
            List<int> expectedNotMatchedOldRecordIndexes = tempOldRecordIndexes.Count() > 0 ? tempOldRecordIndexes.Select(int.Parse).ToList() : [];
            List<int> expectedMatchedOnOnlyNonderivedIndexes = tempMatchedOnOnlyNonderivedIndexes.Count() > 0 ? tempMatchedOnOnlyNonderivedIndexes.Select(int.Parse).ToList() : [];

            CompareDerivedDataCollectionsHelperImp<Demo01_SalesRow> _sut = new CompareDerivedDataCollectionsHelperImp<Demo01_SalesRow>(
                new CompareDerivedDataSpecifications
                {
                    NewFilePath = Environment.ExpandEnvironmentVariables("MockNewFilePath"),
                    OldFilePath = Environment.ExpandEnvironmentVariables("MockOldFilePath"),
                    FileName = "MockFileName",
                    RowDelimiter = Environment.NewLine,
                    ColumnDelimiter = ",",
                    HasHeaderRow = true
                });
            var (newDataRecords, oldDataRecords) = GetNewAndOldDemoSalesData(scenario);
            _sut.NewDataRecords = newDataRecords;
            _sut.OldDataRecords = oldDataRecords;

            // Act
            var actualResult = _sut.DataValuesMatchWithDifferentSortOrder(endUsersAgreeToDecimalDifferenceThreshold);

            // Assert
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedNotMatchedNewRecordIndexes, _sut.NotMatchedNewRecordIndexes);
            Assert.Equal(expectedNotMatchedOldRecordIndexes, _sut.NotMatchedOldRecordIndexes);
            foreach(var expectedMatchedOnOnlyNonderivedIndex in expectedMatchedOnOnlyNonderivedIndexes)
            {
                Assert.True(_sut.MatchedOnOnlyNonderivedPropertiesButNotMatchedOnDerivedProperties.ContainsKey(expectedMatchedOnOnlyNonderivedIndex));
            }
        }

        private (Demo01_SalesRow[], Demo01_SalesRow[]) GetNewAndOldDemoSalesData(string scenario)
        {
            List<Demo01_SalesRow> newDataRecords = new List<Demo01_SalesRow>();
            Demo01_SalesRow[] oldDataRecords = new Demo01_SalesRow[]
            {
                new Demo01_SalesRow {
                    RawData1_StoreNbr = 1,
                    RawData2_RegisterId = "B",
                    RawData3_TransactionDateTime = DateTime.Parse("07/01/2024 09:00:00"),
                    RawData4_ItemUpc = "111",
                    RawData5_ItemWeightInPounds = 1.23M,
                    RawData6_ItemPricePerPound = 3.99M,
                    DerivedData1_ItemPromotionDescription = "Buy one pound save ten percent",
                    DerivedData2_ItemFinalCostAfterDiscountApplied = 4.417M
                },
                new Demo01_SalesRow {
                    RawData1_StoreNbr = 1,
                    RawData2_RegisterId = "C",
                    RawData3_TransactionDateTime = DateTime.Parse("07/01/2024 10:00:00"),
                    RawData4_ItemUpc = "111",
                    RawData5_ItemWeightInPounds = 2.34M,
                    RawData6_ItemPricePerPound = 3.99M,
                    DerivedData1_ItemPromotionDescription = "Buy two pounds save twenty percent",
                    DerivedData2_ItemFinalCostAfterDiscountApplied = 7.469M
                },
                new Demo01_SalesRow {
                    RawData1_StoreNbr = 3,
                    RawData2_RegisterId = "E",
                    RawData3_TransactionDateTime = DateTime.Parse("07/01/2024 11:00:00"),
                    RawData4_ItemUpc = "333",
                    RawData5_ItemWeightInPounds = 0.91M,
                    RawData6_ItemPricePerPound = 5.55M,
                    DerivedData1_ItemPromotionDescription = "No discounts for this UPC",
                    DerivedData2_ItemFinalCostAfterDiscountApplied = 5.051M
                },
                new Demo01_SalesRow {
                    RawData1_StoreNbr = 1,
                    RawData2_RegisterId = "A",
                    RawData3_TransactionDateTime = DateTime.Parse("07/01/2024 08:00:00"),
                    RawData4_ItemUpc = "111",
                    RawData5_ItemWeightInPounds = 0.56M,
                    RawData6_ItemPricePerPound = 3.99M,
                    DerivedData1_ItemPromotionDescription = "No discount under one pound",
                    DerivedData2_ItemFinalCostAfterDiscountApplied = 2.234M
                }
            };

            switch (scenario)
            {
                case "DataValuesMatchWithSameSortOrder":
                    foreach(var oldDataRecord in oldDataRecords)
                    {
                        newDataRecords.Add(new Demo01_SalesRow
                        {
                            RawData1_StoreNbr = oldDataRecord.RawData1_StoreNbr,
                            RawData2_RegisterId = oldDataRecord.RawData2_RegisterId,
                            RawData3_TransactionDateTime = oldDataRecord.RawData3_TransactionDateTime,
                            RawData4_ItemUpc = oldDataRecord.RawData4_ItemUpc,
                            RawData5_ItemWeightInPounds = oldDataRecord.RawData5_ItemWeightInPounds,
                            RawData6_ItemPricePerPound = oldDataRecord.RawData6_ItemPricePerPound,
                            DerivedData1_ItemPromotionDescription = oldDataRecord.DerivedData1_ItemPromotionDescription,
                            DerivedData2_ItemFinalCostAfterDiscountApplied = oldDataRecord.DerivedData2_ItemFinalCostAfterDiscountApplied
                        });
                    }
                    break;
                case "DataValuesMatchWithDifferentSortOrder":
                    newDataRecords.Add(new Demo01_SalesRow
                    {
                        RawData1_StoreNbr = oldDataRecords[0].RawData1_StoreNbr,
                        RawData2_RegisterId = oldDataRecords[0].RawData2_RegisterId,
                        RawData3_TransactionDateTime = oldDataRecords[0].RawData3_TransactionDateTime,
                        RawData4_ItemUpc = oldDataRecords[0].RawData4_ItemUpc,
                        RawData5_ItemWeightInPounds = oldDataRecords[0].RawData5_ItemWeightInPounds,
                        RawData6_ItemPricePerPound = oldDataRecords[0].RawData6_ItemPricePerPound,
                        DerivedData1_ItemPromotionDescription = oldDataRecords[0].DerivedData1_ItemPromotionDescription,
                        DerivedData2_ItemFinalCostAfterDiscountApplied = oldDataRecords[0].DerivedData2_ItemFinalCostAfterDiscountApplied
                    });
                    newDataRecords.Add(new Demo01_SalesRow
                    {
                        RawData1_StoreNbr = oldDataRecords[2].RawData1_StoreNbr,
                        RawData2_RegisterId = oldDataRecords[2].RawData2_RegisterId,
                        RawData3_TransactionDateTime = oldDataRecords[2].RawData3_TransactionDateTime,
                        RawData4_ItemUpc = oldDataRecords[2].RawData4_ItemUpc,
                        RawData5_ItemWeightInPounds = oldDataRecords[2].RawData5_ItemWeightInPounds,
                        RawData6_ItemPricePerPound = oldDataRecords[2].RawData6_ItemPricePerPound,
                        DerivedData1_ItemPromotionDescription = oldDataRecords[2].DerivedData1_ItemPromotionDescription,
                        DerivedData2_ItemFinalCostAfterDiscountApplied = oldDataRecords[2].DerivedData2_ItemFinalCostAfterDiscountApplied
                    });
                    newDataRecords.Add(new Demo01_SalesRow
                    {
                        RawData1_StoreNbr = oldDataRecords[1].RawData1_StoreNbr,
                        RawData2_RegisterId = oldDataRecords[1].RawData2_RegisterId,
                        RawData3_TransactionDateTime = oldDataRecords[1].RawData3_TransactionDateTime,
                        RawData4_ItemUpc = oldDataRecords[1].RawData4_ItemUpc,
                        RawData5_ItemWeightInPounds = oldDataRecords[1].RawData5_ItemWeightInPounds,
                        RawData6_ItemPricePerPound = oldDataRecords[1].RawData6_ItemPricePerPound,
                        DerivedData1_ItemPromotionDescription = oldDataRecords[1].DerivedData1_ItemPromotionDescription,
                        DerivedData2_ItemFinalCostAfterDiscountApplied = oldDataRecords[1].DerivedData2_ItemFinalCostAfterDiscountApplied
                    });
                    newDataRecords.Add(new Demo01_SalesRow
                    {
                        RawData1_StoreNbr = oldDataRecords[3].RawData1_StoreNbr,
                        RawData2_RegisterId = oldDataRecords[3].RawData2_RegisterId,
                        RawData3_TransactionDateTime = oldDataRecords[3].RawData3_TransactionDateTime,
                        RawData4_ItemUpc = oldDataRecords[3].RawData4_ItemUpc,
                        RawData5_ItemWeightInPounds = oldDataRecords[3].RawData5_ItemWeightInPounds,
                        RawData6_ItemPricePerPound = oldDataRecords[3].RawData6_ItemPricePerPound,
                        DerivedData1_ItemPromotionDescription = oldDataRecords[3].DerivedData1_ItemPromotionDescription,
                        DerivedData2_ItemFinalCostAfterDiscountApplied = oldDataRecords[3].DerivedData2_ItemFinalCostAfterDiscountApplied
                    });
                    break;
                case "DataValuesMatchWithCloseDecimalRounding":
                    foreach (var oldDataRecord in oldDataRecords)
                    {
                        newDataRecords.Add(new Demo01_SalesRow
                        {
                            RawData1_StoreNbr = oldDataRecord.RawData1_StoreNbr,
                            RawData2_RegisterId = oldDataRecord.RawData2_RegisterId,
                            RawData3_TransactionDateTime = oldDataRecord.RawData3_TransactionDateTime,
                            RawData4_ItemUpc = oldDataRecord.RawData4_ItemUpc,
                            RawData5_ItemWeightInPounds = oldDataRecord.RawData5_ItemWeightInPounds,
                            RawData6_ItemPricePerPound = oldDataRecord.RawData6_ItemPricePerPound,
                            DerivedData1_ItemPromotionDescription = oldDataRecord.DerivedData1_ItemPromotionDescription,
                            DerivedData2_ItemFinalCostAfterDiscountApplied = oldDataRecord.DerivedData2_ItemFinalCostAfterDiscountApplied
                        });
                    }

                    // The difference is due to differnt decimal rounding behaviors between two different platforms.
                    // This could be a show stopper, or it could be OK...depending on what the business wants.
                    newDataRecords[2].DerivedData2_ItemFinalCostAfterDiscountApplied = 5.050M;
                    break;
                case "IrreconcilableDifferencesWereFound":
                    foreach (var oldDataRecord in oldDataRecords)
                    {
                        newDataRecords.Add(new Demo01_SalesRow
                        {
                            RawData1_StoreNbr = oldDataRecord.RawData1_StoreNbr,
                            RawData2_RegisterId = oldDataRecord.RawData2_RegisterId,
                            RawData3_TransactionDateTime = oldDataRecord.RawData3_TransactionDateTime,
                            RawData4_ItemUpc = oldDataRecord.RawData4_ItemUpc,
                            RawData5_ItemWeightInPounds = oldDataRecord.RawData5_ItemWeightInPounds,
                            RawData6_ItemPricePerPound = oldDataRecord.RawData6_ItemPricePerPound,
                            DerivedData1_ItemPromotionDescription = oldDataRecord.DerivedData1_ItemPromotionDescription,
                            DerivedData2_ItemFinalCostAfterDiscountApplied = oldDataRecord.DerivedData2_ItemFinalCostAfterDiscountApplied
                        });
                    }

                    // The derived data value has a bug, which is causing the final cost to be negative.
                    newDataRecords[2].DerivedData2_ItemFinalCostAfterDiscountApplied = -5.050M;
                    break;
                default:
                    //Cannot compare an empty collection.
                    break;
            }

            return (newDataRecords.ToArray(), oldDataRecords);
        }
    }
}
