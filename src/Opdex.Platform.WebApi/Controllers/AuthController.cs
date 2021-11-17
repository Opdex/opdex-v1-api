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

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthConfiguration _authConfiguration;
        private readonly IMediator _mediator;

        public AuthController(AuthConfiguration authConfiguration, IMediator mediator)
        {
            _authConfiguration = authConfiguration ?? throw new ArgumentNullException(nameof(authConfiguration));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost]
        public IActionResult WalletCallback([FromQuery] StratisOpenAuthProtocolRequestQueryParameters query,
                                            [FromBody] StratisOpenAuthProtocolRequestBody body, CancellationToken cancellationToken)
        {
            if (query.Exp >= DateTime.UtcNow) return BadRequest("Signature expired.");

            // validate the signature
            
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
