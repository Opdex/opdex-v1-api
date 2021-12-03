using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Mempool;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Mempool;

public class CallCirrusGetExistsInMempoolQueryHandler : IRequestHandler<CallCirrusGetExistsInMempoolQuery, bool>
{
    private readonly IMempoolModule _mempoolModule;

    public CallCirrusGetExistsInMempoolQueryHandler(IMempoolModule mempoolModule)
    {
        _mempoolModule = mempoolModule ?? throw new ArgumentNullException(nameof(mempoolModule));
    }

    public async Task<bool> Handle(CallCirrusGetExistsInMempoolQuery request, CancellationToken cancellationToken)
    {
        var mempoolTransactions = await _mempoolModule.GetRawMempoolAsync(cancellationToken);
        return mempoolTransactions.Contains(request.TransactionHash);
    }
}