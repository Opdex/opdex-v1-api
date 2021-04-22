using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens
{
    public class SelectAllTokensQuery : IRequest<IEnumerable<Token>>
    {
        
    }
}