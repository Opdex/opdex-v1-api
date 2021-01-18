using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Indexer.WebApi.Models;

namespace Opdex.Indexer.WebApi.Controllers
{
    [ApiController]
    [Route("index")]
    public class IndexController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public IndexController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> GetLastSyncedBlock(CancellationToken cancellationToken)
        {
            return Ok();
        }

        // Todo: Auth Attribute - InternalAccessOnly
        [HttpPut("reindex")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ReIndexBlocks(ReIndexRequest request, CancellationToken cancellationToken)
        {
            return NoContent();
        }
    }
}