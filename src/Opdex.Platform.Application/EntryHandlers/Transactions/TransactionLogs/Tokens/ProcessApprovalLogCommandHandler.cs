using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens
{
    public class ProcessApprovalLogCommandHandler : ProcessLogCommandHandler, IRequestHandler<ProcessApprovalLogCommand, bool>
    {
        private readonly ILogger<ProcessApprovalLogCommandHandler> _logger;

        public ProcessApprovalLogCommandHandler(IMediator mediator, ILogger<ProcessApprovalLogCommandHandler> logger) : base(mediator)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ProcessApprovalLogCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var persisted = await MakeTransactionLog(request.Log);
                if (!persisted)
                {
                    return false;
                }

                var liquidityPoolAddress = request.Log.Contract;
                var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(liquidityPoolAddress, findOrThrow: false), CancellationToken.None);
                if (liquidityPool is null)
                {
                    return false;
                }

                var allowance = await _mediator.Send(new RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery(liquidityPool.LpTokenId,
                                                                                                                  request.Log.Owner,
                                                                                                                  request.Log.Spender,
                                                                                                                  findOrThrow: false), CancellationToken.None);

                if (allowance is null || request.BlockHeight >= allowance.ModifiedBlock)
                {
                    allowance ??= new AddressAllowance(liquidityPool.LpTokenId, request.Log.Owner, request.Log.Spender,
                                                       request.Log.Amount, request.BlockHeight);

                    allowance.SetAllowance(request.Log.Amount, request.BlockHeight);

                    await _mediator.Send(new MakeAddressAllowanceCommand(allowance), CancellationToken.None);
                }

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure processing {nameof(ApprovalLog)}");

                return false;
            }
        }
    }
}
