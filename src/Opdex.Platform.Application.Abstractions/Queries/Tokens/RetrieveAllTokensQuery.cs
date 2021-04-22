using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveAllTokensQuery : IRequest<IEnumerable<Token>>
    {
        // Nothing to include for now, eventually will add pagination
        public RetrieveAllTokensQuery()
        {
            
        }
    }
}