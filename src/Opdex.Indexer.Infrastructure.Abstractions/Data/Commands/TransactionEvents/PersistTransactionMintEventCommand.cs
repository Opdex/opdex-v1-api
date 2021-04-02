using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionMintEventCommand : IRequest<long>
    {
        public PersistTransactionMintEventCommand(MintEvent mintEvent)
        {
            MintEvent = mintEvent ?? throw new ArgumentNullException(nameof(mintEvent));
        }
        
        public MintEvent MintEvent { get; }
    }
}