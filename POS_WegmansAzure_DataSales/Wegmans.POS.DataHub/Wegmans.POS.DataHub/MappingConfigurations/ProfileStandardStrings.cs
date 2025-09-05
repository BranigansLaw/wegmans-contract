using AutoMapper;
using Wegmans.POS.DataHub.Util;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.MappingConfigurations
{
    public class ProfileStandardStrings : Profile
    {
        public ProfileStandardStrings()
        {
            CreateMap<TransactionRecord03, Discount>()
                .ForMember(dest => dest.AssociatedCouponCode, opt => opt.MapFrom(src => src.GroupId.convertDiscountGroupToCouponCode()))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Divide() ?? 0))
            ;
            CreateMap<TransactionRecord04, Discount>()
                .ForMember(dest => dest.AssociatedCouponCode, opt => opt.MapFrom(src => src.GroupId.convertDiscountGroupToCouponCode()))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Divide().Negate() ?? 0))
            ;
            CreateMap<TransactionRecord05, TenderExchange>()
                .ForMember(dest => dest.TenderType, opt => opt.MapFrom(src => src.TenderType.getTenderTypeName()))
                .ForMember(dest => dest.TenderAmount, opt => opt.MapFrom(src => src.TenderAmount.Divide() ?? 0))
                .ForMember(dest => dest.TenderCashingFee, opt => opt.MapFrom(src => src.TenderCashingFee.Divide()))
            ;
            CreateMap<TransactionRecord06, TenderVoid>()
                .ForMember(dest => dest.TenderType, opt => opt.MapFrom(src => src.TenderType.getTenderTypeName()))
                .ForMember(dest => dest.TenderAmount, opt => opt.MapFrom(src => src.TenderAmount.Divide()))
                .ForMember(dest => dest.TenderCashingFee, opt => opt.MapFrom(src => src.TenderCashingFee.Divide()))
            ;
            CreateMap<TransactionRecord07, TaxData>()
                .ForMember(dest => dest.TaxAAmount, opt => opt.MapFrom(src => src.TaxAAmount.Divide()))
                .ForMember(dest => dest.TaxBAmount, opt => opt.MapFrom(src => src.TaxBAmount.Divide()))
                .ForMember(dest => dest.TaxCAmount, opt => opt.MapFrom(src => src.TaxCAmount.Divide()))
                .ForMember(dest => dest.TaxASales, opt => opt.MapFrom(src => src.TaxASales.Divide()))
                .ForMember(dest => dest.TaxBSales, opt => opt.MapFrom(src => src.TaxBSales.Divide()))
                .ForMember(dest => dest.TaxCSales, opt => opt.MapFrom(src => src.TaxCSales.Divide()))
                .ForMember(dest => dest.TaxDSales, opt => opt.MapFrom(src => src.TaxDSales.Divide()))
                .ForMember(dest => dest.TaxDAmount, opt => opt.MapFrom(src => src.TaxDAmount.Divide()))
                .ForMember(dest => dest.TaxESales, opt => opt.MapFrom(src => src.TaxESales.Divide()))
                .ForMember(dest => dest.TaxEAmount, opt => opt.MapFrom(src => src.TaxEAmount.Divide()))
                .ForMember(dest => dest.TaxFSales, opt => opt.MapFrom(src => src.TaxFSales.Divide()))
                .ForMember(dest => dest.TaxFAmount, opt => opt.MapFrom(src => src.TaxFAmount.Divide()))
                .ForMember(dest => dest.TaxGSales, opt => opt.MapFrom(src => src.TaxGSales.Divide()))
                .ForMember(dest => dest.TaxGAmount, opt => opt.MapFrom(src => src.TaxGAmount.Divide()))
                .ForMember(dest => dest.TaxHSales, opt => opt.MapFrom(src => src.TaxHSales.Divide()))
                .ForMember(dest => dest.TaxHAmount, opt => opt.MapFrom(src => src.TaxHAmount.Divide()))
                .ForMember(dest => dest.FinalAmount, opt => opt.MapFrom(src => src.FinalAmount.Divide()))
                .ForMember(dest => dest.SubTotalAmount, opt => opt.MapFrom(src => src.SubTotalAmount.Divide()))
            ;
            CreateMap<TransactionRecord08, TaxRefund>()
                .ForMember(dest => dest.TaxAAmount, opt => opt.MapFrom(src => src.TaxAAmount.Divide()))
                .ForMember(dest => dest.TaxBAmount, opt => opt.MapFrom(src => src.TaxBAmount.Divide()))
                .ForMember(dest => dest.TaxCAmount, opt => opt.MapFrom(src => src.TaxCAmount.Divide()))
                .ForMember(dest => dest.TaxASales, opt => opt.MapFrom(src => src.TaxASales.Divide()))
                .ForMember(dest => dest.TaxBSales, opt => opt.MapFrom(src => src.TaxBSales.Divide()))
                .ForMember(dest => dest.TaxCSales, opt => opt.MapFrom(src => src.TaxCSales.Divide()))
                .ForMember(dest => dest.TaxDSales, opt => opt.MapFrom(src => src.TaxDSales.Divide()))
                .ForMember(dest => dest.TaxDAmount, opt => opt.MapFrom(src => src.TaxDAmount.Divide()))
                .ForMember(dest => dest.TaxESales, opt => opt.MapFrom(src => src.TaxESales.Divide()))
                .ForMember(dest => dest.TaxEAmount, opt => opt.MapFrom(src => src.TaxEAmount.Divide()))
                .ForMember(dest => dest.TaxFSales, opt => opt.MapFrom(src => src.TaxFSales.Divide()))
                .ForMember(dest => dest.TaxFAmount, opt => opt.MapFrom(src => src.TaxFAmount.Divide()))
                .ForMember(dest => dest.TaxGSales, opt => opt.MapFrom(src => src.TaxGSales.Divide()))
                .ForMember(dest => dest.TaxGAmount, opt => opt.MapFrom(src => src.TaxGAmount.Divide()))
                .ForMember(dest => dest.TaxHSales, opt => opt.MapFrom(src => src.TaxHSales.Divide()))
                .ForMember(dest => dest.TaxHAmount, opt => opt.MapFrom(src => src.TaxHAmount.Divide()))
                .ForMember(dest => dest.FinalAmount, opt => opt.MapFrom(src => src.FinalAmount.Divide()))
                .ForMember(dest => dest.SubTotalAmount, opt => opt.MapFrom(src => src.SubTotalAmount.Divide()))
            ;

            CreateMap<TransactionRecord09, TenderExchange>()
                .ForMember(dest => dest.TenderAmount, opt => opt.MapFrom(src => src.TenderAmount.Divide().Negate() ?? 0))
                .ForMember(dest => dest.TenderType, opt => opt.MapFrom(src => src.TenderType.getTenderTypeName()))
            ;

            CreateMap<TransactionRecord10, ManagerOverride>();
            CreateMap<TransactionRecord16, PaymentProcessorRequest>()
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Divide()))
                .ForMember(dest => dest.CashBack, opt => opt.MapFrom(src => src.CashBack.Divide()))
                .ForMember(dest => dest.TotalsType, opt => opt.MapFrom(src => src.TotalType16Enum))
                .ForMember(dest => dest.MessageType, opt => opt.MapFrom(src => src.MessageType16Enum))
                .ForMember(dest => dest.EpsFailureReason, opt => opt.MapFrom(src => src.ReasonCode16Enum))
                .ForMember(dest => dest.TenderType, opt => opt.MapFrom(src => src.TenderType16Enum))
            ;
            CreateMap<TransactionRecord11BD, CouponDataEntry>()
                .ForMember(dest => dest.CouponPrice, opt => opt.MapFrom(src => src.CouponPrice.Divide()))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Divide()))
                .ForMember(dest => dest.DepartmentNumber, opt => opt.MapFrom(src => src.DepartmentNumber.ToNullableInt()))
                .ForMember(dest => dest.CouponDepartment, opt => opt.MapFrom(src => src.CouponDepartment.ToNullableInt()))
                .ForMember(dest => dest.CouponDepartment, opt => opt.MapFrom(src => src.CouponDepartment.ToNullableInt()))
            ;
            CreateMap<TransactionRecord11DB, UsedTargetedCoupon>();
            CreateMap<TransactionRecord11DD, CouponDataEntry>()
                .ForMember(dest => dest.CampaignId, opt => opt.MapFrom(src => src.CampaignId.ToNullableString()))
                .ForMember(dest => dest.ManufacturerId, opt => opt.MapFrom(src => src.ManufacturerId.ToNullableString()))
                .ForMember(dest => dest.PromotionCode, opt => opt.MapFrom(src => src.PromotionCode.ToNullableString()))
                .ForMember(dest => dest.AssociatedItemId, opt => opt.MapFrom(src => src.AssociatedItemId.ToNullableString()))
                .ForMember(dest => dest.Unused, opt => opt.MapFrom(src => src.Unused.ToNullableInt()))
                .ForMember(dest => dest.HasCouponRequiredMultipleItemsInOrderToBeIssued, opt => opt.MapFrom(src => src.LogFlags.Bit0.ToNullableBoolean()))
                .ForMember(dest => dest.HasKeyedValueLimitCheck, opt => opt.MapFrom(src => src.LogFlags.Bit1.ToNullableBoolean()))
                .ForMember(dest => dest.HasCouponValueExceedsItemValue, opt => opt.MapFrom(src => src.LogFlags.Bit2.ToNullableBoolean()))
                .ForMember(dest => dest.HasQuantityNotSatisfiedForCoupon, opt => opt.MapFrom(src => src.LogFlags.Bit3.ToNullableBoolean()))
                .ForMember(dest => dest.HasTooManyCouponsRelativeToSales, opt => opt.MapFrom(src => src.LogFlags.Bit4.ToNullableBoolean()))
                .ForMember(dest => dest.HasTooManyLikeCouponsForTransaction, opt => opt.MapFrom(src => src.LogFlags.Bit5.ToNullableBoolean()))
                .ForMember(dest => dest.HasCouponHasExpired, opt => opt.MapFrom(src => src.LogFlags.Bit6.ToNullableBoolean()))
                .ForMember(dest => dest.HasMinimumPurchaseNotSatisfied, opt => opt.MapFrom(src => src.LogFlags.Bit7.ToNullableBoolean()))
                .ForMember(dest => dest.HasCouponGoodForFreeItem, opt => opt.MapFrom(src => src.LogFlags.Bit8.ToNullableBoolean()))
                .ForMember(dest => dest.HasOperatorOverrideRequired, opt => opt.MapFrom(src => src.LogFlags.Bit9.ToNullableBoolean()))
                .ForMember(dest => dest.HasManagerOverrideRequired, opt => opt.MapFrom(src => src.LogFlags.Bit10.ToNullableBoolean()))
                .ForMember(dest => dest.HasCouponDidNotRequireValidation, opt => opt.MapFrom(src => src.LogFlags.Bit11.ToNullableBoolean()))
                .ForMember(dest => dest.HasOnlyFamilySuperGroupOrFamilyGroupIsValid, opt => opt.MapFrom(src => src.LogFlags.Bit12.ToNullableBoolean()))
                .ForMember(dest => dest.HasOnlyManufacturerIsValid, opt => opt.MapFrom(src => src.LogFlags.Bit13.ToNullableBoolean()))
                .ForMember(dest => dest.HasOnlyDepartmentIsValid, opt => opt.MapFrom(src => src.LogFlags.Bit14.ToNullableBoolean()))
                .ForMember(dest => dest.IsNoMatchFound, opt => opt.MapFrom(src => src.LogFlags.Bit15.ToNullableBoolean()))
            ;
            CreateMap<TransactionRecord9904, TenderExchange>()
                .ForMember(dest => dest.SignatureSource, opt => opt.MapFrom(src => src.getSignatureSource()))
                .ForMember(dest => dest.SignatureFormat, opt => opt.MapFrom(src => src.SignatureFormat))
                .ForMember(dest => dest.SignatureName, opt => opt.MapFrom(src => src.SignatureName))
            ;
            CreateMap<TransactionRecord99050, AceItem>()
                .EnsureOnlyOnce()
                .ForMember(dest => dest.RefundReason, opt => opt.MapFrom(src => src.RefundReason.ConvertToRefundReason()))
            ;
            CreateMap<TransactionRecord99096, AceItem>()
                .ForMember(dest => dest.PlanId, opt => opt.MapFrom(src => src.Plan))
                .ForMember(dest => dest.GiftCardNumber, opt => opt.MapFrom(src => src.AccountNumber))
            ;
            CreateMap<TransactionRecord99104, PharmacyItem>()
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Divide()))
                .ForMember(dest => dest.InsuranceAmount, opt => opt.MapFrom(src => src.InsuranceAmount.Divide()))
                .ForMember(dest => dest.NetDue, opt => opt.MapFrom(src => src.NetDue.Divide()))
            ;
            CreateMap<TransactionRecord991011, QualifiedHealthCareProvider>()
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.QhcpSubtotal.Divide()))
                .ForMember(dest => dest.DiscountTotal, opt => opt.MapFrom(src => src.QhcpDiscountTotal.Divide()))
                .ForMember(dest => dest.SalesTax, opt => opt.MapFrom(src => src.QhcpSalesTax.Divide()))
                .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.QhcpTotal.Divide()))
            ;
            CreateMap<TransactionRecord991012, CustomArrangement>()
                .ForMember(dest => dest.Void, opt => opt.MapFrom(src => src.Void.ToNullableBoolean()))
            ;
            CreateMap<TransactionRecord991014, CustomArrangement>();
            CreateMap<TransactionRecord991014, AceCustomArrangementItem>();
            CreateMap<TransactionRecord991026, InstacartQR>();
            CreateMap<TransactionRecord991028, Meals2GoQR>();
            CreateMap<TransactionRecord991029, ShopicQR>();
            CreateMap<TransactionRecord991031, AmazonDashCartQR>(); 
            CreateMap<TransactionRecord991096, ValueCard>();
            CreateMap<TransactionRecord99111, AddItemButtonPressed>()
                .ForMember(dest => dest.MinimumAgeRestriction, opt => opt.MapFrom(src => src.MinimumAgeRestriction.ToNullableInt()))
                .ForMember(dest => dest.CurrentScaleWeight, opt => opt.MapFrom(src => src.CurrentScaleWeight.DivideDouble()))
            ;
            CreateMap<TransactionRecord99112, VoidItemButtonPressed>()
                .ForMember(dest => dest.MinimumAgeRestriction, opt => opt.MapFrom(src => src.MinimumAgeRestriction.ToNullableInt()))
                .ForMember(dest => dest.CurrentScaleWeight, opt => opt.MapFrom(src => src.CurrentScaleWeight.DivideDouble()))
            ;
            CreateMap<TransactionRecord99113, VoidItemButtonPressedDuringException>()
                .ForMember(dest => dest.MinimumAgeRestriction, opt => opt.MapFrom(src => src.MinimumAgeRestriction.ToNullableInt()))
                .ForMember(dest => dest.CurrentScaleWeight, opt => opt.MapFrom(src => src.CurrentScaleWeight.DivideDouble()))
            ;
            CreateMap<TransactionRecord99115, RemoveFromListButtonPressed>()
                .ForMember(dest => dest.MinimumAgeRestriction, opt => opt.MapFrom(src => src.MinimumAgeRestriction.ToNullableInt()))
                .ForMember(dest => dest.CurrentScaleWeight, opt => opt.MapFrom(src => src.CurrentScaleWeight.DivideDouble()))
            ;
            CreateMap<TransactionRecord99116, BypassAuditButtonPressed>()
                .ForMember(dest => dest.MinimumAgeRestriction, opt => opt.MapFrom(src => src.MinimumAgeRestriction.ToNullableInt()))
                .ForMember(dest => dest.CurrentScaleWeight, opt => opt.MapFrom(src => src.CurrentScaleWeight.DivideDouble()))
            ;
            CreateMap<TransactionRecord99117, ItemAddedDuringAudit>();
            CreateMap<TransactionRecord991110, MobileTransactionStarted>();
            CreateMap<TransactionRecord995010, InmarCoupon>();
            CreateMap<TransactionRecord995012, InmarCoupon>()
                .ForMember(dest => dest.InmarCouponShortDescription, opt => opt.MapFrom(src => src.InmarCouponShortDescription))
            ;
        }
    }
}