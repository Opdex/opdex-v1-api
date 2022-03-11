using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens;

public class CallCirrusTrustedWrappedTokenQueryHandler : IRequestHandler<CallCirrusTrustedWrappedTokenQuery, bool>
{
    private readonly ISupportedContractsModule _supportedContractsModule;

    public CallCirrusTrustedWrappedTokenQueryHandler(ISupportedContractsModule supportedContractsModule)
    {
        _supportedContractsModule = supportedContractsModule ?? throw new ArgumentNullException(nameof(supportedContractsModule));
    }

    public async Task<bool> Handle(CallCirrusTrustedWrappedTokenQuery request, CancellationToken cancellationToken)
    {
        var supportedTokens = await _supportedContractsModule.GetList(cancellationToken);
        return supportedTokens.Any(t => t.Src20Address == request.Token);
    }
}
