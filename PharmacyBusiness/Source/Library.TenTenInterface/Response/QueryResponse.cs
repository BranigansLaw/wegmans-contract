using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Library.TenTenInterface.Response
{
    [XmlRoot(ElementName = "out", IsNullable = false)]
    public class QueryResponse : ITenTenQueryResponse
    {
        [XmlElement(ElementName = "rc")]
        public int ResponseCode { get; set; }

        [XmlElement(ElementName = "msg")]
        public string? Message { get; set; }

        [XmlElement(ElementName = "csv")]
        public string? Csv { get; set; }

        [XmlElement(ElementName = "data")]
        public string? Xml { get; set; }
    }
}
