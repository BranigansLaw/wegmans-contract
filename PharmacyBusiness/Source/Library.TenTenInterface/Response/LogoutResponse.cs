using System.Xml.Serialization;

namespace Library.TenTenInterface.Response
{
    [XmlRoot(ElementName = "out", IsNullable = false)]
    public class LogoutResponse : ITenTenResponse
    {
        [XmlElement(ElementName = "rc")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "msg")]
        public string? Message { get; set; }
    }
}
