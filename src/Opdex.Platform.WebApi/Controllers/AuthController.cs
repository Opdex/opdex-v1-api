using MediatR;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.WebApi.Auth;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Opdex.Platform.WebApi.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthConfiguration _authConfiguration;
        private readonly OpdexConfiguration _opdexConfiguration;
        private readonly IMediator _mediator;

        public AuthController(AuthConfiguration authConfiguration, OpdexConfiguration opdexConfiguration, IMediator mediator)
        {
            _authConfiguration = authConfiguration ?? throw new ArgumentNullException(nameof(authConfiguration));
            _opdexConfiguration = opdexConfiguration ?? throw new ArgumentNullException(nameof(opdexConfiguration));
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
        public async Task<IActionResult> Authorize([FromQuery] string market, [FromQuery] string wallet)
        {
            // Throws NotFoundException if not found
            var marketDto = await _mediator.Send(new GetMarketByAddressQuery(market));

            await ValidateWallet(wallet, marketDto.Id);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authConfiguration.Opdex.SigningKey));
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

            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            return new OkObjectResult(tokenHandler.WriteToken(jwt));
        }

        // Todo: If private market; roles && enforce wallet != null && wallet has permission
        private async Task ValidateWallet(string wallet, long marketId)
        {
            // Devnet validate the wallet signing in has a balance,
            // This is a hack to be able to remove the firewall and still close off the api by wallet address without adding new tables
            // When we have stratis identity and stratis wallet details, there will be two flows, one for devnet and one for test/mainnet
            // At that time, consider creating queries and commands to whitelist devnet addresses
            if (_opdexConfiguration.Network == NetworkType.DEVNET)
            {
                if (!wallet.HasValue()) throw new BadRequestException("Wallet address must be provided.");

                var validWallet = false;
                var tokens = await _mediator.Send(new RetrieveTokensWithFilterQuery(marketId, false, 0, 5, "Name", "ASC", new string[0]));

                foreach (var token in tokens.Where(t => t.Address != TokenConstants.Cirrus.Address))
                {
                    try
                    {
                        var balance = await _mediator.Send(new GetAddressBalanceByTokenQuery(wallet, token.Address));

                        if (balance.Balance == "0") continue;

                        validWallet = true;

                        break;
                    }
                    catch { }
                }

                if (!validWallet) throw new AuthenticationException("Invalid wallet address");
            }
        }
    }
}
