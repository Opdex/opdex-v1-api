using System;
using MediatR;
using Opdex.Core.Common.Extensions;

namespace Opdex.Indexer.Application.Abstractions.Commands.Pools
{
    public class MakeMiningPoolCommand : IRequest<long>
    {
        public MakeMiningPoolCommand(string miningPool, long liquidityPoolId)
        {
            if (!miningPool.HasValue())
            {
                throw new ArgumentNullException(nameof(miningPool));
            }
            
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }

            MiningPool = miningPool;
            LiquidityPoolId = liquidityPoolId;
        }
        
        public string MiningPool { get; }
        public long LiquidityPoolId { get; }
    }
}