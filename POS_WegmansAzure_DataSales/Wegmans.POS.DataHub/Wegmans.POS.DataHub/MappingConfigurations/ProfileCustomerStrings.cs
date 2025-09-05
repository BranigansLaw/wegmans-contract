using AutoMapper;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;
using Wegmans.POS.DataHub.ACETransactionModel;

namespace Wegmans.POS.DataHub.MappingConfigurations;

public class ProfileCustomerStrings : Profile
{
    public ProfileCustomerStrings()
    {
        CreateMap<TransactionRecord11EE, CustomerIdentification>()
            .ForMember(dest => dest.LoyaltyNumber, opt => opt.MapFrom(src => src.CustomerAccountID))
            .ForMember(dest => dest.CouponAmount, opt => opt.MapFrom(src => src.CouponAmount))
            .ForMember(dest => dest.CouponCount, opt => opt.MapFrom(src => src.CouponCount))
            .ForMember(dest => dest.EntryMethod, opt => opt.MapFrom(src => src.getEntryMethod()))
            ;

        CreateMap<TransactionRecord11FF, CustomerIdentification>()
            .ForMember(dest => dest.LoyaltyNumber, opt => opt.MapFrom(src => src.LoyaltyNumber))
            ;
    }    
}
