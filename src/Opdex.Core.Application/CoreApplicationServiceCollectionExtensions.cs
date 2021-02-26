using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries;
using Opdex.Core.Application.Handlers;

namespace Opdex.Core.Application
{
    public static class CoreApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ServiceFactory>(serviceProvider => serviceProvider.GetService);
            services.AddScoped(typeof(IMediator), typeof(Mediator));
            
            services.AddTransient<IRequestHandler<RetrieveLatestBlockQuery, BlockDto>, RetrieveLatestBlockQueryHandler>();

            return services;
        }
    }
}