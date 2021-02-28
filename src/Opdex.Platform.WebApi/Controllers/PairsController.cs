using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
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
        
        [HttpGet("{pairAddress}")]
        public async Task<ActionResult<PairResponseModel>> GetPair(string pairAddress, CancellationToken cancellationToken)
        {
            var response = Task.FromResult("Pair");
            
            return Ok(response);
        }
    }
}