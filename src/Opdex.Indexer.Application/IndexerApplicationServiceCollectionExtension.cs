using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Indexer.Application.Abstractions.Commands.Blocks;
using Opdex.Indexer.Application.Abstractions.Commands.Transactions;
using Opdex.Indexer.Application.Abstractions.Commands.Pairs;
using Opdex.Indexer.Application.Abstractions.Commands.Tokens;
using Opdex.Indexer.Application.Abstractions.Queries.Transactions;
using Opdex.Indexer.Application.Abstractions.Queries.Blocks;
using Opdex.Indexer.Application.Handlers.Blocks;
using Opdex.Indexer.Application.Handlers.Pairs;
using Opdex.Indexer.Application.Handlers.Tokens;
using Opdex.Indexer.Application.Handlers.Transactions;

namespace Opdex.Indexer.Application
{
    public static class IndexerApplicationServiceCollectionExtension
    {
        public static IServiceCollection AddIndexerApplicationServices(this IServiceCollection services)
        {
            // Queries
            services.AddTransient<IRequestHandler<RetrieveCirrusCurrentBlockQuery, BlockReceiptDto>, RetrieveCirrusCurrentBlockQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusBlockByHashQuery, BlockReceiptDto>, RetrieveCirrusBlockByHashQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveCirrusTransactionByHashQuery, Transaction>, RetrieveCirrusTransactionByHashQueryHandler>();
            
            // Commands
            services.AddTransient<IRequestHandler<MakeBlockCommand, bool>, MakeBlockCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTokenCommand, long>, MakeTokenCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTransactionCommand, bool>, MakeTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakePairCommand, bool>, MakePairCommandHandler>();

            return services;
        }
    }
}