using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Core.Application.Abstractions.Queries.Transactions;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionLogs;
using Opdex.Indexer.Application.Abstractions.Commands.Transactions;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands.TransactionLogs;

namespace Opdex.Indexer.Application.Handlers.Transactions
{
    public class MakeTransactionCommandHandler : IRequestHandler<MakeTransactionCommand, bool>
    {
        private readonly IMediator _mediator;
        public MakeTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public async Task<bool> Handle(MakeTransactionCommand request, CancellationToken cancellationToken)
        {
            var transaction = await GetTransaction(request.Transaction.Hash);

            if (transaction != null) return false;
            
            var transactionId = await _mediator.Send(new PersistTransactionCommand(request.Transaction));

            if (transactionId == null) return false;
            
            foreach (var log in request.Transaction.Logs)
            {
                await MakeTransactionLog(log);
            }

            return true;
        }

        private async Task<Transaction> GetTransaction(string txHash)
        {
            Transaction transaction;
            
            try
            {
                transaction = await _mediator.Send(new RetrieveTransactionByHashQuery(txHash), CancellationToken.None);
            }
            catch (NotFoundException)
            {
                transaction = null;
            }

            return transaction;
        }

        private async Task MakeTransactionLog(TransactionLog log)
        {
            var logId = log switch
            {
                ReservesLog syncLog => await _mediator.Send(new PersistTransactionReservesLogCommand(syncLog)),
                MintLog mintLog => await _mediator.Send(new PersistTransactionMintLogCommand(mintLog)),
                BurnLog burnLog => await _mediator.Send(new PersistTransactionBurnLogCommand(burnLog)),
                SwapLog swapLog => await _mediator.Send(new PersistTransactionSwapLogCommand(swapLog)),
                ApprovalLog approvalLog => await _mediator.Send(new PersistTransactionApprovalLogCommand(approvalLog)),
                TransferLog transferLog => await _mediator.Send(new PersistTransactionTransferLogCommand(transferLog)),
                LiquidityPoolCreatedLog poolCreatedLog => await _mediator.Send(new PersistTransactionLiquidityPoolCreatedLogCommand(poolCreatedLog)),
                _ => 0
            };

            if (logId == 0)  return;
            
            var command = new PersistTransactionLogSummaryCommand(log, logId);
            await _mediator.Send(command);
        }
    }
}