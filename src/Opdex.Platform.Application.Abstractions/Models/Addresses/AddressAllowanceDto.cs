using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.Addresses
{
    public class AddressAllowanceDto
    {
        public FixedDecimal Allowance { get; set; }

        public Address Spender { get; set; }

        public Address Owner { get; set; }

        public Address Token { get; set; }
    }
}
