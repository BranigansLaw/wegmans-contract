using AutoMapper;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.MappingConfigurations;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub.UnitTests.MappingConfigurations
{
    public class StoreCloseProfileTests
    {
        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord21"/> to <see cref="StoreClose"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord21TestsCases))]
        public void Mapping_TransactionRecord21_To_StoreClose_MapsAllFields(TransactionRecord21 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<StoreCloseProfile>());
            IMapper mapper = config.CreateMapper();

            // Act
            var res = mapper.Map<TransactionRecord21, EnterpriseLibrary.Data.Hubs.POS.StoreClose.v1.StoreClose>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "CustomerAccountFileClosed", "ExceptionLogFileClosed", "TerminalProductivityFileClosed", "ItemMovementShortFileClosed", "TenderListingFileClosed", "ItemMovementLongFileClosed", "CloseTypeIndication", "CloseProcedure" });

            Assert.Equal(from.Indicat2.Bit0.ToNullableBoolean(), res.CustomerAccountFileClosed);
            Assert.Equal(from.Indicat2.Bit1.ToNullableBoolean(), res.ExceptionLogFileClosed);
            Assert.Equal(from.Indicat2.Bit2.ToNullableBoolean(), res.TerminalProductivityFileClosed);
            Assert.Equal(from.Indicat2.Bit3.ToNullableBoolean(), res.ItemMovementShortFileClosed);
            Assert.Equal(from.Indicat2.Bit4.ToNullableBoolean(), res.TenderListingFileClosed);
            Assert.Equal(from.Indicat2.Bit5.ToNullableBoolean(), res.ItemMovementLongFileClosed);
            Assert.Equal(from.CloseType.ConvertToCloseTypeIndication(), res.CloseTypeIndication);
            Assert.Equal(from.CloseProcedure.ToNullableInt(), res.CloseProcedure);
        }

        public class TransactionRecord21TestsCases : TheoryData<TransactionRecord21>
        {
            public TransactionRecord21TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord21()
                {
                    StringType = 1,
                    DateTimeString = "2407261938",
                    CloseType = "ABC123",
                    Indicat2 = new Indicat2(),
                    AutoPickup = "ABC123",
                    CloseProcedure = 1
                });

                // Read from file
                foreach (TransactionRecord21 t in Utility.GetTransactionDataFromXml<TransactionRecord21>("21"))
                {
                    AddRow(t);
                }
            }
        }
    }
}