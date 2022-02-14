using MediatR;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Threading;
using System.Threading.Tasks;
using Opdex.Platform.Application.Abstractions.EntryQueries.Admins;
using Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Auth;
using Opdex.Platform.Common.Encryption;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Extensions;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.Commands.Auth;
using Opdex.Platform.Domain.Models.Auth;
using SSAS.NET;

namespace Opdex.Platform.WebApi.Controllers;

[ApiController]
[Route("v{version:apiVersion}/auth")]
[ApiVersion("1")]
public class AuthController : ControllerBase
{
    private readonly AuthConfiguration _authConfiguration;
    private readonly ILogger<AuthController> _logger;
    private readonly IMediator _mediator;
    private readonly ITwoWayEncryptionProvider _twoWayEncryptionProvider;
    private readonly string _baseUri;

    public AuthController(OpdexConfiguration opdexConfiguration, AuthConfiguration authConfiguration, ILogger<AuthController> logger,
                          IMediator mediator, ITwoWayEncryptionProvider twoWayEncryptionProvider)
    {
        if (opdexConfiguration == null || !opdexConfiguration.ApiUrl.HasValue()) throw new ArgumentNullException(nameof(opdexConfiguration));
        _authConfiguration = authConfiguration ?? throw new ArgumentNullException(nameof(authConfiguration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _twoWayEncryptionProvider = twoWayEncryptionProvider ?? throw new ArgumentNullException(nameof(twoWayEncryptionProvider));
        _baseUri = $"{opdexConfiguration.ApiUrl}/{_authConfiguration.StratisSignatureAuth.CallbackPath}";
    }

    /// <summary>
    /// Get a new Stratis Id message to sign for authentication without web sockets.
    /// </summary>
    /// <returns>Stratis Id</returns>
    [HttpGet]
    public ActionResult<string> GetStratisId()
    {
        var expiry = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
        var uid = Base64Extensions.UrlSafeBase64Encode(_twoWayEncryptionProvider.Encrypt($"{Guid.NewGuid()}{expiry}"));
        var created = Uri.TryCreate(_baseUri, UriKind.Absolute, out var uri);

        if (!created) throw new Exception("Unable to create callback URI");

        var authCallback = $"{uri.Authority}{uri.AbsolutePath}";
        var stratisId = new StratisId(authCallback, uid, expiry);

        return Ok(stratisId.ToString());
    }

    /// <summary>
    /// Stratis signature auth endpoint for client authentication without web socket.
    /// </summary>
    /// <param name="query">The UID and Expiration query parameters.</param>
    /// <param name="body">The Stratis Id signature.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Bearer token</returns>
    [HttpPost]
    public async Task<ActionResult<string>> StratisSignatureAuth([FromQuery] StratisSignatureAuthCallbackQuery query,
                                                                 [FromBody] StratisSignatureAuthCallbackBody body,
                                                                 CancellationToken cancellationToken)
    {
        var created = Uri.TryCreate(_baseUri, UriKind.Absolute, out var callbackUri);

        if (!created) throw new Exception("Unable to create callback URI");

        (string _, string bearerToken) = await ValidateStratisSignature(callbackUri, query, body, cancellationToken);

        return Ok(bearerToken);
    }

    /// <summary>
    /// Stratis signature auth callback for client user authentication with web socket.
    /// </summary>
    /// <remarks>Responds to a request from a Stratis Signature Auth Signer.</remarks>
    /// <param name="query">Tne Stratis Signature Auth query string.</param>
    /// <param name="body">The Stratis Signature Auth body.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    [HttpPost]
    [Route("callback")]
    public async Task<IActionResult> StratisSignatureAuthCallback([FromQuery] StratisSignatureAuthCallbackQuery query,
                                                                  [FromBody] StratisSignatureAuthCallbackBody body,
                                                                  CancellationToken cancellationToken)
    {
        var created = Uri.TryCreate($"{_baseUri}/callback", UriKind.Absolute, out var callbackUri);

        if (!created) throw new Exception("Unable to create callback URI");

        (string connectionId, string bearerToken) = await ValidateStratisSignature(callbackUri, query, body, cancellationToken);

        await _mediator.Send(new MakeAuthSuccessCommand(new AuthSuccess(connectionId, body.PublicKey, DateTime.UtcNow.AddMinutes(1))), cancellationToken);
        await _mediator.Send(new NotifyUserOfSuccessfulAuthenticationCommand(connectionId, bearerToken), cancellationToken);

        return NoContent();
    }

    private async Task<(string, string)> ValidateStratisSignature(Uri callbackUri, StratisSignatureAuthCallbackQuery query,
                                                                  StratisSignatureAuthCallbackBody body, CancellationToken cancellationToken)
    {
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
            // Todo: This should technically be much lower once refresh tokens are implemented, maybe back to 1 hour.
            Expires = DateTime.UtcNow.AddHours(24),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        tokenDescriptor.Subject.AddClaim(new Claim("wallet", body.PublicKey));
        var admin = await _mediator.Send(new GetAdminByAddressQuery(body.PublicKey, findOrThrow: false), cancellationToken);
        if (admin != null) tokenDescriptor.Subject.AddClaim(new Claim("admin", "true"));

        var jwt = tokenHandler.CreateToken(tokenDescriptor);
        var bearerToken = tokenHandler.WriteToken(jwt);

        string connectionId;
        long expiration;

        try
        {
            var uid = _twoWayEncryptionProvider.Decrypt(Base64Extensions.UrlSafeBase64Decode(expectedId.Uid));
            const int unixTimestampLength = 10;
            connectionId = uid.Substring(0, uid.Length - unixTimestampLength);
            expiration = long.Parse(uid.Substring(uid.Length - unixTimestampLength));
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Invalid UID.");
            throw new InvalidDataException("uid", "Malformed UID.");
        }

        if (expiration != query.Exp) throw new InvalidDataException("exp", "Invalid expiration.");

        return (connectionId, bearerToken);
    }
}
