using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Models.Addresses;

public class AddressBalancesDto
{
    public IEnumerable<AddressBalanceDto> Balances { get; set; }
    public CursorDto Cursor { get; set; }
}