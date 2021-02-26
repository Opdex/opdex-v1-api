using System;
using MediatR;
using Opdex.Core.Common.Extensions;

namespace Opdex.Indexer.Application.Abstractions.EntryCommands
{
    public class CreateTransactionCommand : IRequest<bool>
    {
        public CreateTransactionCommand(string txHash)
        {
            if (!txHash.HasValue())
            {
                throw new ArgumentNullException(nameof(txHash));
            }
        }
        
        public string TxHash { get; set; }
    }
}