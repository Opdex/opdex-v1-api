using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.WebApi.Conventions;
using Opdex.Platform.WebApi.Middleware;

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

        /// <summary>
        /// Configures MVC so that actions which include a <see cref="NetworkAttribute" /> that does not match the configured network are hidden.
        /// </summary>
        public static IMvcBuilder AddNetworkActionHidingConvention(this IMvcBuilder builder)
        {
            builder.Services.AddSingleton<NetworkActionHidingConvention>();
            builder.Services.AddSingleton<IConfigureOptions<MvcOptions>, NetworkActionHidingConventionMvcOptions>();
            return builder;
        }
    }
}
