using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens
{
    public class ProcessTransferLogCommand : IRequest<bool>
    {
        public ProcessTransferLogCommand(TransactionLog log, string sender, ulong blockHeight)
        {
            if (!sender.HasValue())
            {
                throw new ArgumentNullException(nameof(sender));
            }

            if (blockHeight < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(blockHeight));
            }
            
            Log = log as TransferLog ?? throw new ArgumentNullException(nameof(log));
            Sender = sender;
            BlockHeight = blockHeight;
        }
        
        public TransferLog Log { get; }
        public string Sender { get; }
        public ulong BlockHeight { get; }
    }
}