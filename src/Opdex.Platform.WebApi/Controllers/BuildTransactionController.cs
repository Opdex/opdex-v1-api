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
    [Route("build-transaction/local-broadcast")]
    public class BuildTransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public BuildTransactionController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        [HttpPost("approve-allowance")]
        public async Task<IActionResult> ApproveAllowance(ApproveAllowanceRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletApproveAllowanceTransactionCommand(request.Token, request.Amount, request.Owner, request.Spender);
            
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }
        
        [HttpPost("add-liquidity")]
        public async Task<IActionResult> AddLiquidity(AddLiquidityRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletAddLiquidityTransactionCommand(request.Pool, request.AmountCrs, request.AmountSrc,
                request.Tolerance, request.WalletAddress, request.Market);
            
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }
        
        [HttpPost("remove-liquidity")]
        public async Task<IActionResult> RemoveLiquidity(RemoveLiquidityRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletRemoveLiquidityTransactionCommand(request.Token, request.Liquidity,
                request.AmountCrsMin, request.AmountSrcMin, request.To, request.Market);
            
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }
        
        [HttpPost("swap")]
        public async Task<IActionResult> Swap(SwapRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletSwapTransactionCommand(request.TokenIn, request.TokenOut, request.TokenInAmount, 
                request.TokenOutAmount, request.TokenInExactAmount, request.Tolerance, request.To, request.Market);
            
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }
        
        [HttpPost("create-pool")]
        public async Task<IActionResult> CreatePool(CreatePoolRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletCreateLiquidityPoolTransactionCommand(request.Token, request.Sender, request.Market);
            
            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }
    }
}