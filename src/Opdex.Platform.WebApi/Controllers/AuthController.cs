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
        /// Authorizes access to a specific market
        /// </summary>
        /// <param name="market">The market to request access to</param>
        /// <param name="wallet">The wallet address of the user</param>
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