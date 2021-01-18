using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;
using Opdex.Indexer.Infrastructure.Data.Handlers;

namespace Opdex.Indexer.Infrastructure
{
    public static class IndexerInfrastructureServiceCollectionExtensions
    {
        public static IServiceCollection AddIndexerInfrastructureServices(this IServiceCollection services)
        {
            services.AddTransient<IRequestHandler<PersistBlockCommand, bool>, PersistBlockCommandHandler>();
            services.AddTransient<IRequestHandler<PersistPairCommand, bool>, PersistPairCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTokenCommand, bool>, PersistTokenCommandHandler>();
            services.AddTransient<IRequestHandler<PersistTransactionCommand, bool>, PersistTransactionCommandHandler>();
            
            return services;
        }
    }
}