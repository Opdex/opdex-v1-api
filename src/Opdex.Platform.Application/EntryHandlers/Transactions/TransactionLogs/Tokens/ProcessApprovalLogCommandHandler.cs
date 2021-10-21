using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.TransactionLogs.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.TransactionLogs.Tokens
{
    public class ProcessApprovalLogCommandHandler : IRequestHandler<ProcessApprovalLogCommand, bool>
    {
        private readonly IMediator _mediator;

        public ProcessApprovalLogCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<bool> Handle(ProcessApprovalLogCommand request, CancellationToken cancellationToken)
        {
            var spender = request.Log.Spender;

            // Allowances to routers for swaps/adding/removing liquidity
            var router = await _mediator.Send(new RetrieveMarketRouterByAddressQuery(spender, findOrThrow: false));
            if (router != null)
            {
                return true;
            }

            // Allowances to liquidity pools for staking transactions/contract integrations
            var liquidityPool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(spender, findOrThrow: false));
            if (liquidityPool != null)
            {
                return true;
            }

            // Allowances to mining pools for start mining transactions
            var miningPool = await _mediator.Send(new RetrieveMiningPoolByAddressQuery(spender, findOrThrow: false));
            if (miningPool != null)
            {
                return true;
            }

            // If we're here, it's an approval we don't care about
            return false;
        }
    }
}
