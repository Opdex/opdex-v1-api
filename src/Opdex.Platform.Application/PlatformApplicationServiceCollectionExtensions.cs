using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Domain.Models;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryQueries.Market;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Market;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Market;
using Opdex.Platform.Application.EntryHandlers.Pools;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Application.EntryHandlers.Transactions.Wallet;
using Opdex.Platform.Application.Handlers.Market;
using Opdex.Platform.Application.Handlers.Pools;
using Opdex.Platform.Application.Handlers.Tokens;
using Opdex.Platform.Application.Handlers.Transactions.Wallet;

namespace Opdex.Platform.Application
{
    public static class PlatformApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatformApplicationServices(this IServiceCollection services)
        {
            // Entry Queries
            services.AddTransient<IRequestHandler<GetAllPoolsQuery, IEnumerable<PoolDto>>, GetAllPoolsQueryHandler>();
            services.AddTransient<IRequestHandler<GetAllTokensQuery, IEnumerable<TokenDto>>, GetAllTokensQueryHandler>();
            services.AddTransient<IRequestHandler<GetLatestMarketSnapshotQuery, MarketSnapshotDto>, GetLatestMarketSnapshotQueryHandler>();

            // Queries
            services.AddTransient<IRequestHandler<RetrieveAllPoolsQuery, IEnumerable<Pool>>, RetrieveAllPoolsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveAllTokensQuery, IEnumerable<Token>>, RetrieveAllTokensQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLatestMarketSnapshotQuery, MarketSnapshot>, RetrieveLatestMarketSnapshotQueryHandler>();

            // Entry Commands
            services.AddTransient<IRequestHandler<CreateWalletSwapTransactionCommand, string>, CreateWalletSwapTransactionCommandHandler>();

            // Commands
            services.AddTransient<IRequestHandler<MakeWalletSwapTransactionCommand, string>, MakeWalletSwapTransactionCommandHandler>();
            
            return services;
        }
    }
}