using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.MiningPools;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningPools
{
    public class CallCirrusGetOpdexMiningPoolByAddressQuery : IRequest<MiningPoolSmartContractSummary>
    {
        public CallCirrusGetOpdexMiningPoolByAddressQuery(Address address)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }

        public Address Address { get; }
    }
}
