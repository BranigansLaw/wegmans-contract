using Snowflake.Data.Client;

namespace Library.SnowflakeInterface
{
    public class SnowflakeConfig
    {
        /// <summary>
        /// Your full account name might include additional segments that identify the region and cloud platform where your account is hosted
        /// </summary>
        public string Account { get; set; } = null!;

        /// <summary>
        /// The Snowflake user you are authenticating as
        /// </summary>
        public string User { get; set; } = null!;

        /// <summary>
        /// The user role you will be authenticating as
        /// </summary>
        public string Role { get; set; } = null!;

        /// <summary>
        /// The database warehouse this connection will be using
        /// </summary>
        public string Warehouse { get; set; } = null!;

        /// <summary>
        /// The data warehouse database name
        /// </summary>
        public string SnowflakeDWDatabase { get; set; } = null!;

        /// <summary>
        /// The audit database name
        /// </summary>
        public string SnowflakeAUDDatabase { get; set; } = null!;

        /// <summary>
        /// The login scopes you'll be using
        /// </summary>
        public IEnumerable<string> Scopes { get; set; } = [];

        /// <summary>
        /// The users RSA private key to use for key-pair authentication.
        /// NOTE: If the private key value includes any equal signs(=), make sure to replace each equal sign with two signs(==) to ensure that the connection string is parsed correctly.
        /// </summary>
        public string UnencryptedPublicUserRsaToken { get; set; } = null!;

        /// <summary>
        /// The Snowflake connection string
        /// </summary>
        public string GetConnectionString()
        {
            SnowflakeDbConnectionStringBuilder connectionBuilder = new()
            {
                { "account", Account },
                { "authenticator", "snowflake_jwt" },
                { "user", User },
                { "private_key", UnencryptedPublicUserRsaToken }
            };

            return connectionBuilder.ConnectionString;
        }
    }
}
