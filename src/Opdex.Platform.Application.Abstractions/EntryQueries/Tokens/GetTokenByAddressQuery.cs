using System;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetTokenByAddressQuery : IRequest<TokenDto>
    {
        public GetTokenByAddressQuery(string address, string market = null)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
            Market = market;
        }

        public string Address { get; }
        public string Market { get; }
    }
}
