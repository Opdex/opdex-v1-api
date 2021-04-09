using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionLogs;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs
{
    public class PersistTransactionApprovalLogCommand : IRequest<long>
    {
        public PersistTransactionApprovalLogCommand(ApprovalLog approvalLog)
        {
            ApprovalLog = approvalLog ?? throw new ArgumentNullException(nameof(approvalLog));
        }
        
        public ApprovalLog ApprovalLog { get; }
    }
}