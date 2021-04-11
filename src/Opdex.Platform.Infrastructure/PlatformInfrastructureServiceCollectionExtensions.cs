using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Market;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Transactions;
using Opdex.Platform.Infrastructure.Data.Handlers.Market;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions;

namespace Opdex.Platform.Infrastructure
{
    public static class PlatformInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatformInfrastructureServices(this IServiceCollection services)
        {
            // Data Queries
            services.AddTransient<IRequestHandler<SelectAllLiquidityPoolsQuery, IEnumerable<LiquidityPool>>, SelectAllLiquidityPoolsQueryHandler>();
            services.AddTransient<IRequestHandler<SelectAllTokensQuery, IEnumerable<Token>>, SelectAllTokensQueryHandler>();
            services.AddTransient<IRequestHandler<SelectLatestMarketSnapshotQuery, MarketSnapshot>, SelectLatestMarketSnapshotQueryHandler>();
            services.AddTransient<IRequestHandler<SelectTransactionsByPoolWithFilterQuery, IEnumerable<Transaction>>, SelectTransactionsByPoolWithFilterQueryHandler>();
            
            return services;
        }
    }
}