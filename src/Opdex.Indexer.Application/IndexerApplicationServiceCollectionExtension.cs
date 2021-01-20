using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Indexer.Application.Abstractions.Commands;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus;
using Opdex.Indexer.Application.Handlers;
using Opdex.Indexer.Application.Handlers.Cirrus;
using Opdex.Indexer.Domain.Models;

namespace Opdex.Indexer.Application
{
    public static class IndexerApplicationServiceCollectionExtension
    {
        public static IServiceCollection AddIndexerApplicationServices(this IServiceCollection services)
        {
            // Queries
            services.AddTransient<IRequestHandler<RetrieveCirrusCurrentBlockQuery, BlockReceiptDto>, RetrieveCirrusCurrentBlockQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusBlockByHashQuery, BlockReceiptDto>, RetrieveCirrusBlockByHashQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusTransactionByHashQuery, TransactionReceipt>, RetrieveCirrusTransactionByHashQueryHandler>();
            
            // Commands
            services.AddTransient<IRequestHandler<MakeBlockCommand, bool>, MakeBlockCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTokenCommand, bool>, MakeTokenCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTransactionCommand, bool>, MakeTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakePairCommand, bool>, MakePairCommandHandler>();

            return services;
        }
    }
}