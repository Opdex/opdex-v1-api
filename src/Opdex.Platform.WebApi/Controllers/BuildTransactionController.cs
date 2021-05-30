using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
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
            var command = new CreateWalletApproveAllowanceTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword,
                request.Token, request.Amount, request.Spender);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("add-liquidity")]
        public async Task<IActionResult> AddLiquidity(AddLiquidityRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletAddLiquidityTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword,
                request.LiquidityPool, request.AmountCrs, request.AmountSrc, request.Tolerance, request.Recipient, request.Market);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("remove-liquidity")]
        public async Task<IActionResult> RemoveLiquidity(RemoveLiquidityRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletRemoveLiquidityTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword,
                request.LiquidityPool, request.Liquidity, request.AmountCrsMin, request.AmountSrcMin, request.Recipient, request.Market);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("swap")]
        public async Task<IActionResult> Swap(SwapRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletSwapTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword,
                request.TokenIn, request.TokenOut, request.TokenInAmount, request.TokenOutAmount, request.TokenInExactAmount,
                request.Tolerance, request.Recipient, request.Market);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("skim")]
        public async Task<IActionResult> Skim(SkimRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletSkimTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword, request.LiquidityPool, request.Recipient);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("sync")]
        public async Task<IActionResult> Sync(SyncRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletSyncTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword, request.LiquidityPool);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("create-pool")]
        public async Task<IActionResult> CreatePool(CreatePoolRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletCreateLiquidityPoolTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword,
                request.Token, request.Market);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("start-staking")]
        public async Task<IActionResult> StartStaking(StartStakingRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletStartStakingTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword,
                request.Amount, request.LiquidityPool);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("stop-staking")]
        public async Task<IActionResult> StopStaking(StopStakingRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletStopStakingTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword,
                request.LiquidityPool, request.Amount, request.Liquidate);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("collect-staking-rewards")]
        public async Task<IActionResult> CollectStakingRewards(CollectStakingRewardsRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletCollectStakingRewardsTransactionCommand(request.WalletName, request.WalletAddress, request.WalletPassword,
                request.LiquidityPool, request.Liquidate);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("start-mining")]
        public async Task<IActionResult> StartMining(StartMiningRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletStartMiningTransactionCommand(request.WalletName, request.WalletAddress,
                request.WalletPassword, request.Amount, request.LiquidityPool);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("stop-mining")]
        public async Task<IActionResult> StopMining(StopMiningRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletStopMiningTransactionCommand(request.WalletName, request.WalletAddress,
                request.WalletPassword, request.LiquidityPool, request.Amount);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("collect-mining-rewards")]
        public async Task<IActionResult> CollectMiningRewards(CollectMiningRewardsRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletCollectMiningRewardsTransactionCommand(request.WalletName, request.WalletAddress,
                request.WalletPassword, request.LiquidityPool);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("distribute-odx")]
        public async Task<IActionResult> DistributeOdxTokens(DistributeTokensRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletDistributeTokensTransactionCommand(request.WalletName, request.WalletAddress,
                request.WalletPassword, request.Token);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }

        [HttpPost("reward-mining-pools")]
        public async Task<IActionResult> RewardMiningPools(RewardMiningPoolsRequest request, CancellationToken cancellationToken)
        {
            var command = new CreateWalletRewardMiningPoolsTransactionCommand(request.WalletName, request.WalletAddress,
                request.WalletPassword, request.Governance);

            var response = await _mediator.Send(command, cancellationToken);

            return Ok(new { TxHash = response });
        }
    }
}