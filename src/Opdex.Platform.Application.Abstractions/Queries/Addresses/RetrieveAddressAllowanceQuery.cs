using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses
{
    public class RetrieveAddressAllowanceQuery : IRequest<AddressAllowance>
    {
        public RetrieveAddressAllowanceQuery(Address owner, Address spender, Address token)
        {
            Owner = owner;
            Spender = spender;
            Token = token;
        }

        public Address Owner { get; }
        public Address Spender { get; }
        public Address Token { get; }
    }
}
