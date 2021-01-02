using System.Collections.Generic;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries
{
    public class SelectAllTokensQuery : IRequest<IEnumerable<Token>>
    {
        
    }
}