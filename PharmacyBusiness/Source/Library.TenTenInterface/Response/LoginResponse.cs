using System.Xml.Serialization;

namespace Library.TenTenInterface.Response
{
    [XmlRoot(ElementName = "out", IsNullable = false)]
    public class LoginResponse : ITenTenResponse
    {
        [XmlElement(ElementName = "rc")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "sid")]
        // Will be filled out by TenTen XML response
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string SessionId { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [XmlElement(ElementName = "pswd")]
        // Will be filled out by TenTen XML response
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Password { get; set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        [XmlElement(ElementName = "msg")]
        public string? Message { get; set; }

        [XmlElement(ElementName = "version")]
        public string? Version { get; set; }
    }
}
