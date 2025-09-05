using AutoMapper;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.MappingConfigurations
{
    public class Profile991027String : Profile
    {
        public Profile991027String()
        {
            CreateMap<TransactionRecord991027, AceItem>()
                .EnsureOnlyOnce()
                .ForMember(dest => dest.ItemNumber, opt => opt.MapFrom(src => src.ItemNumber));
        }
        
    }
}