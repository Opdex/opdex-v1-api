using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions.Wallet;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
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
        private readonly NetworkType _network;

        public BuildTransactionController(OpdexConfiguration opdexConfiguration, IMediator mediator, IApplicationContext context)
        {
            _network = opdexConfiguration?.Network ?? throw new ArgumentNullException(nameof(opdexConfiguration));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost("approve-allowance")]
        public async Task<IActionResult> ApproveAllowance(ApproveAllowanceRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletApproveAllowanceTransactionCommand(_context.Wallet, request.Token, request.Amount, request.Spender);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        [HttpPost("add-liquidity")]
        public async Task<IActionResult> AddLiquidity(AddLiquidityRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletAddLiquidityTransactionCommand(_context.Wallet, request.LiquidityPool, request.AmountCrs, request.AmountSrc,
                                                                             request.Tolerance, request.Recipient, _context.Market);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        [HttpPost("remove-liquidity")]
        public async Task<IActionResult> RemoveLiquidity(RemoveLiquidityRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletRemoveLiquidityTransactionCommand(_context.Wallet, request.LiquidityPool, request.Liquidity,
                                                                                request.AmountCrsMin, request.AmountSrcMin, request.Recipient,
                                                                                _context.Market);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [HttpPost("swap")]
        public async Task<IActionResult> Swap(SwapRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletSwapTransactionCommand(_context.Wallet, request.TokenIn, request.TokenOut, request.TokenInAmount,
                                                                     request.TokenOutAmount, request.TokenInExactAmount, request.Tolerance,
                                                                     request.Recipient, _context.Market);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        [HttpPost("skim")]
        public async Task<IActionResult> Skim(SkimRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletSkimTransactionCommand(_context.Wallet, request.LiquidityPool, request.Recipient.ToString());

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        [HttpPost("sync")]
        public async Task<IActionResult> Sync(SyncRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletSyncTransactionCommand(_context.Wallet, request.LiquidityPool);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        [HttpPost("create-pool")]
        public async Task<IActionResult> CreatePool(CreatePoolRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletCreateLiquidityPoolTransactionCommand(_context.Wallet, request.Token, _context.Market);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        [HttpPost("start-staking")]
        public async Task<IActionResult> StartStaking(StartStakingRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletStartStakingTransactionCommand(_context.Wallet, request.Amount, request.LiquidityPool);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        [HttpPost("stop-staking")]
        public async Task<IActionResult> StopStaking(StopStakingRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletStopStakingTransactionCommand(_context.Wallet, request.LiquidityPool, request.Amount, request.Liquidate);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        [HttpPost("collect-staking-rewards")]
        public async Task<IActionResult> CollectStakingRewards(CollectStakingRewardsRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletCollectStakingRewardsTransactionCommand(_context.Wallet, request.LiquidityPool, request.Liquidate);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        [HttpPost("start-mining")]
        public async Task<IActionResult> StartMining(StartMiningRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletStartMiningTransactionCommand(_context.Wallet, request.Amount, request.LiquidityPool);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        [HttpPost("stop-mining")]
        public async Task<IActionResult> StopMining(StopMiningRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletStopMiningTransactionCommand(_context.Wallet, request.LiquidityPool, request.Amount);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [Obsolete]
        [HttpPost("collect-mining-rewards")]
        public async Task<IActionResult> CollectMiningRewards(CollectMiningRewardsRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletCollectMiningRewardsTransactionCommand(_context.Wallet, request.LiquidityPool);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        [HttpPost("distribute")]
        public async Task<IActionResult> DistributeGovernanceTokens(DistributeTokensRequest request, CancellationToken cancellationToken)
        {
            if (_network == NetworkType.DEVNET)
            {
                var command = new CreateWalletDistributeTokensTransactionCommand(_context.Wallet, request.Token);

                var response = await _mediator.Send(command, cancellationToken);

                return Ok(new { TxHash = response });
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
