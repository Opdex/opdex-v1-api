using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances
{
    public class CallCirrusGetAddressBalanceQuery : IRequest<ulong>
    {
        public CallCirrusGetAddressBalanceQuery(string address)
        {
            Address = address.HasValue()
                ? address
                : throw new ArgumentNullException(nameof(address), "A wallet address must be provided.");
        }

        public string Address { get; }
    }
}
