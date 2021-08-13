using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    public class GetAddressAllowanceQuery : IRequest<AddressAllowanceDto>
    {
        public GetAddressAllowanceQuery(Address owner, Address spender, Address token)
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
