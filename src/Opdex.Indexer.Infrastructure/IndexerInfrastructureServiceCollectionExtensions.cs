using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Domain.Models;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs;
using Opdex.Indexer.Infrastructure.Data.Handlers;
using Opdex.Indexer.Infrastructure.Data.Handlers.TransactionLogs;

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
            services.AddTransient<IRequestHandler<PersistTransactionApprovalLogCommand, long>, PersistTransactionApprovalLogCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionBurnLogCommand, long>, PersistTransactionBurnLogCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionMintLogCommand, long>, PersistTransactionMintLogCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionLiquidityPoolCreatedLogCommand, long>, PersistTransactionLiquidityPoolCreatedLogCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionSwapLogCommand, long>, PersistTransactionSwapLogCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionReservesLogCommand, long>, PersistTransactionReservesLogCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionTransferLogCommand, long>, PersistTransactionTransferLogCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionLogSummaryCommand, bool>, PersistTransactionLogSummaryCommandHandler>();
            
            return services;
        }
    }
}