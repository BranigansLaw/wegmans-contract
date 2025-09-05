using AutoMapper;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.MappingConfigurations;

namespace Wegmans.POS.DataHub.UnitTests.MappingConfigurations
{
    public class ProfileCustomerStringsTests
    {

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord11EE"/> to <see cref="CustomerIdentification"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord11EETestsCases))]
        public void Mapping_TransactionRecord11EE_To_CustomerIdentification_MapsAllFields(TransactionRecord11EE from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileCustomerStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            CustomerIdentification res = mapper.Map<TransactionRecord11EE, CustomerIdentification>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "LoyaltyNumber", "CouponAmount", "CouponCount", "EntryMethod" });

            Assert.Equal(from.CustomerAccountID, res.LoyaltyNumber);
            Assert.Equal(from.CouponAmount, res.CouponAmount);
            Assert.Equal(from.CouponCount, res.CouponCount);
            Assert.Equal(from.getEntryMethod(), res.EntryMethod);
        }

        public class TransactionRecord11EETestsCases : TheoryData<TransactionRecord11EE>
        {
            public TransactionRecord11EETestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord11EE
                {
                    StringType = 1,
                    SubStringType = "ABC123",
                    CustomerAccountID = "ABC123",
                    Points = 1,
                    CouponAmount = 1,
                    CouponCount = 1,
                    MessageCount = 1,
                    TransferredTransactionCount = 1,
                    TransferredTransactionAmount = 1,
                    BonusPoints = 1,
                    RedeemedPoints = 1,
                    EntryMethod = 1
                });

                // Read from file
                foreach (TransactionRecord11EE t in Utility.GetTransactionDataFromXml<TransactionRecord11EE>("11", null, null, "EE"))
                {
                    AddRow(t);
                }
            }
        }

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord11FF"/> to <see cref="CustomerIdentification"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord11FFTestsCases))]
        public void Mapping_TransactionRecord11FF_To_CustomerIdentification_MapsAllFields(TransactionRecord11FF from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<ProfileCustomerStrings>());
            IMapper mapper = config.CreateMapper();

            // Act
            CustomerIdentification res = mapper.Map<TransactionRecord11FF, CustomerIdentification>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "EntryMethod" });

            Assert.Equal(res.EntryMethod, from.getEntryMethod());
        }

        public class TransactionRecord11FFTestsCases : TheoryData<TransactionRecord11FF>
        {
            public TransactionRecord11FFTestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord11FF
                {
                    StringType = 1,
                    SubStringType = "ABC123",
                    LoyaltyNumber = "ABC123",
                    StoreNumber = 1,
                    EntryMethod = 1,
                    Unused = 1
                });

                // Read from file
                foreach (TransactionRecord11FF t in Utility.GetTransactionDataFromXml<TransactionRecord11FF>("11", null, null, "FF"))
                {
                    AddRow(t);
                }
            }
        }
    }
}