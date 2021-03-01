using System;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Core.Common.Extensions;

namespace Opdex.Core.Application.Abstractions.EntryQueries.Tokens
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