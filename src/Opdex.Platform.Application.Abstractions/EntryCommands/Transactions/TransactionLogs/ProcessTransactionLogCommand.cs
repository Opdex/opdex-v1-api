using System;
using MediatR;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public abstract class ProcessTransactionLogCommand : IRequest<bool>
    {
        protected ProcessTransactionLogCommand(string sender, ulong blockHeight)
        {
            if (!sender.HasValue())
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (blockHeight < 1)
            {
                throw new ArgumentNullException(nameof(blockHeight));
            }

            Sender = sender;
            BlockHeight = blockHeight;
        }
        
        public string Sender { get; }
        public ulong BlockHeight { get; }
    }
}