using Xunit;
using YYYQATools.CompareOutputFiles.Helper;

namespace ZZZTest.YYYQATools.CompareOutputFiles.Helper
{
    public class CompareCollectionsHelperImpTests
    {
        private readonly CompareCollectionsHelperImp _sut;

        public CompareCollectionsHelperImpTests()
        {
            _sut = new CompareCollectionsHelperImp("MockNewFileContents", "MockOldFileContents");
        }

        [Theory]
        [InlineData("Success_PerfectlyIdentical", true, "", "")]
        [InlineData("ConditionalSuccess_SameValuesButDifferentSortOrder", false, "2", "")]
        [InlineData("Failure_DifferentValuesFound", false, "1", "1")]
        public void FilesArePerfectlyIdentical_ShouldReturnExpectedResult(
            string scenario, 
            bool expectedResult,
            string expectedNotMatchedNewRecordIndexesString,
            string expectedNotMatchedOldRecordIndexesString)
        {
            // Setup
            var (newFileContents, oldFileContents) = GetNewAndOldFileContents(scenario);
            List<string> tempNewRecordIndexes = string.IsNullOrEmpty(expectedNotMatchedNewRecordIndexesString) ? [] : expectedNotMatchedNewRecordIndexesString.Split(',').ToList();
            List<string> tempOldRecordIndexes = string.IsNullOrEmpty(expectedNotMatchedOldRecordIndexesString) ? [] : expectedNotMatchedOldRecordIndexesString.Split(',').ToList();
            List<int> expectedNotMatchedNewRecordIndexes = tempNewRecordIndexes.Count() > 0 ? tempNewRecordIndexes.Select(int.Parse).ToList() : [];
            List<int> expectedNotMatchedOldRecordIndexes = tempOldRecordIndexes.Count() > 0 ? tempOldRecordIndexes.Select(int.Parse).ToList() : [];

            // Act
            var actualResult = _sut.FilesArePerfectlyIdentical(newFileContents, oldFileContents);

            // Assert
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedNotMatchedNewRecordIndexes, _sut.NotMatchedNewRecordIndexes);
            Assert.Equal(expectedNotMatchedOldRecordIndexes, _sut.NotMatchedOldRecordIndexes);
        }

        [Theory]
        [InlineData("Success_PerfectlyIdentical", true, "", "")]
        [InlineData("ConditionalSuccess_SameValuesButDifferentSortOrder", true, "", "")]
        [InlineData("Failure_DifferentValuesFound", false, "1", "1")]
        public void LinesFromFilesMatchButHaveDifferentSortOrder_ShouldReturnExpectedResult(
            string scenario,
            bool expectedResult,
            string expectedNotMatchedNewRecordIndexesString,
            string expectedNotMatchedOldRecordIndexesString)
        {
            // Setup
            var (newFileContents, oldFileContents) = GetNewAndOldFileContents(scenario);
            List<string> tempNewRecordIndexes = string.IsNullOrEmpty(expectedNotMatchedNewRecordIndexesString) ? [] : expectedNotMatchedNewRecordIndexesString.Split(',').ToList();
            List<string> tempOldRecordIndexes = string.IsNullOrEmpty(expectedNotMatchedOldRecordIndexesString) ? [] : expectedNotMatchedOldRecordIndexesString.Split(',').ToList();
            List<int> expectedNotMatchedNewRecordIndexes = tempNewRecordIndexes.Count() > 0 ? tempNewRecordIndexes.Select(int.Parse).ToList() : [];
            List<int> expectedNotMatchedOldRecordIndexes = tempOldRecordIndexes.Count() > 0 ? tempOldRecordIndexes.Select(int.Parse).ToList() : [];

            // Act
            var actualResult = _sut.LinesFromFilesMatchButHaveDifferentSortOrder(newFileContents, oldFileContents);

            // Assert
            Assert.Equal(expectedResult, actualResult);
            Assert.Equal(expectedNotMatchedNewRecordIndexes, _sut.NotMatchedNewRecordIndexes);
            Assert.Equal(expectedNotMatchedOldRecordIndexes, _sut.NotMatchedOldRecordIndexes);
        }

        private (string, string) GetNewAndOldFileContents(string scenario)
        {
            string rowDelimiter = _sut.FileRowDelimiter;

            switch (scenario)
            {
                case "Success_PerfectlyIdentical":
                    return (
                        string.Format("OrderNbr,Cost{0}AAAA,10.00{0}BBBB,20.00", rowDelimiter),
                        string.Format("OrderNbr,Cost{0}AAAA,10.00{0}BBBB,20.00", rowDelimiter));
                case "ConditionalSuccess_SameValuesButDifferentSortOrder":
                    return (
                        string.Format("OrderNbr,Cost{0}BBBB,20.00{0}AAAA,10.00", rowDelimiter),
                        string.Format("OrderNbr,Cost{0}AAAA,10.00{0}BBBB,20.00", rowDelimiter));
                case "Failure_DifferentValuesFound":
                    return (
                        string.Format("OrderNbr,Cost{0}AAAA,10.00{0}BBBB,20.00", rowDelimiter),
                        string.Format("OrderNbr,Cost{0}AAAA,10.01{0}BBBB,20.00", rowDelimiter));
                default:
                    return (string.Empty, string.Empty);
            }
        }
    }
}
