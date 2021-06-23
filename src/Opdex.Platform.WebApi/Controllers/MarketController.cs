using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.WebApi.Models.Responses.Markets;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("market")]
    public class MarketController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public MarketController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpGet("{market}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MarketSnapshotResponseModel>> GetMarketDetails(string market, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMarketByAddressQuery(market), cancellationToken);

            var response = _mapper.Map<MarketResponseModel>(result);

            return Ok(response);
        }

        [HttpGet("{market}/history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MarketSnapshotResponseModel>> GetMarketHistory(string market, DateTime? from, DateTime? to, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetMarketSnapshotsWithFilterQuery(market, from, to), cancellationToken);

            var response = _mapper.Map<IEnumerable<MarketSnapshotResponseModel>>(result);

            return Ok(response);
        }
    }
}
