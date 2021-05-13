using System;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs
{
    public class ProcessApprovalLogCommand : IRequest<bool>
    {
        public ProcessApprovalLogCommand(TransactionLog log)
        {
            Log = log as ApprovalLog ?? throw new ArgumentNullException(nameof(log));
        }
        
        public ApprovalLog Log { get; }
    }
}