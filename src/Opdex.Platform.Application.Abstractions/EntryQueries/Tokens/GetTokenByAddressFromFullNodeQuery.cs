using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetTokenByAddressFromFullNodeQuery : IRequest<TokenDto>
    {
        public GetTokenByAddressFromFullNodeQuery(Address address)
        {
            if (address == Address.Empty)
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }

        public Address Address { get; }
    }
}
