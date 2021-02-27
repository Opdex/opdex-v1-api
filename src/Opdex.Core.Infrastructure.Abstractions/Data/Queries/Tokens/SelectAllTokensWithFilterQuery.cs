using System.Collections.Generic;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Queries.Tokens
{
    public class SelectAllTokensWithFilterQuery : IRequest<IEnumerable<Token>>
    {
        public SelectAllTokensWithFilterQuery()
        {
            
        }
    }
}