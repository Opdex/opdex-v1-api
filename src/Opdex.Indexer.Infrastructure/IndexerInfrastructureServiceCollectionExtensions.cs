using MediatR;
using Microsoft.Extensions.DependencyInjection;
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
            services.AddTransient<IRequestHandler<PersistPairCommand, long>, PersistPairCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTokenCommand, long>, PersistTokenCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionCommand, long>, PersistTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionApprovalEventCommand, bool>, PersistTransactionApprovalEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionBurnEventCommand, bool>, PersistTransactionBurnEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionMintEventCommand, bool>, PersistTransactionMintEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionPairCreatedEventCommand, bool>, PersistTransactionPairCreatedEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionSwapEventCommand, bool>, PersistTransactionSwapEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionSyncEventCommand, bool>, PersistTransactionSyncEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionTransferEventCommand, bool>, PersistTransactionTransferEventCommandHandler>();
            
            return services;
        }
    }
}