using Library.SnowflakeInterface.Data;
using Library.SnowflakeInterface.QueryConfigurations;
using NSubstitute;
using System.Data.Common;

namespace ZZZTest.Library.SnowflakeInterface.QueryConfigurations
{
    public class DeceasedQueryTest
    {
        /// <summary>
        /// Tests that <see cref="DeceasedQuery"/> correctly maps all fields
        /// </summary>
        [Theory]
        [ClassData(typeof(DeceasedQueryTest.MappingTests))]
        public void MapFromDataReaderToType_Returns_MappedObject((object[], DeceasedRow) testParameters)
        {
            // Arrange
            (object[] readerIndex, DeceasedRow expectedResult) = testParameters;
            DeceasedQuery query = new DeceasedQuery
            {
                RunDate = DateOnly.FromDateTime(DateTime.Now)
            };
            DbDataReader mockedReader = Substitute.For<DbDataReader>();
            mockedReader.GetFieldValue<long>(0).Returns(readerIndex[0]);

            // Act
            DeceasedRow res = query.MapFromDataReaderToType(mockedReader, s => { });

            // Assert
            Assert.NotNull(res);
            Assert.Equal(expectedResult.PdPatientNumber, res.PdPatientNumber);
        }

        public class MappingTests : TheoryData<(object[], DeceasedRow)>
        {
            public MappingTests()
            {
                AddRow((
                    new object[] {
                        (long) 3458389
                    },
                    new DeceasedRow
                    {
                        PdPatientNumber = 3458389
                    }
                ));

                AddRow((
                    new object[] {
                        (long) 903486093
                    },
                    new DeceasedRow
                    {
                        PdPatientNumber = 903486093
                    }
                ));
            }
        }
    }
}