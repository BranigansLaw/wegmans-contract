using AutoMapper;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using Wegmans.POS.DataHub.MappingConfigurations;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub.UnitTests.MappingConfigurations
{
    public class Profile02StringTests
    {

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord02"/> to <see cref="AceItem"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord02TestsCases))]
        public void Mapping_TransactionRecord02_To_AceItem_MapsAllFields(TransactionRecord02 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<Profile02String>());
            IMapper mapper = config.CreateMapper();

            // Act
            AceItem res = mapper.Map<TransactionRecord02, AceItem>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "TareWeight", "HasCouponQuantity", "HasRepeatQuantity", "IsQuantityRequired", "HasScaleWeight", "HasTareKey", "WasQuantityKeyPressed", "WasWeightOrVolumeKey", "WasDealQuantityKeyed", "SalePrice", "Quantity", "Weight" });

            Assert.Equal(from.UserValue, res.TareWeight);
            Assert.Equal(from.Indicat1.Bit0.ToNullableBoolean(), res.HasCouponQuantity);
            Assert.Equal(from.Indicat1.Bit1.ToNullableBoolean(), res.HasRepeatQuantity);
            Assert.Equal(from.Indicat1.Bit2.ToNullableBoolean(), res.IsQuantityRequired);
            Assert.Equal(from.Indicat1.Bit3.ToNullableBoolean(), res.HasScaleWeight);
            Assert.Equal(from.Indicat1.Bit4.ToNullableBoolean(), res.HasTareKey);
            Assert.Equal(from.Indicat1.Bit5.ToNullableBoolean(), res.WasQuantityKeyPressed);
            Assert.Equal(from.Indicat1.Bit6.ToNullableBoolean(), res.WasWeightOrVolumeKey);
            Assert.Equal(from.Indicat1.Bit7.ToNullableBoolean(), res.WasDealQuantityKeyed);
            Assert.Equal(from.SalePrice.Divide(), res.SalePrice);
            Assert.Equal(from.Indicat1.Bit5.ToNullableBoolean().getQuantityValue(from), res.Quantity);
            Assert.Equal(from.Indicat1.Bit6.ToNullableBoolean().getWeightValue(from), res.Weight);
        }

        public class TransactionRecord02TestsCases : TheoryData<TransactionRecord02>
        {
            public TransactionRecord02TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord02
                {
                    StringType = 1,
                    ItemEntrySequenceNumber = 1,
                    MultiPricingGroup = "ABC123",
                    DealQuantity = "ABC123",
                    PricingMethod = "ABC123",
                    SaleQuantity = 1,
                    SalePrice = 1.23M,
                    QuantityOrWeightOrVolume = "ABC123",
                    Indicat1 = new Indicat1(),
                    UserValue = "ABC123"
                });

                // Read from file
                foreach (TransactionRecord02 t in Utility.GetTransactionDataFromXml<TransactionRecord02>("02"))
                {
                    AddRow(t);
                }
            }
        }
    }
}