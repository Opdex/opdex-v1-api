using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Application.Abstractions.Commands.Pools
{
    public class MakeMiningPoolCommand : IRequest<long>
    {
        public MakeMiningPoolCommand(MiningPool miningPool)
        {
            MiningPool = miningPool ?? throw new ArgumentNullException(nameof(miningPool));
        }
        
        public MiningPool MiningPool { get; }
    }
}