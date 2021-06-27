using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    public class GetAddressAllowanceForTokenQuery : IRequest<AddressAllowanceDto>
    {
        public GetAddressAllowanceForTokenQuery(string token, string owner, string spender)
        {
            if (!token.HasValue()) throw new ArgumentNullException("Token address must be set.");
            if (!owner.HasValue()) throw new ArgumentNullException("Owner address must be set.");
            if (!spender.HasValue()) throw new ArgumentNullException("Spender address must be set.");

            Token = token;
            Owner = owner;
            Spender = spender;
        }

        public string Token { get; }
        public string Owner { get; }
        public string Spender { get; }
    }
}
