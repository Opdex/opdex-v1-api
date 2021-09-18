using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Governances;
using Opdex.Platform.Domain.Models.TransactionLogs.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Governances
{
    public class ProcessNominationLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessNominationLogCommand, bool>
    {
        private readonly ILogger<ProcessNominationLogCommandHandler> _logger;

        public ProcessNominationLogCommandHandler(IMediator mediator, ILogger<ProcessNominationLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessNominationLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                await _mediator.Send(new CreateGovernanceNominationsCommand(request.Log.Contract, request.BlockHeight));

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(NominationLog)}");

                return false;
            }
        }
    }
}
