using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.MiningPools;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.MiningPools
{
    public class CallCirrusGetOpdexMiningPoolByAddressQuery : IRequest<MiningPoolSmartContractSummary>
    {
        public CallCirrusGetOpdexMiningPoolByAddressQuery(string address)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }

        public string Address { get; }
    }
}
