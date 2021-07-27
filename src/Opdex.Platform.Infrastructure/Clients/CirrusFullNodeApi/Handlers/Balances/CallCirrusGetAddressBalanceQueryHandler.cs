using MediatR;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Balances
{
    public class CallCirrusGetAddressBalanceQueryHandler : IRequestHandler<CallCirrusGetAddressBalanceQuery, ulong>
    {
        private readonly NetworkType _network;
        private readonly ISmartContractsModule _smartContractsModule;

        public CallCirrusGetAddressBalanceQueryHandler(OpdexConfiguration opdexConfiguration, ISmartContractsModule smartContractsModule)
        {
            _network = opdexConfiguration?.Network ?? throw new ArgumentNullException(nameof(opdexConfiguration));
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        }

        public async Task<ulong> Handle(CallCirrusGetAddressBalanceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                return _network == NetworkType.DEVNET
                    ? await _smartContractsModule.GetWalletAddressCrsBalance(request.Address, cancellationToken)
                    : 0ul;
            }
            catch (Exception)
            {
                // Already logged previous in ApiClientBase
                if (request.FindOrThrow)
                {
                    throw;
                }

                return 0;
            }
        }
    }
}
