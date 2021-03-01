using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Opdex.Core.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.WebApi.Models;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("tokens")]
    public class TokensController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public TokensController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<ActionResult<List<TokenResponseModel>>> GetTokens(CancellationToken cancellationToken)
        {
            var query = new GetAllTokensQuery();
            
            var result = await _mediator.Send(query, cancellationToken);
            
            return Ok(result);
        }
        
        [HttpGet("{address}")]
        public async Task<ActionResult<TokenResponseModel>> GetToken(string address, CancellationToken cancellationToken)
        {
            var query = new GetTokenByAddressQuery(address);
            
            var result = await _mediator.Send(query, cancellationToken);
            
            return Ok(result);
        }
    }
}