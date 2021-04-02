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
using Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionEvents;
using Opdex.Core.Application.Assemblers;
using Opdex.Core.Application.EntryHandlers.Pools;
using Opdex.Core.Application.EntryHandlers.Tokens;
using Opdex.Core.Application.Handlers.Blocks;
using Opdex.Core.Application.Handlers.Pools;
using Opdex.Core.Application.Handlers.Tokens;
using Opdex.Core.Application.Handlers.Transactions;
using Opdex.Core.Application.Handlers.Transactions.TransactionEvents;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionEvents;

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
            services.AddTransient<IRequestHandler<RetrieveMintEventsByIdsQuery, IEnumerable<MintEvent>>, RetrieveMintEventsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveBurnEventsByIdsQuery, IEnumerable<BurnEvent>>, RetrieveBurnEventsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveSwapEventsByIdsQuery, IEnumerable<SwapEvent>>, RetrieveSwapEventsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveSyncEventsByIdsQuery, IEnumerable<SyncEvent>>, RetrieveSyncEventsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveApprovalEventsByIdsQuery, IEnumerable<ApprovalEvent>>, RetrieveApprovalEventsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransferEventsByIdsQuery, IEnumerable<TransferEvent>>, RetrieveTransferEventsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrievePoolCreatedEventsByIdsQuery, IEnumerable<PoolCreatedEvent>>, RetrievePoolCreatedEventsByIdsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionEventSummariesByTransactionIdQuery, List<TransactionEventSummary>>, RetrieveTransactionEventSummariesByTransactionIdQueryHandler>();

            // Assemblers
            services.AddTransient<IModelAssembler<Transaction, TransactionDto>, TransactionDtoAssembler>();
            
            return services;
        }
    }
}