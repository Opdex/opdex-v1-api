using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Markets;
using System.Collections.Generic;
using Opdex.Platform.WebApi.Models.Responses.Transactions;
using Opdex.Platform.WebApi.Models.Requests.Markets;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("markets")]
    public class MarketsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IApplicationContext _context;

        public MarketsController(IMediator mediator, IMapper mapper, IApplicationContext context)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>Get Market</summary>
        /// <remarks>Retrieves a market.</remarks>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Market</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MarketSnapshotResponseModel>> GetMarketDetails(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMarketByAddressQuery(_context.Market), cancellationToken);

            var response = _mapper.Map<MarketResponseModel>(result);

            return Ok(response);
        }

        /// <summary>Get Market History</summary>
        /// <summary>Retrieves the history of a market.</summary>
        /// <param name="from">From Date</param>
        /// <param name="to">To Date</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>Market History</returns>
        [HttpGet("history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MarketSnapshotResponseModel>> GetMarketHistory(DateTime? from, DateTime? to, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMarketSnapshotsWithFilterQuery(_context.Market, from, to), cancellationToken);

            var response = _mapper.Map<IEnumerable<MarketSnapshotResponseModel>>(result);

            return Ok(response);
        }

        /// <summary>Create Standard Market Quote</summary>
        /// <remarks>Quote a transaction to create a standard market.</remarks>
        /// <param name="request">Information about the standard market.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
        [HttpPost("standard")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> CreateStandardMarketQuote(CreateStandardMarketQuoteRequest request,
                                                                                                 CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateCreateStandardMarketTransactionQuoteCommand(_context.Wallet,
                                                                                                      request.MarketOwner,
                                                                                                      request.TransactionFee,
                                                                                                      request.AuthPoolCreators,
                                                                                                      request.AuthLiquidityProviders,
                                                                                                      request.AuthTraders,
                                                                                                      request.EnableMarketFee), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Set Ownership Quote</summary>
        /// <remarks>Quote a transaction to set the owner of a standard market, pending a transaction to redeem ownership.</remarks>
        /// <param name="address">The address of the standard market.</param>
        /// <param name="request">Information about the new owner.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
        [HttpPost("{address}/standard/set-ownership")]
        public async Task<ActionResult<TransactionQuoteResponseModel>> SetOwnershipQuote([FromRoute] Address address,
                                                                                         SetMarketOwnerQuoteRequest request,
                                                                                         CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateSetStandardMarketOwnershipTransactionQuoteCommand(address, _context.Wallet, request.Owner),
                                                cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Claim Ownership Quote</summary>
        /// <remarks>Quote a transaction to claim ownership of a standard market.</remarks>
        /// <param name="address">The address of the standard market.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
        [HttpPost("{address}/standard/claim-ownership")]
        public async Task<ActionResult<TransactionQuoteResponseModel>> ClaimOwnershipQuote([FromRoute] Address address,
                                                                                           CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateClaimStandardMarketOwnershipTransactionQuoteCommand(address, _context.Wallet),
                                                cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Set Permissions Quote</summary>
        /// <remarks>Quote a transaction to set permissions within a standard market.</remarks>
        /// <param name="address">The address of the standard market.</param>
        /// <param name="walletAddress">The address to assign permissions.</param>
        /// <param name="request">Information about the permissions.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
        [HttpPost("{address}/standard/permissions/{walletAddress}")]
        public async Task<ActionResult<TransactionQuoteResponseModel>> SetPermissionsQuote([FromRoute] Address address,
                                                                                           [FromRoute] Address walletAddress,
                                                                                           SetMarketPermissionsQuoteRequest request,
                                                                                           CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateSetStandardMarketPermissionsTransactionQuoteCommand(address,
                                                                                                              _context.Wallet,
                                                                                                              walletAddress,
                                                                                                              request.Permission,
                                                                                                              request.Authorize), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Collect Fees Quote</summary>
        /// <param name="address">The address of the standard market.</param>
        /// <param name="request">Information about the fees to collect.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
        [HttpPost("{address}/standard/collect-fees")]
        public async Task<ActionResult<TransactionQuoteResponseModel>> CollectFeesQuote([FromRoute] Address address,
                                                                                        CollectMarketFeesQuoteRequest request,
                                                                                        CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateCollectStandardMarketFeesTransactionQuoteCommand(address, _context.Wallet, request.Token, request.Amount),
                                                cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Create Staking Market Quote</summary>
        /// <param name="request">Information about the staking market.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TransactionQuoteResponseModel"/> with the quoted result and the properties used to obtain the quote.</returns>
        [HttpPost("staking")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<TransactionQuoteResponseModel>> CreateStakingMarketQuote(CreateStakingMarketQuoteRequest request,
                                                                                                CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateCreateStakingMarketTransactionQuoteCommand(_context.Wallet, request.StakingToken), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }
    }
}
