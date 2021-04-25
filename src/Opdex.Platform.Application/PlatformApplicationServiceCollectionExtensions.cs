using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Platform.Application.Abstractions.Commands.Blocks;
using Opdex.Platform.Application.Abstractions.Commands.Pools;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryQueries.Market;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.Market;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Platform.Application.EntryHandlers.Market;
using Opdex.Platform.Application.EntryHandlers.Pools;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Application.EntryHandlers.Transactions.Wallet;
using Opdex.Platform.Application.Handlers;
using Opdex.Platform.Application.Handlers.Blocks;
using Opdex.Platform.Application.Handlers.Market;
using Opdex.Platform.Application.Handlers.Pools;
using Opdex.Platform.Application.Handlers.Tokens;
using Opdex.Platform.Application.Handlers.Transactions;
using Opdex.Platform.Application.Handlers.Transactions.TransactionLogs;
using Opdex.Platform.Application.Handlers.Transactions.Wallet;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using TokenDto = Opdex.Platform.Application.Abstractions.Models.TokenDto;

namespace Opdex.Platform.Application
{
    public static class PlatformApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatformApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ServiceFactory>(serviceProvider => serviceProvider.GetService);
            services.AddScoped(typeof(IMediator), typeof(Mediator));
            
            // Entry Queries
            services.AddTransient<IRequestHandler<GetAllPoolsQuery, IEnumerable<LiquidityPoolDto>>, GetAllPoolsQueryHandler>();
            services.AddTransient<IRequestHandler<GetAllTokensQuery, IEnumerable<TokenDto>>, GetAllTokensQueryHandler>();
            services.AddTransient<IRequestHandler<GetLatestMarketSnapshotQuery, MarketSnapshotDto>, GetLatestMarketSnapshotQueryHandler>();
            services.AddTransient<IRequestHandler<GetTransactionsByPoolWithFilterQuery, IEnumerable<TransactionDto>>, GetTransactionsByPoolWithFilterQueryHandler>();

            // Queries
            services.AddTransient<IRequestHandler<RetrieveAllPoolsQuery, IEnumerable<LiquidityPool>>, RetrieveAllPoolsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveAllTokensQuery, IEnumerable<Token>>, RetrieveAllTokensQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLatestMarketSnapshotQuery, MarketSnapshot>, RetrieveLatestMarketSnapshotQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionsByPoolWithFilterQuery,  IEnumerable<Transaction>>, RetrieveTransactionsByPoolWithFilterQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusCurrentBlockQuery, BlockReceiptDto>, RetrieveCirrusCurrentBlockQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusBlockByHashQuery, BlockReceiptDto>, RetrieveCirrusBlockByHashQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusTransactionByHashQuery, Transaction>, RetrieveCirrusTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCmcStraxPriceQuery, decimal>, RetrieveCmcStraxPriceQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveActiveTokenSnapshotsByTokenIdQuery, IEnumerable<TokenSnapshot>>, RetrieveActiveTokenSnapshotsByTokenIdQueryHandler>();

            // Entry Commands
            services.AddTransient<IRequestHandler<CreateWalletSwapTransactionCommand, string>, CreateWalletSwapTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletAddLiquidityTransactionCommand, string>, CreateWalletAddLiquidityTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletRemoveLiquidityTransactionCommand, string>, CreateWalletRemoveLiquidityTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletApproveAllowanceTransactionCommand, string>, CreateWalletApproveAllowanceTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateWalletCreateLiquidityPoolTransactionCommand, string>, CreateWalletCreateLiquidityPoolTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateTransactionCommand, bool>, CreateTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<CreateCrsTokenSnapshotsCommand, Unit>, CreateCrsTokenSnapshotsCommandHandler>();

            // Commands
            services.AddTransient<IRequestHandler<MakeWalletSwapTransactionCommand, string>, MakeWalletSwapTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletAddLiquidityTransactionCommand, string>, MakeWalletAddLiquidityTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletRemoveLiquidityTransactionCommand, string>, MakeWalletRemoveLiquidityTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletApproveAllowanceTransactionCommand, string>, MakeWalletApproveAllowanceTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeWalletCreateLiquidityPoolTransactionCommand, string>, MakeWalletCreateLiquidityPoolTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeBlockCommand, bool>, MakeBlockCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTokenCommand, long>, MakeTokenCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTransactionCommand, bool>, MakeTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakeLiquidityPoolCommand, long>, MakeLiquidityPoolCommandHandler>();
            services.AddTransient<IRequestHandler<MakeMiningPoolCommand, long>, MakeMiningPoolCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTokenSnapshotCommand, bool>, MakeTokenSnapshotCommandHandler>();
            
            // Entry Handlers
            services.AddTransient<IRequestHandler<RetrieveLatestBlockQuery, BlockDto>, RetrieveLatestBlockQueryHandler>();
            services.AddTransient<IRequestHandler<GetTokenByAddressQuery, TokenDto>, GetTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<GetLiquidityPoolByAddressQuery, LiquidityPoolDto>, GetLiquidityPoolByAddressQueryHandler>();
            
            // Handlers
            services.AddTransient<IRequestHandler<RetrieveTokenByAddressQuery, Token>, RetrieveTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolByAddressQuery, LiquidityPool>, RetrieveLiquidityPoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionByHashQuery, Transaction>, RetrieveTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionLogsByTransactionIdQuery, IEnumerable<TransactionLog>>, RetrieveTransactionLogsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTokenByIdQuery, Token>, RetrieveTokenByIdQueryHandler>();
            
            // Assemblers
            services.AddTransient<IModelAssembler<Transaction, TransactionDto>, TransactionDtoAssembler>();
            services.AddTransient<IModelAssembler<LiquidityPool, LiquidityPoolDto>, LiquidityPoolDtoAssembler>();
            
            return services;
        }
    }
}