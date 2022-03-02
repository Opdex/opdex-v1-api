using MediatR;
using Opdex.Platform.Application.Abstractions.Cache;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Cache;

public class WrappedTokenTrustValidator : IWrappedTokenTrustValidator
{
    private readonly ISupportedContractsModule _supportedContractsModule;

    private ImmutableHashSet<Address> _trustedTokens;

    public WrappedTokenTrustValidator(ISupportedContractsModule supportedContractsModule)
    {
        _supportedContractsModule = supportedContractsModule ?? throw new ArgumentNullException(nameof(supportedContractsModule));
    }

    public async ValueTask<bool> Validate(Address token, CancellationToken cancellationToken = default)
    {
        if (_trustedTokens is null)
        {
            var supportedTokens = await _supportedContractsModule.GetList(cancellationToken);
            _trustedTokens = supportedTokens.Select(t => t.Src20Address).ToImmutableHashSet();
        }

        return _trustedTokens.Contains(token);
    }
}
