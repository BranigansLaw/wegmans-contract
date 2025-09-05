using Library.DataFileInterface.VendorFileDataModels;
using Library.TenTenInterface.XmlTemplateHandlers.Implementations;

namespace INN.JobRunner.Commands.CommandHelpers.TenTenImportIvrOutboundCallsFromOmnicellFileHelper
{
    public class MapperImp : IMapper
    {
        /// <inheritdoc />
        public IEnumerable<IvrOutboundCalls> MapToTenTenIvrOutboundCalls(IEnumerable<IvrOutboundCallsRow> ivrOutboundCallsRows)
        {
            return ivrOutboundCallsRows.Select(r => new IvrOutboundCalls
            {
                PhoneNbr = r.PhoneNbr,
                CallMadeDate = r.CallMadeDate?.ToString("yyyy-MM-dd"),
                CallMadeTime = r.CallMadeTime,
                CallStatus = r.CallStatus,
                AnsweredBy = r.AnsweredBy,
                RxNum = r.RxNum,
                NbrAttempts = r.NbrAttempts,
                StoreNum = r.StoreNum,
                CallType = r.CallType
            });
        }
    }
}
