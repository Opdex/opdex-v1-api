using MediatR;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses
{
    public class RetrieveAddressAllowanceQuery : IRequest<AddressAllowance>
    {
        public RetrieveAddressAllowanceQuery(Address owner, Address spender, Address token)
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
