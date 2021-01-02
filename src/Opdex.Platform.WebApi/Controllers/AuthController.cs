using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public AuthController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Gets an Opdex client specific token based on referrer address.
        /// </summary>
        /// <remarks>This is necessary for any API access but is only for internal Opdex clients without any API limits.</remarks>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns></returns>
        [HttpGet("token")]
        public Task<IActionResult> GetClientToken(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Requests a timestamped message to be included in a client signed messaged for authentication.
        /// </summary>
        /// <param name="walletAddress">The wallet address to be authenticated.</param>
        /// <param name="cancellationToken">CancellationToken</param>
        /// <returns>message as string to be included in signed message</returns>
        [HttpGet("request/{walletAddress}")]
        public Task<IActionResult> RequestAuthMessage(string walletAddress, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Validates a client signed message and returns a valid JWT to be authed during each request.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        [HttpPost("validate")]
        public Task<IActionResult> ValidateMessageAuthWallet(string message, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}