namespace Library.TenTenInterface.Response
{
    public interface ITenTenQueryResponse : ITenTenResponse
    {
        string? Csv { get; set; }

        string? Xml { get; set; }
    }
}
