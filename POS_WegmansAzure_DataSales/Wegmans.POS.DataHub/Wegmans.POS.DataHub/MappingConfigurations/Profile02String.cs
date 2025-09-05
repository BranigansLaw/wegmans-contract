using AutoMapper;
using Wegmans.POS.DataHub.Util;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.MappingConfigurations
{
    public class Profile02String : Profile
    {
        public Profile02String()
        {
            CreateMap<TransactionRecord02, AceItem>()
                .EnsureOnlyOnce()
                .ForMember(dest => dest.TareWeight, opt => opt.MapFrom(src => src.UserValue))
                //Indicat1
                .ForMember(dest => dest.HasCouponQuantity, opt => opt.MapFrom(src => src.Indicat1.Bit0.ToNullableBoolean()))
                .ForMember(dest => dest.HasRepeatQuantity, opt => opt.MapFrom(src => src.Indicat1.Bit1.ToNullableBoolean()))
                .ForMember(dest => dest.IsQuantityRequired, opt => opt.MapFrom(src => src.Indicat1.Bit2.ToNullableBoolean()))
                .ForMember(dest => dest.HasScaleWeight, opt => opt.MapFrom(src => src.Indicat1.Bit3.ToNullableBoolean()))
                .ForMember(dest => dest.HasTareKey, opt => opt.MapFrom(src => src.Indicat1.Bit4.ToNullableBoolean()))
                .ForMember(dest => dest.WasQuantityKeyPressed, opt => opt.MapFrom(src => src.Indicat1.Bit5.ToNullableBoolean()))
                .ForMember(dest => dest.WasWeightOrVolumeKey, opt => opt.MapFrom(src => src.Indicat1.Bit6.ToNullableBoolean()))
                .ForMember(dest => dest.WasDealQuantityKeyed, opt => opt.MapFrom(src => src.Indicat1.Bit7.ToNullableBoolean()))

                .ForMember(dest => dest.SalePrice, opt => opt.MapFrom(src => src.SalePrice.Divide()))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Indicat1.Bit5.ToNullableBoolean().getQuantityValue(src)))
                .ForMember(dest => dest.Weight, opt => opt.MapFrom(src => src.Indicat1.Bit6.ToNullableBoolean().getWeightValue(src) ?? src.Indicat1.Bit3.ToNullableBoolean().getWeightValue(src)))
            ;
        }
    }
}