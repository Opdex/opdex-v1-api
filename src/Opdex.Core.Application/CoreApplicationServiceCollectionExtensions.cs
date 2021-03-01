using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Application.Abstractions.EntryQueries.Pairs;
using Opdex.Core.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Application.Abstractions.Queries.Blocks;
using Opdex.Core.Application.Abstractions.Queries.Pairs;
using Opdex.Core.Application.Abstractions.Queries.Tokens;
using Opdex.Core.Application.Assemblers;
using Opdex.Core.Application.EntryHandlers.Pairs;
using Opdex.Core.Application.EntryHandlers.Tokens;
using Opdex.Core.Application.Handlers.Blocks;
using Opdex.Core.Application.Handlers.Pairs;
using Opdex.Core.Application.Handlers.Tokens;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Application
{
    public static class CoreApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<ServiceFactory>(serviceProvider => serviceProvider.GetService);
            services.AddScoped(typeof(IMediator), typeof(Mediator));
            
            // Handlers
            services.AddTransient<IRequestHandler<RetrieveLatestBlockQuery, BlockDto>, RetrieveLatestBlockQueryHandler>();
            services.AddTransient<IRequestHandler<GetTokenByAddressQuery, TokenDto>, GetTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<GetPairByAddressQuery, PairDto>, GetPairByAddressQueryHandler>();

            // Entry Handlers
            services.AddTransient<IRequestHandler<RetrieveTokenByAddressQuery, Token>, RetrieveTokenByAddressQueryHandler>();
            services.AddTransient<IRequestHandler<RetrievePairByAddressQuery, Pair>, RetrievePairByAddressQueryHandler>();

            // Assemblers
            services.AddTransient<IModelAssembler<Transaction, TransactionDto>, TransactionDtoAssembler>();
            
            return services;
        }
    }
}