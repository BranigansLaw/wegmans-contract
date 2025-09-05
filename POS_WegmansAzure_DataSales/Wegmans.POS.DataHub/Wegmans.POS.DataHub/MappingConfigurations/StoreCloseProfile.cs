using AutoMapper;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.StoreClose.v1;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.Util;

namespace Wegmans.POS.DataHub.MappingConfigurations
{
    public class StoreCloseProfile : Profile
    {
        public StoreCloseProfile()
        {
            CreateMap<TransactionRecord21, StoreClose>()
                .ForMember(dest => dest.CustomerAccountFileClosed, opt => opt.MapFrom(src => src.Indicat2.Bit0.ToNullableBoolean()))
                .ForMember(dest => dest.ExceptionLogFileClosed, opt => opt.MapFrom(src => src.Indicat2.Bit1.ToNullableBoolean()))
                .ForMember(dest => dest.TerminalProductivityFileClosed, opt => opt.MapFrom(src => src.Indicat2.Bit2.ToNullableBoolean()))
                .ForMember(dest => dest.ItemMovementShortFileClosed, opt => opt.MapFrom(src => src.Indicat2.Bit3.ToNullableBoolean()))
                .ForMember(dest => dest.TenderListingFileClosed, opt => opt.MapFrom(src => src.Indicat2.Bit4.ToNullableBoolean()))
                .ForMember(dest => dest.ItemMovementLongFileClosed, opt => opt.MapFrom(src => src.Indicat2.Bit5.ToNullableBoolean()))
                .ForMember(dest => dest.CloseTypeIndication, opt => opt.MapFrom(src => src.CloseType.ConvertToCloseTypeIndication()))
                .ForMember(dest => dest.CloseProcedure, opt => opt.MapFrom(src => src.CloseProcedure.ToNullableInt()))
            ;
        }
    }
}
