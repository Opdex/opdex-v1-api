using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Opdex.Core.Application
{
    public static class CoreApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ServiceFactory>(serviceProvider => serviceProvider.GetService);
            
            services.AddScoped(typeof(IMediator), typeof(Mediator));
            
            return services;
        }
    }
}