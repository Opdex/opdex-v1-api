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

        // Transactions that include CRS have a bug at the FN that doesn't allow quotes to work
        // Keeping this around until we can safely remove it.
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
    }
}
