using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;

namespace Opdex.Platform.Infrastructure.Data.Handlers
{
    public class SelectAllPairsQueryHandler : IRequestHandler<SelectAllPairsQuery, IEnumerable<Pair>>
    {
        public SelectAllPairsQueryHandler()
        {
            
        }
        
        public Task<IEnumerable<Pair>> Handle(SelectAllPairsQuery request, CancellationToken cancellationToken)
        {
            var list = new List<Pair> 
            {
                new Pair("stbdf8n5gxase9ngss0gfaitrexm3j8l234", 1, 8924.23413m, 10842.584923m),
                new Pair("sgfaitrexm3j8l234stbdf8n5gxase9ngs0", 2, 8924.23413m, 10842.584923m)
            };

            return Task.FromResult<IEnumerable<Pair>>(list);
        }
    }
}