using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Opdex.Platform.WebApi.Auth;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly IOptions<AuthConfiguration> _authConfiguration;

        public AuthController(IOptions<AuthConfiguration> authConfiguration)
        {
            _authConfiguration = authConfiguration ?? throw new ArgumentNullException(nameof(authConfiguration));
        }
        
        /// <summary>
        /// Authorizes access to a specific market
        /// </summary>
        /// <param name="market">The market contract address to request access to</param>
        /// <param name="wallet">The wallet public key of the user</param>
        /// <returns>An access token</returns>
        [HttpPost("authorize")]
        public IActionResult Authorize([FromQuery] string market, [FromQuery] string wallet)
        {
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