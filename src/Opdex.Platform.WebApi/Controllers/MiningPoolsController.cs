using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pools;
using Opdex.Platform.WebApi.Models.Responses.Pools;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("mining-pools")]
    public class MiningPoolsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public MiningPoolsController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Retrieves mining pool details.
        /// </summary>
        /// <param name="address">Address of the mining pool.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Mining pool details.</returns>
        [HttpGet("{address}")]
        [ProducesResponseType(typeof(MiningPoolResponseModel), StatusCodes.Status200OK)]
        public async Task<ActionResult<MiningPoolResponseModel>> GetMiningPool(string address, CancellationToken cancellationToken)
        {
            var dto = await _mediator.Send(new GetMiningPoolByAddressQuery(address), cancellationToken);
            var response = _mapper.Map<MiningPoolResponseModel>(dto);
            return Ok(response);
        }
    }
}
