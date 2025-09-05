using AutoMapper;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.MappingConfigurations;

namespace Wegmans.POS.DataHub.UnitTests.MappingConfigurations
{
    public class Profile995013StringTests
    {

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord995013"/> to <see cref="Transaction"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(Profile995013StringTestsCases))]
        public void Mapping_TransactionRecord995013_To_Transaction_MapsAllFields(TransactionRecord995013 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<Profile995013String>());
            IMapper mapper = config.CreateMapper();

            // Act
            Transaction res = mapper.Map<TransactionRecord995013, Transaction>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "InmarStatus" });

            Assert.Equal(from.getQueryResultMeaning(), res.InmarStatus);
        }

        public class Profile995013StringTestsCases : TheoryData<TransactionRecord995013>
        {
            public Profile995013StringTestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord995013
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    QueryResult = 1
                });

                // Read from file
                foreach (TransactionRecord995013 t in Utility.GetTransactionDataFromXml<TransactionRecord995013>("99", "50", "13"))
                {
                    AddRow(t);
                }
            }
        }
    }
}