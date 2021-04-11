using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Transactions;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("transactions")]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public Task<IActionResult> GetAllTransactions(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}