using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("market")]
    public class MarketController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public MarketController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public Task<IActionResult> GetMarketDetails(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}