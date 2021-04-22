using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands
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