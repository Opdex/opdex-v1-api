using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Addresses;

public class AddressBalanceDto
{
    public FixedDecimal Balance { get; set; }

    public Address Address { get; set; }

    public Address Token { get; set; }
}