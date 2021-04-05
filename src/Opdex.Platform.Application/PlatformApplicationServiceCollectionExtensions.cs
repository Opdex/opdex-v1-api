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
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Market;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.EntryHandlers.Market;
using Opdex.Platform.Application.EntryHandlers.Pools;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Application.EntryHandlers.Transactions.Wallet;
using Opdex.Platform.Application.Handlers.Market;
using Opdex.Platform.Application.Handlers.Pools;
using Opdex.Platform.Application.Handlers.Tokens;
using Opdex.Platform.Application.Handlers.Transactions;
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
            services.AddTransient<IRequestHandler<GetTransactionsByPoolWithFilterQuery, IEnumerable<TransactionDto>>, GetTransactionsByPoolWithFilterQueryHandler>();

            // Queries
            services.AddTransient<IRequestHandler<RetrieveAllPoolsQuery, IEnumerable<Pool>>, RetrieveAllPoolsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveAllTokensQuery, IEnumerable<Token>>, RetrieveAllTokensQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLatestMarketSnapshotQuery, MarketSnapshot>, RetrieveLatestMarketSnapshotQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionsByPoolWithFilterQuery,  IEnumerable<Transaction>>, RetrieveTransactionsByPoolWithFilterQueryHandler>();

            // Entry Commands
            services.AddTransient<IRequestHandler<CreateWalletSwapTransactionCommand, string>, CreateWalletSwapTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletAddLiquidityTransactionCommand, string>, CreateWalletAddLiquidityTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletRemoveLiquidityTransactionCommand, string>, CreateWalletRemoveLiquidityTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletApproveAllowanceTransactionCommand, string>, CreateWalletApproveAllowanceTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletCreateLiquidityPoolTransactionCommand, string>, CreateWalletCreateLiquidityPoolTransactionCommandHandler>();

            // Commands
            services.AddTransient<IRequestHandler<MakeWalletSwapTransactionCommand, string>, MakeWalletSwapTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletAddLiquidityTransactionCommand, string>, MakeWalletAddLiquidityTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletRemoveLiquidityTransactionCommand, string>, MakeWalletRemoveLiquidityTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletApproveAllowanceTransactionCommand, string>, MakeWalletApproveAllowanceTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletCreateLiquidityPoolTransactionCommand, string>, MakeWalletCreateLiquidityPoolTransactionCommandHandler>();
            
            return services;
        }
    }
}