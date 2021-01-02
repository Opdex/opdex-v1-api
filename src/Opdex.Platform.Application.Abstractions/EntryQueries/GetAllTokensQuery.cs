using System.Collections.Generic;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries
{
    public class GetAllTokensQuery : IRequest<IEnumerable<TokenDto>>
    {
        // Nothing to include for now, eventually with change name to "WithPagination" 
        // and take pagination related properties.
        public GetAllTokensQuery()
        {
            
        }
    }
}