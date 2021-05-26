using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Queries;
using Opdex.Platform.Domain.Models.Addresses;

namespace Opdex.Platform.Application.Abstractions.Queries.Addresses
{
    public class RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery : FindQuery<AddressStaking>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="liquidityPoolId"></param>
        /// <param name="owner"></param>
        /// <param name="findOrThrow"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public RetrieveAddressStakingByLiquidityPoolIdAndOwnerQuery(long liquidityPoolId, string owner, bool findOrThrow = true) : base(findOrThrow)
        {
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }

            if (!owner.HasValue())
            {
                throw new ArgumentNullException(nameof(owner));
            }

            LiquidityPoolId = liquidityPoolId;
            Owner = owner;
        }
        
        public long LiquidityPoolId { get; }
        public string Owner { get; }
    }
}