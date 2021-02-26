using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Domain.Models.Transaction;
using Opdex.Core.Domain.Models.Transaction.TransactionEvents;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Indexer.Application.Abstractions.Commands;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus;
using Opdex.Indexer.Application.Abstractions.Queries.Cirrus.Events;
using Opdex.Indexer.Application.Handlers;
using Opdex.Indexer.Application.Handlers.Cirrus;
using Opdex.Indexer.Application.Handlers.Cirrus.Events;

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
            services.AddTransient<IRequestHandler<RetrieveCirrusPairEventsQuery, IEnumerable<PairCreatedEvent>>, RetrieveCirrusPairEventsQueryHandler>();
            
            // Commands
            services.AddTransient<IRequestHandler<MakeBlockCommand, bool>, MakeBlockCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTokenCommand, long>, MakeTokenCommandHandler>();
            services.AddTransient<IRequestHandler<MakeTransactionCommand, bool>, MakeTransactionCommandHandler>();
            services.AddTransient<IRequestHandler<MakePairCommand, bool>, MakePairCommandHandler>();

            return services;
        }
    }
}