using System;
using MediatR;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public abstract class ProcessTransactionLogCommand : IRequest<bool>
    {
        protected ProcessTransactionLogCommand(Address sender, ulong blockHeight)
        {
            if (sender == Address.Empty)
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

        public Address Sender { get; }
        public ulong BlockHeight { get; }
    }
}
