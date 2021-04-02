using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Domain.Models;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents;
using Opdex.Indexer.Infrastructure.Data.Handlers;
using Opdex.Indexer.Infrastructure.Data.Handlers.TransactionEvents;

namespace Opdex.Indexer.Infrastructure
{
    public static class IndexerInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddIndexerInfrastructureServices(this IServiceCollection services)
        {
            // Commands
            services.AddTransient<IRequestHandler<PersistBlockCommand, bool>, PersistBlockCommandHandler>();
            services.AddTransient<IRequestHandler<PersistPoolCommand, long>, PersistPoolCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTokenCommand, long>, PersistTokenCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionCommand, Transaction>, PersistTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionApprovalEventCommand, long>, PersistTransactionApprovalEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionBurnEventCommand, long>, PersistTransactionBurnEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionMintEventCommand, long>, PersistTransactionMintEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionPoolCreatedEventCommand, long>, PersistTransactionPoolCreatedEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionSwapEventCommand, long>, PersistTransactionSwapEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionSyncEventCommand, long>, PersistTransactionSyncEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionTransferEventCommand, long>, PersistTransactionTransferEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionEventSummaryCommand, bool>, PersistTransactionEventSummaryCommandHandler>();
            
            return services;
        }
    }
}