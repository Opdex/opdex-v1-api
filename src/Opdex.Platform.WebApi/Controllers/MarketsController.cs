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
    }
}
