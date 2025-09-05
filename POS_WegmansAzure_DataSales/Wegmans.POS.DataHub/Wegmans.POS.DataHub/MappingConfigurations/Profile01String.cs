using AutoMapper;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub.MappingConfigurations
{
    public class Profile01String : Profile
    {
        public Profile01String()
        {
            CreateMap<TransactionRecord01, AceItem>()
                .ForMember(dest => dest.VoidedOrdinalNumber, opt => opt.MapFrom(src => src.VoidedOrdinalNumber.ToNullableInt()))
                //Indicat1
                .ForMember(dest => dest.IsUPCOfGS1DataBarCodeType, opt => opt.MapFrom(src => src.Indicat1.Bit13.ToNullableBoolean()))
                .ForMember(dest => dest.IsRainCheckItem, opt => opt.MapFrom(src => src.Indicat1.Bit14.ToNullableBoolean()))
                .ForMember(dest => dest.IsFuelItem, opt => opt.MapFrom(src => src.Indicat1.Bit15.ToNullableBoolean()))
                .ForMember(dest => dest.HasRollbackPriceItem, opt => opt.MapFrom(src => src.Indicat1.Bit16.ToNullableBoolean()))
                .ForMember(dest => dest.IsWeightItem, opt => opt.MapFrom(src => src.Indicat1.Bit17.ToNullableBoolean()))
                .ForMember(dest => dest.HasPriceEntered, opt => opt.MapFrom(src => src.Indicat1.Bit18.ToNullableBoolean()))
                .ForMember(dest => dest.HasPriceRequired, opt => opt.MapFrom(src => src.Indicat1.Bit19.ToNullableBoolean()))
                .ForMember(dest => dest.IsLogAll, opt => opt.MapFrom(src => src.Indicat1.Bit20.ToNullableBoolean()))
                .ForMember(dest => dest.HasLogException, opt => opt.MapFrom(src => src.Indicat1.Bit21.ToNullableBoolean()))
                .ForMember(dest => dest.HasAliasItemCode, opt => opt.MapFrom(src => src.Indicat1.Bit22.ToNullableBoolean()))
                .ForMember(dest => dest.HasNoMovement, opt => opt.MapFrom(src => src.Indicat1.Bit23.ToNullableBoolean()))
                .ForMember(dest => dest.IsTaxableA, opt => opt.MapFrom(src => src.Indicat1.Bit24.ToNullableBoolean()))
                .ForMember(dest => dest.IsTaxableB, opt => opt.MapFrom(src => src.Indicat1.Bit25.ToNullableBoolean()))
                .ForMember(dest => dest.IsTaxableC, opt => opt.MapFrom(src => src.Indicat1.Bit26.ToNullableBoolean()))
                .ForMember(dest => dest.IsTaxableD, opt => opt.MapFrom(src => src.Indicat1.Bit27.ToNullableBoolean()))
                .ForMember(dest => dest.IsFoodStampEligable, opt => opt.MapFrom(src => src.Indicat1.Bit28.ToNullableBoolean()))
                //.ForMember(dest => dest.PointsItem, opt => opt.MapFrom(src => src.Indicat1.Bit29.ToNullableBoolean()))
                .ForMember(dest => dest.IsNonDiscountable, opt => opt.MapFrom(src => src.Indicat1.Bit30.ToNullableBoolean()))
                .ForMember(dest => dest.IsMultipleCouponAllowed, opt => opt.MapFrom(src => src.Indicat1.Bit31.ToNullableBoolean()))
                //Indicat2
                .ForMember(dest => dest.HasFoodstampKey, opt => opt.MapFrom(src => src.Indicat2.Bit0.ToNullableBoolean()))
                .ForMember(dest => dest.HasTaxKey, opt => opt.MapFrom(src => src.Indicat2.Bit1.ToNullableBoolean()))
                .ForMember(dest => dest.HasCouponMultiple, opt => opt.MapFrom(src => src.Indicat2.Bit2.ToNullableBoolean()))
                .ForMember(dest => dest.HasMultipriced, opt => opt.MapFrom(src => src.Indicat2.Bit4.ToNullableBoolean()))
                .ForMember(dest => dest.HasDataEntry, opt => opt.MapFrom(src => src.Indicat2.Bit5.ToNullableBoolean()))
                .ForMember(dest => dest.IsOverrideRequired, opt => opt.MapFrom(src => src.Indicat2.Bit6.ToNullableBoolean()))
                .ForMember(dest => dest.IsExtensionExists, opt => opt.MapFrom(src => src.Indicat2.Bit9.ToNullableBoolean()))
                .ForMember(dest => dest.IsNegativePriceDueToDeal, opt => opt.MapFrom(src => src.Indicat2.Bit10.ToNullableBoolean()))
                .ForMember(dest => dest.WasDepositKey, opt => opt.MapFrom(src => src.Indicat2.Bit11.ToNullableBoolean()))
                .ForMember(dest => dest.WasManufacturerCouponKey, opt => opt.MapFrom(src => src.Indicat2.Bit12.ToNullableBoolean()))
                .ForMember(dest => dest.WasStoreCouponKey, opt => opt.MapFrom(src => src.Indicat2.Bit13.ToNullableBoolean()))
                .ForMember(dest => dest.WasRefundKey, opt => opt.MapFrom(src => src.Indicat2.Bit14.ToNullableBoolean()))
                .ForMember(dest => dest.WasCancelKey, opt => opt.MapFrom(src => src.Indicat2.Bit15.ToNullableBoolean()))
                //Indicat3
                .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => src.ItemType01Enum))
                .ForMember(dest => dest.OperatorEntryMethod, opt => opt.MapFrom(src => src.OperatorEntryMethod01Enum))
                //Indicat4
                .ForMember(dest => dest.IsPharmacyItem, opt => opt.MapFrom(src => src.Indicat4.Bit0.ToNullableBoolean()))
                .ForMember(dest => dest.IsQualifiedHealthcareProduct, opt => opt.MapFrom(src => src.Indicat4.Bit1.ToNullableBoolean()))
                .ForMember(dest => dest.HasMarkdownCoupon, opt => opt.MapFrom(src => src.Indicat4.Bit2.ToNullableBoolean()))
                .ForMember(dest => dest.WasAutomaticVoid, opt => opt.MapFrom(src => src.Indicat4.Bit3.ToNullableBoolean()))
                .ForMember(dest => dest.IsTaxPlanH, opt => opt.MapFrom(src => src.Indicat4.Bit4.ToNullableBoolean()))
                .ForMember(dest => dest.IsTaxPlanG, opt => opt.MapFrom(src => src.Indicat4.Bit5.ToNullableBoolean()))
                .ForMember(dest => dest.IsTaxPlanF, opt => opt.MapFrom(src => src.Indicat4.Bit6.ToNullableBoolean()))
                .ForMember(dest => dest.IsTaxPlanE, opt => opt.MapFrom(src => src.Indicat4.Bit7.ToNullableBoolean()))
                .ForMember(dest => dest.WasFoodstampForgivenFee, opt => opt.MapFrom(src => src.Indicat4.Bit24.ToNullableBoolean()))
                .ForMember(dest => dest.WasWICForgivenFeeVoid, opt => opt.MapFrom(src => src.Indicat4.Bit25.ToNullableBoolean()))

                //Decimal
                .ForMember(dest => dest.ExtendedPrice, opt => opt.MapFrom(src => src.ExtendedPrice.Divide()))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => 1))  //default to 1.  Extension will override this value if needed
            ;
        }
    }
}