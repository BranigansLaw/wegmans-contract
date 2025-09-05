namespace Library.TenTenInterface.Exceptions
{
    public class TenTenXmlSerializeFailedException : Exception
    {
        public TenTenXmlSerializeFailedException(string failedXml, Type type)
        {
            FailedXml = failedXml;
            Type = type;
        }

        public string FailedXml { get; set; }
        public Type Type { get; }
    }
}
