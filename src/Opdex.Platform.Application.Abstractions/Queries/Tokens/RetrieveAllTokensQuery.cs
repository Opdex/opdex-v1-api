using System.Collections.Generic;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Domain.Models;

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