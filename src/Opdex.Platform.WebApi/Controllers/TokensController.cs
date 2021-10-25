﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Tokens;
using Opdex.Platform.WebApi.Models.Requests.WalletTransactions;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Transactions;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("tokens")]
    public class TokensController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IApplicationContext _context;

        public TokensController(IMediator mediator, IMapper mapper, IApplicationContext applicationContext)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }

        /// <summary>Get Tokens</summary>
        /// <remarks>Retrieve tokens from within a market with a filter.</remarks>
        /// <param name="filters">Token search filters.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Filtered tokens with paging.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(TokensResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<TokensResponseModel>> GetTokens([FromQuery] TokenFilterParameters filters, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTokensWithFilterQuery(filters.BuildCursor()), cancellationToken);

            var response = _mapper.Map<TokensResponseModel>(result);

            return Ok(response);
        }

        /// <summary>
        /// Add Token
        /// </summary>
        /// <remarks>Adds an SRC token to the Opdex indexer that can be tracked and used within markets.</remarks>
        /// <param name="request">Token details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Token details.</returns>
        /// <response code="201">The token was added to indexed tokens.</response>
        /// <response code="303">Token is already indexed.</response>
        /// <response code="400">The address provided was not a valid token.</response>
        [HttpPost]
        [ProducesResponseType(typeof(TokenResponseModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status303SeeOther)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> AddToken([FromBody] AddTokenRequest request, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new CreateAddTokenCommand(request.TokenAddress), cancellationToken);

            var response = _mapper.Map<TokenResponseModel>(result);

            return Created($"/tokens/{request.TokenAddress}", response);
        }

        /// <summary>Get Token</summary>
        /// <remarks>Returns the token that matches the provided address.</remarks>
        /// <param name="address">The token's smart contract address.</param>
        /// <param name="cancellationToken">cancellation token.</param>
        /// <returns><see cref="TokenResponseModel"/> of the requested token</returns>
        [HttpGet("{address}")]
        [ProducesResponseType(typeof(TokenResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TokenResponseModel>> GetToken([FromRoute] Address address, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetTokenByAddressQuery(address), cancellationToken);

            var response = _mapper.Map<TokenResponseModel>(result);

            return Ok(response);
        }

        ///<summary>Get Token History</summary>
        /// <remarks>Retrieve historical data points for a tokens tracking open, high, low, and close of USD prices.</remarks>
        /// <param name="address">The address of the token.</param>
        /// <param name="candleSpan">"Hourly" or "Daily" determining the time span of each data point. Default is daily.</param>
        /// <param name="timespan">"1D", "1W", "1M", "1Y" determining how much history to fetch. Default is 1 week.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns><see cref="TokenSnapshotHistoryResponseModel"/> with a list of historical data points.</returns>
        [HttpGet("{address}/history")]
        [ProducesResponseType(typeof(TokenSnapshotHistoryResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TokenSnapshotHistoryResponseModel>> TokenHistory([FromRoute] Address address,
                                                                                        [FromQuery] string candleSpan,
                                                                                        [FromQuery] string timespan,
                                                                                        CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(address), cancellationToken);

            var tokenSnapshotDtos = await _mediator.Send(new GetTokenSnapshotsWithFilterQuery(address,
                                                                                             _context.Market,
                                                                                             candleSpan,
                                                                                             timespan), cancellationToken);

            var response = new TokenSnapshotHistoryResponseModel
            {
                Address = token.Address,
                SnapshotHistory = _mapper.Map<IEnumerable<TokenSnapshotResponseModel>>(tokenSnapshotDtos)
            };

            return Ok(response);
        }

        /// <summary>Approve Allowance Quote</summary>
        /// <remarks>Quotes a transaction to approve an SRC token allowance for a spender.</remarks>
        /// <param name="address">The token address to approve an allowance of.</param>
        /// <param name="request">The allowance approval request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Transaction quote of an approve allowance transaction.</returns>
        [HttpPost("{address}/approve")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveAllowance([FromRoute] Address address,
                                                          [FromBody] ApproveAllowanceRequest request,
                                                          CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateApproveAllowanceTransactionQuoteCommand(address,
                                                                                                  _context.Wallet,
                                                                                                  request.Spender,
                                                                                                  request.Amount), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }

        /// <summary>Distribute Tokens Quote</summary>
        /// <remarks>Quotes a transaction to distribute SRC tokens on eligible token types.</remarks>
        /// <param name="address">The address of the token smart contract.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Token distribute transaction quote.</returns>
        [HttpPost("{address}/distribute")]
        [ProducesResponseType(typeof(TransactionQuoteResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Distribute([FromRoute] Address address, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new CreateDistributeTokensTransactionQuoteCommand(address, _context.Wallet), cancellationToken);

            var quote = _mapper.Map<TransactionQuoteResponseModel>(response);

            return Ok(quote);
        }
    }
}
