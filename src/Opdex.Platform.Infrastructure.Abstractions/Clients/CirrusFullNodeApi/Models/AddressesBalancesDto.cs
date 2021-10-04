using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class AddressesBalancesDto
    {
        public IReadOnlyList<AddressBalanceItemDto> Balances { get; set; }
    }

    public class AddressBalanceItemDto
    {
        public ulong Balance { get; set; }
    }
}
