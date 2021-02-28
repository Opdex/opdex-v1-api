using System.Collections.Generic;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveAllTokensQuery : IRequest<IEnumerable<TokenDto>>
    {
        // Nothing to include for now, eventually will add pagination
        public RetrieveAllTokensQuery()
        {
            
        }
    }
}