using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionApprovalEventCommand : IRequest
    {
        public PersistTransactionApprovalEventCommand(ApprovalEvent approvalEvent)
        {
            ApprovalEvent = approvalEvent ?? throw new ArgumentNullException(nameof(approvalEvent));
        }
        
        public ApprovalEvent ApprovalEvent { get; }
    }
}