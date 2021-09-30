using MediatR;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.WebApi.Auth;
using System.Net;
using System.Threading.Tasks;
using Opdex.Platform.Common.Models;

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

        /// <summary>Authorize</summary>
        /// <remarks>Authorizes access to a specific market</remarks>
        /// <param name="market">The market contract address to request access to</param>
        /// <param name="wallet">The wallet public key of the user</param>
        /// <returns>An access token</returns>
        [HttpPost("authorize")]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> Authorize([FromQuery] Address market, [FromQuery] Address wallet)
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
            var hasKey = Request.Headers.TryGetValue("OPDEX_ADMIN", out var adminKey);
            var validAdmin = hasKey && _authConfiguration.AdminKey == adminKey;
            if (validAdmin) tokenDescriptor.Subject.AddClaim(new Claim("admin", "true"));

            // Market is optional for admins, required otherwise
            if (!validAdmin && market == Address.Empty) return Unauthorized();

            // If the market is included, validate and include it
            if (market != Address.Empty)
            {
                // Throws NotFoundException if not found
                _ = await _mediator.Send(new GetMarketByAddressQuery(market));
                tokenDescriptor.Subject.AddClaim(new Claim("market", market.ToString()));
            }

            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            return new OkObjectResult(tokenHandler.WriteToken(jwt));
        }
    }
}
