using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Application.Abstractions.EntryQueries.Pools;
using Opdex.Core.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Blocks;
using Opdex.Core.Application.Abstractions.Queries.Pools;
using Opdex.Core.Application.Abstractions.Queries.Tokens;
using Opdex.Core.Application.Abstractions.Queries.Transactions;
using Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionLogs;
using Opdex.Core.Application.Assemblers;
using Opdex.Core.Application.EntryHandlers.Pools;
using Opdex.Core.Application.EntryHandlers.Tokens;
using Opdex.Core.Application.Handlers.Blocks;
using Opdex.Core.Application.Handlers.Pools;
using Opdex.Core.Application.Handlers.Tokens;
using Opdex.Core.Application.Handlers.Transactions;
using Opdex.Core.Application.Handlers.Transactions.TransactionLogs;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Core.Application
{
    public static class CoreApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ServiceFactory>(serviceProvider => serviceProvider.GetService);
            services.AddScoped(typeof(IMediator), typeof(Mediator));
            
            // Entry Handlers
            services.AddTransient<IRequestHandler<RetrieveLatestBlockQuery, BlockDto>, RetrieveLatestBlockQueryHandler>();
            services.AddTransient<IRequestHandler<GetTokenByAddressQuery, TokenDto>, GetTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<GetPoolByAddressQuery, PoolDto>, GetPoolByAddressQueryHandler>();

            // Handlers
            services.AddTransient<IRequestHandler<RetrieveTokenByAddressQuery, Token>, RetrieveTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrievePoolByAddressQuery, Pool>, RetrievePoolByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionByHashQuery, Transaction>, RetrieveTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMintLogsByIdsQuery, IEnumerable<MintLog>>, RetrieveMintLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveBurnLogsByIdsQuery, IEnumerable<BurnLog>>, RetrieveBurnLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveSwapLogsByIdsQuery, IEnumerable<SwapLog>>, RetrieveSwapLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveReservesLogsByIdsQuery, IEnumerable<ReservesLog>>, RetrieveReservesLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveApprovalLogsByIdsQuery, IEnumerable<ApprovalLog>>, RetrieveApprovalLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransferLogsByIdsQuery, IEnumerable<TransferLog>>, RetrieveTransferLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveLiquidityPoolCreatedLogsByIdsQuery, IEnumerable<LiquidityPoolCreatedLog>>, RetrieveLiquidityPoolCreatedLogsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionLogSummariesByTransactionIdQuery, List<TransactionLogSummary>>, RetrieveTransactionLogSummariesByTransactionIdQueryHandler>();

            // Assemblers
            services.AddTransient<IModelAssembler<Transaction, TransactionDto>, TransactionDtoAssembler>();
            
            return services;
        }
    }
}