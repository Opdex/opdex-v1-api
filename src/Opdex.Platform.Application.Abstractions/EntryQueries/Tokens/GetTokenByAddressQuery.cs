using System;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    /// <summary>
    /// Get a token by its smart contract address.
    /// </summary>
    public class GetTokenByAddressQuery : IRequest<TokenDto>
    {
        /// <summary>
        /// Constructor to build a get token by address query.
        /// </summary>
        /// <param name="address">The token's smart contract address.</param>
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
