using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    public class GetAddressAllowanceQuery : IRequest<AddressAllowanceDto>
    {
        public GetAddressAllowanceQuery(Address owner, Address spender, Address token)
        {
            Owner = owner != Address.Zero ? owner : throw new ArgumentNullException(nameof(owner), "Owner must be provided.");
            Spender = spender != Address.Zero ? spender : throw new ArgumentNullException(nameof(spender), "Spender must be provided.");
            Token = token != Address.Zero ? token : throw new ArgumentNullException(nameof(token), "Token must be provided.");
        }

        public Address Owner { get; }
        public Address Spender { get; }
        public Address Token { get; }
    }
}
