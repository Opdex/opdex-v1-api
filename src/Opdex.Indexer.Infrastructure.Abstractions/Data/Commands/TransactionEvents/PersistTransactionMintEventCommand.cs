using System;
using MediatR;
using Opdex.Indexer.Domain.Models.LogEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionMintEventCommand : IRequest
    {
        public PersistTransactionMintEventCommand(MintEvent mintEvent)
        {
            MintEvent = mintEvent ?? throw new ArgumentNullException(nameof(mintEvent));
        }
        
        public MintEvent MintEvent { get; }
    }
}