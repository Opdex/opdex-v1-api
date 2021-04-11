using System;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands
{
    public class PersistLiquidityPoolCommand : IRequest<long>
    {
        public PersistLiquidityPoolCommand(LiquidityPool liquidityPool)
        {
            Pool = liquidityPool ?? throw new ArgumentNullException(nameof(liquidityPool));
        }
        
        public LiquidityPool Pool { get;  }
    }
}