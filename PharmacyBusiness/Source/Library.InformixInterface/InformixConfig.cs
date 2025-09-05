namespace Library.InformixInterface
{
    public class InformixConfig
    {
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string ConnectionString => $"Host=ncs035.wfm.wegmans.com; Service=1504; Server=ncs035_uccx; Protocol=onsoctcp; User ID={Username}; Password={Password}; Database=db_cra; Pooling=false; client_locale=en_US.UTF8;db_locale=en_US.UTF8;";
    }
}

