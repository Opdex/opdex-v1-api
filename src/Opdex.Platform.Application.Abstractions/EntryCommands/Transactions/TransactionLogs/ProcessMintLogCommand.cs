using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessMintLogCommand : IRequest<bool>
    {
        public ProcessMintLogCommand(TransactionLog log)
        {
            Log = log as MintLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public MintLog Log { get; }
    }
}