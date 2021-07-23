using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using System;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses
{
    public class RetrieveAddressAllowanceQuery : IRequest<AddressAllowance>
    {
        public RetrieveAddressAllowanceQuery(string owner, string spender, string token)
        {
            Owner = owner.HasValue() ? owner : throw new ArgumentNullException(nameof(owner), "Owner must be provided.");
            Spender = spender.HasValue() ? spender : throw new ArgumentNullException(nameof(spender), "Spender must be provided.");
            Token = token.HasValue() ? token : throw new ArgumentNullException(nameof(token), "Token must be provided.");
        }

        public string Owner { get; }
        public string Spender { get; }
        public string Token { get; }
    }
}
