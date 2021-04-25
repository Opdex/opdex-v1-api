using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools
{
    public class PersistMiningPoolCommand : IRequest<long>
    {
        public PersistMiningPoolCommand(MiningPool miningPool)
        {
            MiningPool = miningPool ?? throw new ArgumentNullException(nameof(miningPool));
        }
        
        public MiningPool MiningPool { get; }
    }
}