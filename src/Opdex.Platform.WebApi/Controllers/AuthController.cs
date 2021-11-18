using MediatR;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Opdex.Platform.WebApi.Auth;
using System.Net;
using Opdex.Platform.Common.Models;
using System.Threading;
using Opdex.Platform.WebApi.Models.Requests.Auth;
using Microsoft.AspNetCore.Http;
using Opdex.Platform.Common.Exceptions;
using System.Threading.Tasks;
using Opdex.Platform.Application.Abstractions.EntryQueries.Admins;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route(AuthPath)]
    public class AuthController : ControllerBase
    {
        private const string AuthPath = "auth";

        private readonly AuthConfiguration _authConfiguration;
        private readonly IMediator _mediator;

        public AuthController(AuthConfiguration authConfiguration, IMediator mediator)
        {
            _authConfiguration = authConfiguration ?? throw new ArgumentNullException(nameof(authConfiguration));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Stratis Open Auth Protocol
        /// </summary>
        /// <remarks>Responds to a request from a Stratis Open Auth Signer.</remarks>
        /// <param name="query">Tne Stratis Open Auth query string.</param>
        /// <param name="body">The Stratis Open Auth body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Signature was validated successfully.</response>
        /// <response code="400">The request is not valid.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StratisOpenAuthCallback([FromQuery] StratisOpenAuthCallbackQuery query,
                                                                 [FromBody] StratisOpenAuthCallbackBody body, CancellationToken cancellationToken)
        {
            var expectedCallbackPath = System.IO.Path.Combine(_authConfiguration.StratisOpenAuthProtocol.CallbackBase, AuthPath);
            var expectedId = new StratisId(expectedCallbackPath, query.Uid, query.Exp);

            if (expectedId.Expired) throw new InvalidDataException("exp", "Expiry exceeded.");

            // if (!Message.Verify(expectedId.Callback, body.PublicKey.ToString(), body.Signature)) throw new InvalidDataException("Invalid signature.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authConfiguration.Opdex.SigningKey));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                Expires = DateTime.UtcNow.AddHours(1),
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            tokenDescriptor.Subject.AddClaim(new Claim("wallet", body.PublicKey.ToString()));
            var admin = await _mediator.Send(new GetAdminByAddressQuery(body.PublicKey, findOrThrow: false), cancellationToken);
            if (admin != null) tokenDescriptor.Subject.AddClaim(new Claim("admin", "true"));

            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            var bearerToken = tokenHandler.WriteToken(jwt);

            await _mediator.Send(new NotifyUserOfSuccessfulAuthenticationCommand(Guid.Parse(expectedId.Uid), bearerToken));
            
            return Ok();
        }

        /// <summary>Authorize</summary>
        /// <remarks>Authorizes access to a specific market</remarks>
        /// <param name="wallet">The wallet public key of the user</param>
        /// <returns>An access token</returns>
        [HttpPost("authorize")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public IActionResult Authorize([FromQuery] Address wallet)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authConfiguration.Opdex.SigningKey));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(),
                Expires = DateTime.UtcNow.AddHours(1),
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            if (wallet != Address.Empty)
            {
                tokenDescriptor.Subject.AddClaim(new Claim("wallet", wallet.ToString()));
            }

            // Validate Admin
            if (Request.Headers.TryGetValue("OPDEX_ADMIN", out var adminKey))
            {
                if (_authConfiguration.AdminKey == adminKey) tokenDescriptor.Subject.AddClaim(new Claim("admin", "true"));
                else return Unauthorized();
            }

            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            return new OkObjectResult(tokenHandler.WriteToken(jwt));
        }
    }
}
