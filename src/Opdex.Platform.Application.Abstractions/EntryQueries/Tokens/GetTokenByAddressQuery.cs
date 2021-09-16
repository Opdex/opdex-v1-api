using System;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetTokenByAddressQuery : IRequest<TokenDto>
    {
        public GetTokenByAddressQuery(Address address, Address? market = null)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
            Market = market ?? Address.Empty;
        }

        public Address Address { get; }
        public Address Market { get; }
    }
}
