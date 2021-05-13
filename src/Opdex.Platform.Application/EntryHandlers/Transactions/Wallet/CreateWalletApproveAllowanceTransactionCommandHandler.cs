using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Application.Abstractions.Queries.Pools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Pools;

namespace Opdex.Platform.Application.EntryHandlers.Transactions.Wallet
{
    public class CreateWalletApproveAllowanceTransactionCommandHandler 
        : IRequestHandler<CreateWalletApproveAllowanceTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly ILogger<CreateWalletApproveAllowanceTransactionCommandHandler> _logger;

        public CreateWalletApproveAllowanceTransactionCommandHandler(IMediator mediator, 
            ILogger<CreateWalletApproveAllowanceTransactionCommandHandler> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<string> Handle(CreateWalletApproveAllowanceTransactionCommand request, CancellationToken cancellationToken)
        {
            int decimals;
            var token = await RetrieveToken(request.Token, cancellationToken);

            if (token == null)
            {
                var pool = await RetrievePool(request.Token, cancellationToken);

                if (pool == null)
                {
                    throw new Exception("Invalid token to approve allowance for.");
                }

                decimals = TokenConstants.LiquidityPoolToken.Decimals;
            }
            else
            {
                decimals = token.Decimals;
            }
            
            var amount = request.Amount.ToSatoshis(decimals);
            
            var command = new MakeWalletApproveAllowanceTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword,
                request.Token, amount, request.Spender);
            
            return await _mediator.Send(command, cancellationToken);
        }

        private async Task<Token> RetrieveToken(string tokenAddress, CancellationToken cancellationToken)
        {
            Token token = null;
            
            try
            {
                token = await _mediator.Send(new RetrieveTokenByAddressQuery(tokenAddress), cancellationToken);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, $"Token not found - {tokenAddress}");
            }

            return token;
        }
        
        private async Task<LiquidityPool> RetrievePool(string poolAddress, CancellationToken cancellationToken)
        {
            LiquidityPool pool = null;
            
            try
            {
                pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(poolAddress), cancellationToken);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, $"Pool not found - {poolAddress}");
            }

            return pool;
        }
    }
}