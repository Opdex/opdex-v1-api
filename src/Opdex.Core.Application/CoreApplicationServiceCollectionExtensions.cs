using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Application.Abstractions.Queries;
using Opdex.Core.Application.Handlers;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Application
{
    public static class CoreApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ServiceFactory>(serviceProvider => serviceProvider.GetService);
            services.AddScoped(typeof(IMediator), typeof(Mediator));
            
            services.AddTransient<IRequestHandler<RetrieveLatestBlockQuery, Block>, RetrieveLatestBlockQueryHandler>();

            return services;
        }
    }
}