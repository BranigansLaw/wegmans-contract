using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations;

public class PetPtNumsQueryTests
{
    /// <summary>
    /// Tests that <see cref="PetPtNumsQuery"/> correctly maps all fields
    /// </summary>
    [Theory]
    [ClassData(typeof(PetPtNumsQueryTests.MappingTests))]
    public void MapFromDataReaderToType_Returns_MappedObject((object[], PetPtNumRow) testParameters)
    {
        // Arrange
        (object[] readerIndex, PetPtNumRow expectedResult) = testParameters;
        PetPtNumsQuery query = new() { };
        DbDataReader mockedReader = Substitute.For<DbDataReader>();
        Util.SetupNullableReturn(mockedReader, readerIndex, 0);
        mockedReader.GetString(1).Returns(readerIndex[1]);
        Util.SetupNullableReturn(mockedReader, readerIndex, 2);
        mockedReader.GetString(3).Returns(readerIndex[3]);

        // Act
        PetPtNumRow res = query.MapFromDataReaderToType(mockedReader, s => { });

        // Assert
        Assert.NotNull(res);
        Assert.Equal(expectedResult.PatientNum, res.PatientNum);
        Assert.Equal(expectedResult.Species, res.Species);
        Assert.Equal(expectedResult.CreateDate, res.CreateDate);
        Assert.Equal(expectedResult.Pet, res.Pet);
    }

    public class MappingTests : TheoryData<(object[], PetPtNumRow)>
    {
        public MappingTests()
        {
            AddRow((
                new object[] {
                        (long) 12345678,
                        "Cat/Feline",
                        new DateTime(2024, 08, 25, 08, 21, 25),
                        "Y"
                },
                new PetPtNumRow
                {
                    PatientNum = 12345678,
                    Species = "Cat/Feline",
                    CreateDate = new DateTime(2024, 08, 25, 08, 21, 25),
                    Pet = "Y"
                }
            ));

            AddRow((
                new object[] {
                        (long) 98765443,
                        "Dog/Canine",
                        new DateTime(2022, 01, 22, 10, 12, 44),
                        "Y"
                },
                new PetPtNumRow
                {
                    PatientNum = 98765443,
                    Species = "Dog/Canine",
                    CreateDate = new DateTime(2022, 01, 22, 10, 12, 44),
                    Pet = "Y"
                }
            ));
        }
    }
}
