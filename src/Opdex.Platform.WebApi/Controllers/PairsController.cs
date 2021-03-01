using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
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
        
        public PairsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        [HttpGet]
        public async Task<ActionResult<List<PairResponseModel>>> GetAllPairs(CancellationToken cancellationToken)
        {
            var query = new GetAllPairsQuery();
            
            var result = await _mediator.Send(query, cancellationToken);
            
            return Ok(result);
        }
        
        [HttpGet("{address}")]
        public async Task<ActionResult<PairResponseModel>> GetPair(string address, CancellationToken cancellationToken)
        {
            var query = new GetPairByAddressQuery(address);
            
            var result = await _mediator.Send(query, cancellationToken);
            
            return Ok(result);
        }
    }
}