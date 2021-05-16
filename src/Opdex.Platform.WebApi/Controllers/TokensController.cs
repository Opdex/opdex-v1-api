using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.WebApi.Models;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Authorize]
    [Route("tokens")]
    public class TokensController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public TokensController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Get a list of all available tokens
        /// </summary>
        /// <remarks>
        /// To be updated to include pagination and filtering
        /// </remarks>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>List of tokens</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<TokenResponseModel>>> GetTokens(CancellationToken cancellationToken)
        {
            var query = new GetAllTokensQuery();

            var result = await _mediator.Send(query, cancellationToken);

            var response = _mapper.Map<IEnumerable<TokenResponseModel>>(result);

            return Ok(response);
        }

        /// <summary>
        /// Returns the token that matches the provided address.
        /// </summary>
        /// <param name="address">Contract address to get token of</param>
        /// <param name="cancellationToken">cancellation token</param>
        /// <returns>The requested token</returns>
        [HttpGet("{address}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TokenResponseModel>> GetToken(string address, CancellationToken cancellationToken)
        {
            var query = new GetTokenByAddressQuery(address);

            var result = await _mediator.Send(query, cancellationToken);

            var response = _mapper.Map<TokenResponseModel>(result);

            return Ok(response);
        }

        [HttpGet("{address}/transactions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public Task<ActionResult<TokenResponseModel>> GetTransasctionsForToken(string address, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}