using Azure.Core;
using Microsoft.Extensions.Options;
using Snowflake.Data.Client;

namespace Library.SnowflakeInterface.SnowflakeDbConnectionFactory
{
    public class SnowflakeDbConnectionFactoryImp : ISnowflakeDbConnectionFactory
    {
        private readonly TokenCredential _credential;
        private readonly IOptions<SnowflakeConfig> _options;

        public SnowflakeDbConnectionFactoryImp(
            TokenCredential credential,
            IOptions<SnowflakeConfig> options
        )
        {
            _credential = credential ?? throw new ArgumentNullException(nameof(credential));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        /// <inheritdoc />
        public async Task<SnowflakeDbConnection> CreateAsync(CancellationToken cancellationToken)
        {
            SnowflakeDbConnectionStringBuilder connectionString = new();

            connectionString["account"] = _options.Value.Account;
            connectionString["user"] = _options.Value.User;
            connectionString["role"] = _options.Value.Role;
            connectionString["warehouse"] = _options.Value.Warehouse;
            connectionString["authenticator"] = "oauth";

            TokenRequestContext context = new(_options.Value.Scopes.ToArray());
            AccessToken accessToken = await _credential.GetTokenAsync(context, cancellationToken).ConfigureAwait(false);
            connectionString["token"] = accessToken.Token;

            return new SnowflakeDbConnection(connectionString.ConnectionString);
        }
    }
}
