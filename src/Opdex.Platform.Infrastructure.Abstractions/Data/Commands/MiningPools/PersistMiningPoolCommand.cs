using MediatR;
using Opdex.Platform.Domain.Models.MiningPools;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.MiningPools
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
