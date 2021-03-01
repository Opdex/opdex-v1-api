using System;
using MediatR;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;

namespace Opdex.Core.Application.Abstractions.Queries.Tokens
{
    public class RetrieveTokenByAddressQuery : IRequest<Token>
    {
        public RetrieveTokenByAddressQuery(string address)
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