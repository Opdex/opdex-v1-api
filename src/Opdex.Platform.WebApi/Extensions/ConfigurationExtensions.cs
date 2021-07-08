using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Opdex.Platform.Common.Configurations;

namespace Opdex.Platform.WebApi.Extensions
{
    public static class ConfigurationExtensions
    {
        public static void SetupConfiguration<T>(this IServiceCollection services, IConfiguration configuration)
            where T : class, IValidatable, new()
        {
            services.Configure<T>(configuration);
            services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<T>>().Value);
            services.AddSingleton<IValidatable>(resolver => resolver.GetRequiredService<IOptions<T>>().Value);
        }
    }
}
