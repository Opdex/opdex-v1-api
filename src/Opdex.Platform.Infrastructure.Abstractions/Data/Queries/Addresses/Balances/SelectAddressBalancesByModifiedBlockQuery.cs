using MediatR;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances
{
    public class SelectAddressBalancesByModifiedBlockQuery : IRequest<IEnumerable<AddressBalance>>
    {
        public SelectAddressBalancesByModifiedBlockQuery(ulong blockHeight)
        {
            if (blockHeight < 1)
            {
                throw new ArgumentNullException(nameof(blockHeight), "Block height must be greater than 0.");
            }

            BlockHeight = blockHeight;
        }

        public ulong BlockHeight { get; }
    }
}
