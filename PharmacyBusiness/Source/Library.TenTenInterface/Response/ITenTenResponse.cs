namespace Library.TenTenInterface.Response
{
    public  interface ITenTenResponse
    {
        int ResponseCode { get; }

        string? Message { get; set; }
    }
}
