using System;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens
{
    public class GetTokenByAddressQuery : IRequest<TokenDto>
    {
        public GetTokenByAddressQuery(string address)
        {
            if (!address.HasValue())
            {
                throw new ArgumentNullException(nameof(address));
            }

            Address = address;
        }
        
        public string Address { get; }
    }
}