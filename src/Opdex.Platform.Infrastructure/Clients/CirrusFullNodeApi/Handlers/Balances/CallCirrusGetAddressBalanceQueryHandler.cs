using MediatR;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Balances
{
    public class CallCirrusGetAddressBalanceQueryHandler : IRequestHandler<CallCirrusGetAddressBalanceQuery, ulong>
    {
        private readonly IBlockStoreModule _blockStoreModule;

        public CallCirrusGetAddressBalanceQueryHandler(IBlockStoreModule blockStoreModule)
        {
            _blockStoreModule = blockStoreModule ?? throw new ArgumentNullException(nameof(blockStoreModule));
        }

        public async Task<ulong> Handle(CallCirrusGetAddressBalanceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _blockStoreModule.GetWalletAddressesBalances(new[] { request.Address }, cancellationToken);
                return response.Balances[0].Balance;
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
