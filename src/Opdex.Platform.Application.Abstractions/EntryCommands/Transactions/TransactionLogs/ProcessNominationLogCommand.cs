using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessNominationLogCommand : IRequest<bool>
    {
        public ProcessNominationLogCommand(TransactionLog log)
        {
            Log = log as NominationLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public NominationLog Log { get; }
    }
}