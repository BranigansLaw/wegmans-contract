namespace Library.TenTenInterface.TenTenApiCallWrapper
{
    public class SessionCredentials
    {
        public SessionCredentials(string sessionId, string username, string password)
        {
            if (string.IsNullOrEmpty(sessionId))
            {
                throw new ArgumentException($"'{nameof(sessionId)}' cannot be null or empty.", nameof(sessionId));
            }

            if (string.IsNullOrEmpty(username))
            {
                throw new ArgumentException($"'{nameof(username)}' cannot be null or empty.", nameof(username));
            }

            if (string.IsNullOrEmpty(password))
            {
                throw new ArgumentException($"'{nameof(password)}' cannot be null or empty.", nameof(password));
            }

            SessionId = sessionId;
            Username = username;
            Password = password;
        }

        public readonly string SessionId;

        public readonly string Username;

        public readonly string Password;
    }
}
