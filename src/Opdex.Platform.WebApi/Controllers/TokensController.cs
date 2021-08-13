﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Tokens;

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
        /// <param name="lpToken">Optional flag to return liquidity pool tokens or not.</param>
        /// <param name="skip">How many records to skip for pagination.</param>
        /// <param name="take">How many records to take for pagination</param>
        /// <param name="sortBy">Sort By</param>
        /// <param name="orderBy">Order By</param>
        /// <param name="tokens">Specific token addresses to filter for.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>List of filtered tokens.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TokenResponseModel>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TokenResponseModel>>> Tokens([FromQuery] bool? lpToken,
                                                                                [FromQuery] uint? skip,
                                                                                [FromQuery] uint? take,
                                                                                [FromQuery] string sortBy,
                                                                                [FromQuery] string orderBy,
                                                                                [FromQuery] IEnumerable<string> tokens,
                                                                                CancellationToken cancellationToken)
        {
            var query = new GetTokensWithFilterQuery(_context.Market, lpToken, skip ?? 0, take ?? 10, sortBy, orderBy, tokens);

            var result = await _mediator.Send(query, cancellationToken);

            var response = _mapper.Map<IEnumerable<TokenResponseModel>>(result);

            return Ok(response);
        }

        /// <summary>Get Token</summary>
        /// <remarks>Returns the token that matches the provided address.</remarks>
        /// <param name="address">The token's smart contract address.</param>
        /// <param name="cancellationToken">cancellation token.</param>
        /// <returns><see cref="TokenResponseModel"/> of the requested token</returns>
        [HttpGet("{address}")]
        [ProducesResponseType(typeof(TokenResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TokenResponseModel>> Token(string address, CancellationToken cancellationToken)
        {
            var query = new GetTokenByAddressQuery(address, _context.Market);

            var result = await _mediator.Send(query, cancellationToken);

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
        public async Task<ActionResult<TokenSnapshotHistoryResponseModel>> TokenHistory(string address,
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
    }
}
