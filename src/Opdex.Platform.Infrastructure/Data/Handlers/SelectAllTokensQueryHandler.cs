using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;

namespace Opdex.Platform.Infrastructure.Data.Handlers
{
    public class SelectAllTokensQueryHandler : IRequestHandler<SelectAllTokensQuery, IEnumerable<Token>>
    {
        public SelectAllTokensQueryHandler()
        {
            
        }
        
        public Task<IEnumerable<Token>> Handle(SelectAllTokensQuery request, CancellationToken cancellationToken)
        {
            var list = new List<Token> 
            {
                new Token("stbdf8n5gxase9ngss0gfaitrexm3j8l234", "MediConnect", "MEDI", 8, 100_000_000, "100_000_000"),
                new Token("sgfaitrexm3j8l234stbdf8n5gxase9ngs0", "Bitcoin (Wrapped)", "WBTC", 8, 100_000_000, "100_000_000")
            };

            return Task.FromResult<IEnumerable<Token>>(list);
        }
    }
}