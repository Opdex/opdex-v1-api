using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Governances;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("governances")]
    public class GovernancesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly IApplicationContext _context;

        public GovernancesController(IMediator mediator, IMapper mapper, IApplicationContext applicationContext)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _context = applicationContext ?? throw new ArgumentNullException(nameof(applicationContext));
        }

        [HttpGet("{address}")]
        [ProducesResponseType(typeof(MiningGovernanceResponseModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MiningGovernanceResponseModel>> Token(string address, CancellationToken cancellationToken)
        {
            var governanceDto = await _mediator.Send(new GetMiningGovernanceByAddressQuery(address), cancellationToken);

            var response = _mapper.Map<MiningGovernanceResponseModel>(governanceDto);

            return Ok(response);
        }
    }
}
