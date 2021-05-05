using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletAddLiquidityTransactionCommandHandler 
        : IRequestHandler<CreateWalletAddLiquidityTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public CreateWalletAddLiquidityTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<string> Handle(CreateWalletAddLiquidityTransactionCommand request, CancellationToken cancellationToken)
        {
            var pool = await _mediator.Send(new GetLiquidityPoolByAddressQuery(request.Pool), cancellationToken);
            
            if (!decimal.TryParse(request.AmountCrs, out var amountCrsDecimal))
            {
                throw new Exception("Cannot add liquidity with 0 CRS");
            }
            
            var amountCrsMin = Math.Round(amountCrsDecimal * request.Tolerance, 8).ToSatoshis(TokenConstants.Cirrus.Decimals);

            var amountSrc = request.AmountSrc.ToSatoshis(pool.Token.Decimals);
            var amountSrcMin = amountSrc.ToleranceAsSatoshis(request.Tolerance);
                
            var command = new MakeWalletAddLiquidityTransactionCommand(pool.Token.Address, request.AmountCrs, amountSrc,
                amountCrsMin.ToString(), amountSrcMin, request.To, request.Market);
            
            return await _mediator.Send(command, cancellationToken);
        }
    }
}