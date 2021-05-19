using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.Commands.Pools
{
    public class MakeMiningPoolCommand : IRequest<long>
    {
        public MakeMiningPoolCommand(string miningPool, long liquidityPoolId, ulong createdBlock, ulong modifiedBlock)
        {
            if (!miningPool.HasValue())
            {
                throw new ArgumentNullException(nameof(miningPool));
            }
            
            if (liquidityPoolId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(liquidityPoolId));
            }
            
            if (createdBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(createdBlock));
            }
            
            if (modifiedBlock < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(modifiedBlock));
            }

            MiningPool = miningPool;
            LiquidityPoolId = liquidityPoolId;
            CreatedBlock = createdBlock;
            ModifiedBlock = modifiedBlock;
        }
        
        public string MiningPool { get; }
        public long LiquidityPoolId { get; }
        public ulong CreatedBlock { get; }
        public ulong ModifiedBlock { get; }
    }
}