using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Market;
using Opdex.Platform.WebApi.Models.Responses;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
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

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<MarketSnapshotResponseModel>> GetMarketDetails(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetLatestMarketSnapshotQuery(), cancellationToken);

            var response = _mapper.Map<MarketSnapshotResponseModel>(result);

            return Ok(response);
        }
    }
}