using System.Diagnostics.CodeAnalysis;

namespace Wegmans.RX.Cmm.AzureFunctions.Astute.Models.Outbound
{
    [ExcludeFromCodeCoverage]
    public class CaseText
    {
        public string TextTypeCode { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
    }
}
