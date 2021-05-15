namespace Opdex.Platform.WebApi.Auth
{
    public class AuthConfiguration
    {
        public AuthProvider Opdex { get; set; }

        public class AuthProvider
        {
            public string SigningKey { get; set; }
        }
    }
}