using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Core.Application.Abstractions.EntryQueries.Pairs;
using Opdex.Platform.Application.Abstractions.EntryQueries.Pairs;
using Opdex.Platform.WebApi.Models;


namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("pairs")]
    public class PairsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public PairsController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        /// <summary>
        /// Get a list of all available pairs
        /// </summary>
        /// <remarks>
        /// To be updated to include pagination and filtering
        /// </remarks>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>List of pairs</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PairResponseModel>>> GetAllPairs(CancellationToken cancellationToken)
        {
            var query = new GetAllPairsQuery();
            
            var result = await _mediator.Send(query, cancellationToken);

            var response = _mapper.Map<IEnumerable<PairResponseModel>>(result);
            
            return Ok(response);
        }
        
        /// <summary>
        /// Returns the pair that matches the provided address.
        /// </summary>
        /// <param name="address">Contract address to get pair of</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>The requested pair</returns>
        [HttpGet("{address}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PairResponseModel>> GetPair(string address, CancellationToken cancellationToken)
        {
            var query = new GetPairByAddressQuery(address);
            
            var result = await _mediator.Send(query, cancellationToken);
            
            var response = _mapper.Map<PairResponseModel>(result);
            
            return Ok(response);
        }
    }
}