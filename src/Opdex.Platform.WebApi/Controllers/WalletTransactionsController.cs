using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("wallet-transactions")]
    public class WalletTransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public WalletTransactionsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        [HttpGet]
        public Task<IActionResult> GetAllMyTransactions(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet("pool/{poolAddress}")]
        public Task<IActionResult> GetMyTransactionsForPool(string poolAddress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        [HttpGet("token/{tokenAddress}")]
        public Task<IActionResult> GetMyTransactionsForToken(string tokenAddress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        
        [HttpPost("build/approve-allowance")]
        public async Task<IActionResult> ApproveAllowance(ApproveAllowanceRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletApproveAllowanceTransactionCommand(request.Token, request.Amount, request.Owner, request.Spender);
            
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }
        
        [HttpPost("build/add-liquidity")]
        public async Task<IActionResult> AddLiquidity(AddLiquidityRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletAddLiquidityTransactionCommand(request.Token, request.AmountCrsDesired, request.AmountSrcDesired,
                request.AmountCrsMin, request.AmountSrcMin, request.To);
            
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }
        
        [HttpPost("build/remove-liquidity")]
        public async Task<IActionResult> RemoveLiquidity(RemoveLiquidityRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletRemoveLiquidityTransactionCommand(request.Token, request.Liquidity,
                request.AmountCrsMin, request.AmountSrcMin, request.To);
            
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }
        
        [HttpPost("build/swap")]
        public async Task<IActionResult> SwapExactTokens(SwapRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletSwapTransactionCommand(request.TokenIn, request.TokenOut, request.TokenInAmount, 
                request.TokenOutAmount, request.TokenInExactAmount, request.Tolerance, request.To);
            
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }
        
        [HttpPost("build/create-pool")]
        public async Task<IActionResult> CreatePool(CreatePoolRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletCreateLiquidityPoolTransactionCommand(request.Token, request.Sender);
            
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }
    }
}