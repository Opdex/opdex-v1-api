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
            services.AddTransient<IRequestHandler<PersistPairCommand, bool>, PersistPairCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTokenCommand, long>, PersistTokenCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionCommand, bool>, PersistTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionApprovalEventCommand, Unit>, PersistTransactionApprovalEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionBurnEventCommand, Unit>, PersistTransactionBurnEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionMintEventCommand, Unit>, PersistTransactionMintEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionPairCreatedEventCommand, Unit>, PersistTransactionPairCreatedEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionSwapEventCommand, Unit>, PersistTransactionSwapEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionSyncEventCommand, Unit>, PersistTransactionSyncEventCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionTransferEventCommand, Unit>, PersistTransactionTransferEventCommandHandler>();
            
            return services;
        }
    }
}