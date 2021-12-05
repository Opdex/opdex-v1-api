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
using System.Threading.Tasks;
using Opdex.Platform.Application.Abstractions.EntryQueries.Admins;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Auth;
using Opdex.Platform.Common.Encryption;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Extensions;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;

namespace Opdex.Platform.WebApi.Controllers;

[ApiController]
[Route("auth")]
[ApiVersion("1.0")]
public class AuthController : ControllerBase
{
    private readonly OpdexConfiguration _opdexConfiguration;
    private readonly AuthConfiguration _authConfiguration;
    private readonly ILogger<AuthController> _logger;
    private readonly IMediator _mediator;
    private readonly ITwoWayEncryptionProvider _twoWayEncryptionProvider;

    public AuthController(OpdexConfiguration opdexConfiguration, AuthConfiguration authConfiguration, ILogger<AuthController> logger,
                          IMediator mediator, ITwoWayEncryptionProvider twoWayEncryptionProvider)
    {
        _opdexConfiguration = opdexConfiguration ?? throw new ArgumentNullException(nameof(opdexConfiguration));
        _authConfiguration = authConfiguration ?? throw new ArgumentNullException(nameof(authConfiguration));
        _logger = logger;
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _twoWayEncryptionProvider = twoWayEncryptionProvider ?? throw new ArgumentNullException(nameof(twoWayEncryptionProvider));
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
        var callbackUri = new Uri(System.IO.Path.Combine(_opdexConfiguration.ApiUrl, _authConfiguration.StratisOpenAuthProtocol.CallbackPath));
        var expectedCallbackPath = $"{callbackUri.Authority}{callbackUri.AbsolutePath}";
        var expectedId = new StratisId(expectedCallbackPath, query.Uid, query.Exp);

        if (expectedId.Expired) throw new InvalidDataException("exp", "Expiry exceeded.");

        var verified = await _mediator.Send(new CallCirrusVerifyMessageQuery(expectedId.Callback, body.PublicKey, body.Signature), cancellationToken);
        if (!verified) throw new InvalidDataException("signature", "Invalid signature.");

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

        string connectionId;
        try
        {
            connectionId = _twoWayEncryptionProvider.Decrypt(Base64Extensions.UrlSafeBase64Decode(expectedId.Uid));
        }
        catch (CryptographicException exception)
        {
            _logger.LogWarning(exception, "Invalid UID.");
            throw new InvalidDataException("uid", "Malformed UID.");
        }

        await _mediator.Send(new NotifyUserOfSuccessfulAuthenticationCommand(connectionId, bearerToken), cancellationToken);

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