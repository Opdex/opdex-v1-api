using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Transactions([FromQuery] IEnumerable<string> contracts,
                                                      [FromQuery] IEnumerable<uint> includedEvents,
                                                      [FromQuery] IEnumerable<uint> excludedEvents,
                                                      [FromQuery] string wallet,
                                                      [FromQuery] uint limit,
                                                      [FromQuery] string direction,
                                                      [FromQuery] string next,
                                                      [FromQuery] string previous,
                                                      CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(new GetTransactionsWithFilterQuery(wallet, includedEvents, excludedEvents, contracts,
                                                    direction, limit, next, previous), cancellationToken);

            return Ok(response);
        }
    }
}
