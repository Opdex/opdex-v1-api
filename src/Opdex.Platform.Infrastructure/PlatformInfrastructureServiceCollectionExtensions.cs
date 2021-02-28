using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pairs;
using Opdex.Platform.Infrastructure.Data.Handlers.Pairs;

namespace Opdex.Platform.Infrastructure
{
    public static class PlatformInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatformInfrastructureServices(this IServiceCollection services)
        {
            // Data Queries
            services.AddTransient<IRequestHandler<SelectAllPairsQuery, IEnumerable<Pair>>, SelectAllPairsQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAllTokensQuery, IEnumerable<Token>>, SelectAllTokensQueryHandler>();
            
            return services;
        }
    }
}