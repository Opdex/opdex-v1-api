using Opdex.Platform.Common.Configurations;

namespace Opdex.Platform.WebApi.Auth
{
    public class AuthConfiguration : IValidatable
    {
        public AuthProvider Opdex { get; set; }

        public class AuthProvider
        {
            public string SigningKey { get; set; }
        }

        public void Validate()
        {
            // Todo: Implement
            return;
        }
    }
}
