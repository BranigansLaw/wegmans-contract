using AutoMapper;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using Wegmans.POS.DataHub.MappingConfigurations;

namespace Wegmans.POS.DataHub.UnitTests.MappingConfigurations
{
    public class Profile991027StringTests
    {

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord991027"/> to <see cref="AceItem"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord991027TestsCases))]
        public void Mapping_TransactionRecord991027_To_AceItem_MapsAllFields(TransactionRecord991027 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<Profile991027String>());
            IMapper mapper = config.CreateMapper();

            // Act
            AceItem res = mapper.Map<TransactionRecord991027, AceItem>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[0]);
        }

        public class TransactionRecord991027TestsCases : TheoryData<TransactionRecord991027>
        {
            public TransactionRecord991027TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord991027
                {
                    StringType = 1,
                    Originator = 1,
                    SubType = 1,
                    ItemNumber = 1
                });

                // Read from file
                foreach (TransactionRecord991027 t in Utility.GetTransactionDataFromXml<TransactionRecord991027>("99", "10", "27"))
                {
                    AddRow(t);
                }
            }
        }
    }
}