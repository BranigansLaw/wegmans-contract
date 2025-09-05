using AutoMapper;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using Wegmans.POS.DataHub.MappingConfigurations;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub.UnitTests.MappingConfigurations
{
    public class Profile01StringTests
    {

        /// <summary>
        /// Tests automapper correctly maps fields from object type <see cref="TransactionRecord01"/> to <see cref="AceItem"/>
        /// </summary>
        [Theory]
        [ClassData(typeof(TransactionRecord01TestsCases))]
        public void Mapping_TransactionRecord01_To_AceItem_MapsAllFields(TransactionRecord01 from)
        {
            // Arrange
            MapperConfiguration config = new(cfg => cfg.AddProfile<Profile01String>());
            IMapper mapper = config.CreateMapper();

            // Act
            AceItem res = mapper.Map<TransactionRecord01, AceItem>(from);

            // Assert
            Assert.NotNull(res);

            Utility.AssertSameNameFieldsMatch(
                obj1: from,
                obj2: res,
                ignoreFields: new string[] { "VoidedOrdinalNumber", "IsUPCOfGS1DataBarCodeType", "IsRainCheckItem", "IsFuelItem", "HasRollbackPriceItem", "IsWeightItem", "HasPriceEntered", "HasPriceRequired", "IsLogAll", "HasLogException", "HasAliasItemCode", "HasNoMovement", "IsTaxableA", "IsTaxableB", "IsTaxableC", "IsTaxableD", "IsFoodStampEligable", "PointsItem", "IsNonDiscountable", "IsMultipleCouponAllowed", "HasFoodstampKey", "HasTaxKey", "HasCouponMultiple", "HasMultipriced", "HasDataEntry", "IsOverrideRequired", "IsExtensionExists", "IsNegativePriceDueToDeal", "WasDepositKey", "WasManufacturerCouponKey", "WasStoreCouponKey", "WasRefundKey", "WasCancelKey", "ItemType", "OperatorEntryMethod", "IsPharmacyItem", "IsQualifiedHealthcareProduct", "HasMarkdownCoupon", "WasAutomaticVoid", "IsTaxPlanH", "IsTaxPlanG", "IsTaxPlanF", "IsTaxPlanE", "WasFoodstampForgivenFee", "WasWICForgivenFeeVoid", "ExtendedPrice", "Quantity", "DepartmentNumber", "CouponFamilyNumber" });

            Assert.Equal(from.VoidedOrdinalNumber.ToNullableInt(), res.VoidedOrdinalNumber);
            Assert.Equal(from.Indicat1.Bit13.ToNullableBoolean(), res.IsUPCOfGS1DataBarCodeType);
            Assert.Equal(from.Indicat1.Bit14.ToNullableBoolean(), res.IsRainCheckItem);
            Assert.Equal(from.Indicat1.Bit15.ToNullableBoolean(), res.IsFuelItem);
            Assert.Equal(from.Indicat1.Bit16.ToNullableBoolean(), res.HasRollbackPriceItem);
            Assert.Equal(from.Indicat1.Bit17.ToNullableBoolean(), res.IsWeightItem);
            Assert.Equal(from.Indicat1.Bit18.ToNullableBoolean(), res.HasPriceEntered);
            Assert.Equal(from.Indicat1.Bit19.ToNullableBoolean(), res.HasPriceRequired);
            Assert.Equal(from.Indicat1.Bit20.ToNullableBoolean(), res.IsLogAll);
            Assert.Equal(from.Indicat1.Bit21.ToNullableBoolean(), res.HasLogException);
            Assert.Equal(from.Indicat1.Bit22.ToNullableBoolean(), res.HasAliasItemCode);
            Assert.Equal(from.Indicat1.Bit23.ToNullableBoolean(), res.HasNoMovement);
            Assert.Equal(from.Indicat1.Bit24.ToNullableBoolean(), res.IsTaxableA);
            Assert.Equal(from.Indicat1.Bit25.ToNullableBoolean(), res.IsTaxableB);
            Assert.Equal(from.Indicat1.Bit26.ToNullableBoolean(), res.IsTaxableC);
            Assert.Equal(from.Indicat1.Bit27.ToNullableBoolean(), res.IsTaxableD);
            Assert.Equal(from.Indicat1.Bit28.ToNullableBoolean(), res.IsFoodStampEligable);
            Assert.Equal(from.Indicat1.Bit30.ToNullableBoolean(), res.IsNonDiscountable);
            Assert.Equal(from.Indicat1.Bit31.ToNullableBoolean(), res.IsMultipleCouponAllowed);
            Assert.Equal(from.Indicat2.Bit0.ToNullableBoolean(), res.HasFoodstampKey);
            Assert.Equal(from.Indicat2.Bit1.ToNullableBoolean(), res.HasTaxKey);
            Assert.Equal(from.Indicat2.Bit2.ToNullableBoolean(), res.HasCouponMultiple);
            Assert.Equal(from.Indicat2.Bit4.ToNullableBoolean(), res.HasMultipriced);
            Assert.Equal(from.Indicat2.Bit5.ToNullableBoolean(), res.HasDataEntry);
            Assert.Equal(from.Indicat2.Bit6.ToNullableBoolean(), res.IsOverrideRequired);
            Assert.Equal(from.Indicat2.Bit9.ToNullableBoolean(), res.IsExtensionExists);
            Assert.Equal(from.Indicat2.Bit10.ToNullableBoolean(), res.IsNegativePriceDueToDeal);
            Assert.Equal(from.Indicat2.Bit11.ToNullableBoolean(), res.WasDepositKey);
            Assert.Equal(from.Indicat2.Bit12.ToNullableBoolean(), res.WasManufacturerCouponKey);
            Assert.Equal(from.Indicat2.Bit13.ToNullableBoolean(), res.WasStoreCouponKey);
            Assert.Equal(from.Indicat2.Bit14.ToNullableBoolean(), res.WasRefundKey);
            Assert.Equal(from.Indicat2.Bit15.ToNullableBoolean(), res.WasCancelKey);
            Assert.Equal(from.ItemType01Enum.ToString(), res.ItemType);
            Assert.Equal(from.OperatorEntryMethod01Enum.ToString(), res.OperatorEntryMethod);
            Assert.Equal(from.Indicat4.Bit0.ToNullableBoolean(), res.IsPharmacyItem);
            Assert.Equal(from.Indicat4.Bit1.ToNullableBoolean(), res.IsQualifiedHealthcareProduct);
            Assert.Equal(from.Indicat4.Bit2.ToNullableBoolean(), res.HasMarkdownCoupon);
            Assert.Equal(from.Indicat4.Bit3.ToNullableBoolean(), res.WasAutomaticVoid);
            Assert.Equal(from.Indicat4.Bit4.ToNullableBoolean(), res.IsTaxPlanH);
            Assert.Equal(from.Indicat4.Bit5.ToNullableBoolean(), res.IsTaxPlanG);
            Assert.Equal(from.Indicat4.Bit6.ToNullableBoolean(), res.IsTaxPlanF);
            Assert.Equal(from.Indicat4.Bit7.ToNullableBoolean(), res.IsTaxPlanE);
            Assert.Equal(from.Indicat4.Bit24.ToNullableBoolean(), res.WasFoodstampForgivenFee);
            Assert.Equal(from.Indicat4.Bit25.ToNullableBoolean(), res.WasWICForgivenFeeVoid);
            Assert.Equal(from.ExtendedPrice.Divide(), res.ExtendedPrice);
        }

        public class TransactionRecord01TestsCases : TheoryData<TransactionRecord01>
        {
            public TransactionRecord01TestsCases()
            {
                // Random Test Data
                AddRow(new TransactionRecord01
                {
                    StringType = 1,
                    ItemEntrySequenceNumber = 1,
                    UniversalProductCode = "ABC123",
                    ExtendedPrice = 1.23M,
                    DepartmentNumber = "5",
                    CouponFamilyNumber = "3",
                    Indicat0 = new Indicat1(),
                    Indicat1 = new Indicat1(),
                    Indicat2 = new Indicat2(),
                    Indicat3 = "ABC123",
                    Indicat4 = new Indicat4(),
                    OrdinalNumber = 1,
                    VoidedOrdinalNumber = 1
                });

                // Read from file
                foreach (TransactionRecord01 t in Utility.GetTransactionDataFromXml<TransactionRecord01>("01"))
                {
                    AddRow(t);
                }
            }
        }
    }
}