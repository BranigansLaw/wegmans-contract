using AutoMapper;
using Wegmans.POS.DataHub.ACETransactionModel;
using Wegmans.POS.DataHub.JSONOutputModel.Transaction.v1;
using Wegmans.EnterpriseLibrary.Data.Hubs.POS.Transaction.v1;

namespace Wegmans.POS.DataHub.MappingConfigurations
{
    public class Profile995013String : Profile
    {
        public Profile995013String()
        {
            CreateMap<TransactionRecord995013, Transaction>()
                .ForMember(dest => dest.InmarStatus, opt => opt.MapFrom(src => src.getQueryResultMeaning()))
                ;
        }
    }
}
