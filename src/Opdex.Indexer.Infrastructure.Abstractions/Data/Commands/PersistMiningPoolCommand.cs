using System;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands
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