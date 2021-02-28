using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pairs;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pairs
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
                new Pair("stbdf8n5gxase9ngss0gfaitrexm3j8l234", 1, 892423413, "23324"),
                new Pair("sgfaitrexm3j8l234stbdf8n5gxase9ngs0", 2, 892423413, "234234")
            };

            return Task.FromResult<IEnumerable<Pair>>(list);
        }
    }
}