using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveAllMarketsQuery : IRequest<IEnumerable<Market>>
    {

    }
}