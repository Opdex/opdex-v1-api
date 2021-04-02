using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Application.Abstractions.EntryQueries.Pairs;
using Opdex.Core.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Blocks;
using Opdex.Core.Application.Abstractions.Queries.Pairs;
using Opdex.Core.Application.Abstractions.Queries.Tokens;
using Opdex.Core.Application.Abstractions.Queries.Transactions;
using Opdex.Core.Application.Abstractions.Queries.Transactions.TransactionEvents;
using Opdex.Core.Application.Assemblers;
using Opdex.Core.Application.EntryHandlers.Pairs;
using Opdex.Core.Application.EntryHandlers.Tokens;
using Opdex.Core.Application.Handlers.Blocks;
using Opdex.Core.Application.Handlers.Pairs;
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
            services.AddTransient<IRequestHandler<GetPairByAddressQuery, PairDto>, GetPairByAddressQueryHandler>();

            // Handlers
            services.AddTransient<IRequestHandler<RetrieveTokenByAddressQuery, Token>, RetrieveTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrievePairByAddressQuery, Pair>, RetrievePairByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionByHashQuery, Transaction>, RetrieveTransactionByHashQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveMintEventsByTransactionIdQuery, IEnumerable<MintEvent>>, RetrieveMintEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveBurnEventsByTransactionIdQuery, IEnumerable<BurnEvent>>, RetrieveBurnEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveSwapEventsByTransactionIdQuery, IEnumerable<SwapEvent>>, RetrieveSwapEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveSyncEventsByTransactionIdQuery, IEnumerable<SyncEvent>>, RetrieveSyncEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveApprovalEventsByTransactionIdQuery, IEnumerable<ApprovalEvent>>, RetrieveApprovalEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransferEventsByTransactionIdQuery, IEnumerable<TransferEvent>>, RetrieveTransferEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrievePairCreatedEventsByTransactionIdQuery, IEnumerable<PairCreatedEvent>>, RetrievePairCreatedEventsByTransactionIdQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveTransactionEventSummariesByTransactionIdQuery, List<TransactionEventSummary>>, RetrieveTransactionEventSummariesByTransactionIdQueryHandler>();

            // Assemblers
            services.AddTransient<IModelAssembler<Transaction, TransactionDto>, TransactionDtoAssembler>();
            
            return services;
        }
    }
}