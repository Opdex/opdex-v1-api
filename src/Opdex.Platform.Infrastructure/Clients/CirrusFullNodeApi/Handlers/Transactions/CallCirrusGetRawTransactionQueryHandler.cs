using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Transactions;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Transactions;

public class CallCirrusGetRawTransactionQueryHandler : IRequestHandler<CallCirrusGetRawTransactionQuery, RawTransactionDto>
{
    private readonly INodeModule _nodeModule;

    public CallCirrusGetRawTransactionQueryHandler(INodeModule nodeModule)
    {
        _nodeModule = nodeModule;
    }

    public async Task<RawTransactionDto> Handle(CallCirrusGetRawTransactionQuery request, CancellationToken cancellationToken)
    {
        return await _nodeModule.GetRawTransactionAsync(request.TransactionHash, cancellationToken);
    }
}
