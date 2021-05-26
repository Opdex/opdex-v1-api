using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions.TransactionLogs;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs
{
    public abstract class ProcessLogCommandHandler
    {
        protected readonly IMediator _mediator;

        protected ProcessLogCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        protected Task<bool> MakeTransactionLog(TransactionLog log)
        {
            return _mediator.Send(new PersistTransactionLogCommand(log), CancellationToken.None);
        }
    }
}