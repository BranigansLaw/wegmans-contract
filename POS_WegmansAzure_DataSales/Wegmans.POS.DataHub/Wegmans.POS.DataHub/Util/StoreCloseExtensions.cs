using Wegmans.EnterpriseLibrary.Data.Hubs.POS.StoreClose.v1;

namespace Wegmans.POS.DataHub.Util
{
    public static class StoreCloseExtensions
    {
        public static CloseTypeIndication? ConvertToCloseTypeIndication(this string value)
        => value switch
        {
            "0" or "00" => CloseTypeIndication.CloseFiles,
            "01" => CloseTypeIndication.CloseReportingPeriodLong,
            "02" => CloseTypeIndication.CloseReportingPeriodShort,
            "03" => CloseTypeIndication.CloseDepartmentTotals,
            "99" => CloseTypeIndication.InvalidClose,
            _ => null
        };
    }
}
