using System.Collections.Generic;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.EntryQueries;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.EntryHandlers;
using Opdex.Platform.Application.Handlers;
using Opdex.Platform.Application.Mappers;

namespace Opdex.Platform.Application
{
    public static class PlatformApplicationServiceCollectionExtensions
    {
        public static IServiceCollection AddPlatformApplicationServices(this IServiceCollection services)
        {
            // Entry Queries
            services.AddTransient<IRequestHandler<GetAllPairsQuery, IEnumerable<PairDto>>, GetAllPairsQueryHandler>();
            services.AddTransient<IRequestHandler<GetAllTokensQuery, IEnumerable<TokenDto>>, GetAllTokensQueryHandler>();

            // Queries
            services.AddTransient<IRequestHandler<RetrieveAllPairsQuery, IEnumerable<PairDto>>, RetrieveAllPairsQueryHandler>();
            services.AddTransient<IRequestHandler<RetrieveAllTokensQuery, IEnumerable<TokenDto>>, RetrieveAllTokensQueryHandler>();

            return services;
        }
    }
}