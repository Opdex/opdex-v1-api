using System;
using MediatR;
using Opdex.Core.Domain.Models.TransactionEvents;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionEvents
{
    public class PersistTransactionApprovalEventCommand : IRequest<bool>
    {
        public PersistTransactionApprovalEventCommand(ApprovalEvent approvalEvent)
        {
            ApprovalEvent = approvalEvent ?? throw new ArgumentNullException(nameof(approvalEvent));
        }
        
        public ApprovalEvent ApprovalEvent { get; }
    }
}