using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetAllTokensByMarketAddressQuery : IRequest<IEnumerable<TokenDto>>
    {
        public GetAllTokensByMarketAddressQuery(string marketAddress)
        {
            if (!marketAddress.HasValue())
            {
                throw new ArgumentNullException(nameof(marketAddress));
            }

            MarketAddress = marketAddress;
        }

        public string MarketAddress { get; }
    }
}
