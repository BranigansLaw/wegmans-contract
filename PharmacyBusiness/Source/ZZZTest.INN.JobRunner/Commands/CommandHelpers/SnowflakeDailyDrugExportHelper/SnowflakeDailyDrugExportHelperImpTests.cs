using INN.JobRunner.Commands.CommandHelpers.SnowflakeDailyDrugExportHelper;
using Library.LibraryUtilities.Extensions;


namespace ZZZTest.INN.JobRunner.Commands.CommandHelpers.SnowflakeDailyDrugExportHelper;

public class SnowflakeDailyDrugExportHelperImpTests
{
    private readonly HelperImp _sut;

    public SnowflakeDailyDrugExportHelperImpTests()
    {
        _sut = new HelperImp();
    }

    [Theory]
    [InlineData("groupName", 162241917, 162241917, "Active", 1000019, "g")]
    [InlineData("groupName", 162241917, 162241916, "Active", 1000019, null)]
    [InlineData("groupName", 162241917, 162241917, "Active", 5, null)]
    [InlineData("", 162241917, 162241917, "Active", 1000019, null)]
    public void DeriveDecile_ShouldReturnExpectedResults(string? groupName,
        int? pPrdProductKey, 
        int? pgPrdProductKey,
        string? pgMemberStatus,
        int? gsGrpGroupNumber,
        string expectedResult)
    {
        // Arrange
        
        // Act
        string? actualResult = _sut.DeriveDecile(groupName, pPrdProductKey, pgPrdProductKey, pgMemberStatus, gsGrpGroupNumber);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Theory]
    [InlineData("groupName", 162241917, 162241917, "Active", 1000010, "g")]
    [InlineData("groupName", 162241917, 162241916, "Active", 1000010, null)]
    [InlineData("groupName", 162241917, 162241917, "Active", 5, null)]
    [InlineData("", 162241917, 162241917, "Active", 1000010, null)]
    public void DeriveOtcType_ShouldReturnExpectedResults(string? groupName, 
        int? pPrdProductKey,
        int? pgPrdProductKey,
        string? pgMemberStatus,
        int? gsGrpGroupNumber,
        string expectedResult)
    {
        // Arrange

        // Act
        string? actualResult = _sut.DeriveOtcType(groupName, pPrdProductKey, pgPrdProductKey, pgMemberStatus, gsGrpGroupNumber);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    [Theory]
    [ClassData(typeof(SnowflakeDailyDrugExportHelperImpTests.CostBasisValues))]
    public void DeriveCostBasisEffDate_ShouldReturnExpectedResults((TestDeriveCostBasisEffDateParameters, string) testParameters)
    {
        // Arrange
        (TestDeriveCostBasisEffDateParameters testValues, string expectedResult) = testParameters;

        // Act
        string? actualResult = _sut.DeriveCostBasisEffDate(testValues.PcfrCost,
            testValues.PcfcCost,
            testValues.PcfnCost,
            (testValues.UserDefDate.HasValue) ? testValues.UserDefDate.Value.ToString("MM/dd/yyyy hh:mm:ss tt") : string.Empty,
            (testValues.NteEffDate.HasValue) ? testValues.NteEffDate.Value.ToString("MM/dd/yyyy hh:mm:ss tt") : string.Empty,
            (testValues.ConDate.HasValue) ? testValues.ConDate.Value.ToString("MM/dd/yyyy hh:mm:ss tt") : string.Empty,
            (testValues.RepackconDate.HasValue) ? testValues.RepackconDate.Value.ToString("MM/dd/yyyy hh:mm:ss tt") : string.Empty);

        // Assert
        Assert.Equal(actualResult, expectedResult);
    }

    public class TestDeriveCostBasisEffDateParameters 
    {
        public required decimal? PcfrCost { get; set; }
        public required decimal? PcfcCost { get; set; }
        public required decimal? PcfnCost { get; set; }
        public required DateTime? UserDefDate { get; set; }
        public required DateTime? NteEffDate { get; set; }
        public required DateTime? ConDate { get; set; }
        public required DateTime? RepackconDate { get; set; }
    }


    public class CostBasisValues : TheoryData<(TestDeriveCostBasisEffDateParameters, string)>
    {
        public CostBasisValues()
        {
            AddRow((
                new TestDeriveCostBasisEffDateParameters 
                {
                    PcfrCost = null,
                    PcfcCost = null,
                    PcfnCost = 2.91521M,
                    UserDefDate = null,
                    NteEffDate = new DateTime(2013, 11, 13),
                    ConDate = null,
                    RepackconDate = null
                },
                "11/13/2013 12:00:00 AM"
            ));

            AddRow((
                new TestDeriveCostBasisEffDateParameters
                {
                    PcfrCost = null,
                    PcfcCost = null,
                    PcfnCost = 3.22111M,
                    UserDefDate = null,
                    NteEffDate = new DateTime(2006, 1, 10),
                    ConDate = null,
                    RepackconDate = null
                },
                "01/10/2006 12:00:00 AM"
             ));

            AddRow((
                new TestDeriveCostBasisEffDateParameters
                {
                    PcfrCost = null,
                    PcfcCost = 0.064333M,
                    PcfnCost = 0.07M,
                    UserDefDate = null,
                    NteEffDate = new DateTime(2019, 3, 18),
                    ConDate = new DateTime(2019, 5, 13),
                    RepackconDate = null
                },
                "05/13/2019 12:00:00 AM"
            ));

            AddRow((
                new TestDeriveCostBasisEffDateParameters
                {
                    PcfrCost = 0.2904M,
                    PcfcCost = 0.7424M,
                    PcfnCost = 0.7424M,
                    UserDefDate = null,
                    NteEffDate = new DateTime(2007, 10, 07),
                    ConDate = new DateTime(2007, 10, 30),
                    RepackconDate = new DateTime(2007, 10, 30)
                },
                "10/30/2007 12:00:00 AM"
            ));
        }
    }
}
