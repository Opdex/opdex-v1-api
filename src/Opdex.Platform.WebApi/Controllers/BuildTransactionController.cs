using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("build-transaction/local-broadcast")]
    public class BuildTransactionController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IApplicationContext _context;

        public BuildTransactionController(IMediator mediator, IApplicationContext context)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost("approve-allowance")]
        public async Task<IActionResult> ApproveAllowance(ApproveAllowanceRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletApproveAllowanceTransactionCommand(_context.Wallet, request.Token, request.Amount, request.Spender);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("add-liquidity")]
        public async Task<IActionResult> AddLiquidity(AddLiquidityRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletAddLiquidityTransactionCommand(_context.Wallet, request.LiquidityPool, request.AmountCrs, request.AmountSrc,
                                                                         request.Tolerance, request.Recipient, request.Market);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("remove-liquidity")]
        public async Task<IActionResult> RemoveLiquidity(RemoveLiquidityRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletRemoveLiquidityTransactionCommand(_context.Wallet, request.LiquidityPool, request.Liquidity,
                                                                            request.AmountCrsMin, request.AmountSrcMin, request.Recipient,
                                                                            request.Market);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("swap")]
        public async Task<IActionResult> Swap(SwapRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletSwapTransactionCommand(_context.Wallet, request.TokenIn, request.TokenOut, request.TokenInAmount,
                                                                 request.TokenOutAmount, request.TokenInExactAmount, request.Tolerance,
                                                                 request.Recipient, request.Market);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("skim")]
        public async Task<IActionResult> Skim(SkimRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletSkimTransactionCommand(_context.Wallet, request.LiquidityPool, request.Recipient);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync(SyncRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletSyncTransactionCommand(_context.Wallet, request.LiquidityPool);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("create-pool")]
        public async Task<IActionResult> CreatePool(CreatePoolRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletCreateLiquidityPoolTransactionCommand(_context.Wallet, request.Token, request.Market);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("start-staking")]
        public async Task<IActionResult> StartStaking(StartStakingRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletStartStakingTransactionCommand(_context.Wallet, request.Amount, request.LiquidityPool);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("stop-staking")]
        public async Task<IActionResult> StopStaking(StopStakingRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletStopStakingTransactionCommand(_context.Wallet, request.LiquidityPool, request.Amount, request.Liquidate);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("collect-staking-rewards")]
        public async Task<IActionResult> CollectStakingRewards(CollectStakingRewardsRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletCollectStakingRewardsTransactionCommand(_context.Wallet, request.LiquidityPool, request.Liquidate);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("start-mining")]
        public async Task<IActionResult> StartMining(StartMiningRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletStartMiningTransactionCommand(_context.Wallet, request.Amount, request.LiquidityPool);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("stop-mining")]
        public async Task<IActionResult> StopMining(StopMiningRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletStopMiningTransactionCommand(_context.Wallet, request.LiquidityPool, request.Amount);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("collect-mining-rewards")]
        public async Task<IActionResult> CollectMiningRewards(CollectMiningRewardsRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletCollectMiningRewardsTransactionCommand(_context.Wallet, request.LiquidityPool);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("distribute-odx")]
        public async Task<IActionResult> DistributeOdxTokens(DistributeTokensRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletDistributeTokensTransactionCommand(_context.Wallet, request.Token);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("reward-mining-pools")]
        public async Task<IActionResult> RewardMiningPools(RewardMiningPoolsRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletRewardMiningPoolsTransactionCommand(_context.Wallet, request.Governance);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }
    }
}
