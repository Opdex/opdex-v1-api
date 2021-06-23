using MediatR;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.WebApi.Auth;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<AuthConfiguration> _authConfiguration;
        private readonly IMediator _mediator;

        public AuthController(IOptions<AuthConfiguration> authConfiguration, IMediator mediator)
        {
            _authConfiguration = authConfiguration ?? throw new ArgumentNullException(nameof(authConfiguration));
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Authorizes access to a specific market
        /// </summary>
        /// <param name="market">The market contract address to request access to</param>
        /// <param name="wallet">The wallet public key of the user</param>
        /// <returns>An access token</returns>
        [HttpPost("authorize")]
        public async Task<IActionResult> Authorize([FromQuery] string market, [FromQuery] string wallet)
        {
            // Throws NotFoundException if not found
            var marketDto = await _mediator.Send(new GetMarketByAddressQuery(market));

            // Todo: If private market; roles && enforce wallet != null && wallet has permission

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authConfiguration.Value.Opdex.SigningKey));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("market", market)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
            };

            if (!string.IsNullOrEmpty(wallet))
            {
                tokenDescriptor.Subject.AddClaim(new Claim("wallet", wallet));
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return new OkObjectResult(tokenHandler.WriteToken(token));
        }
    }
}
