using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses
{
    public class RetrieveAddressBalanceByTokenIdAndOwnerQuery : IRequest<AddressBalance>
    {
        public RetrieveAddressBalanceByTokenIdAndOwnerQuery(long tokenId, string owner)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }

            TokenId = tokenId;
            Owner = owner;
        }
        
        public long TokenId { get; }
        public string Owner { get; }
    }
}