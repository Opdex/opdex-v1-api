using System;
using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveStakingTokenContractSummaryByAddressQuery : IRequest<StakingTokenContractSummary>
    {
        public RetrieveStakingTokenContractSummaryByAddressQuery(Address address)
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
