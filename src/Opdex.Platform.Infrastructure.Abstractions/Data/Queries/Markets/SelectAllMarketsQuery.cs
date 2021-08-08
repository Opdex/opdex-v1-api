using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets
{
    public class SelectAllMarketsQuery : IRequest<IEnumerable<Market>>
    {

    }
}