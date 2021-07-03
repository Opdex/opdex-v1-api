using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Extensions;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Addresses
{
    public class GetAddressAllowancesApprovedByOwnerQuery : IRequest<IEnumerable<AddressAllowanceDto>>
    {
        public GetAddressAllowancesApprovedByOwnerQuery(string owner, string spender = "", string token = "")
        {
            if (!owner.HasValue()) throw new ArgumentNullException("Owner address must be set.");

            Owner = owner;
            Spender = spender;
            Token = token;
        }

        public string Owner { get; }
        public string Spender { get; }
        public string Token { get; }
    }
}
