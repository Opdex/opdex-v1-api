using System;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetTokenByAddressQuery : IRequest<TokenDto>
    {
        public GetTokenByAddressQuery(Address address)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address), "Token address must be provided.");
            }

            Address = address;
        }

        public Address Address { get; }
    }
}
