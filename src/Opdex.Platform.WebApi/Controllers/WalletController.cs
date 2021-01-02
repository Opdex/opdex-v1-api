using System;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("wallet")]
    public class WalletController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public WalletController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
    }
}