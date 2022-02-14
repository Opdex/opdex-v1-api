using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Encryption;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Admins;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Auth;
using SSAS.NET;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.SignalR;

public class PlatformHub : Hub<IPlatformClient>
{
    private readonly IMediator _mediator;
    private readonly ITwoWayEncryptionProvider _twoWayEncryptionProvider;
    private readonly AuthConfiguration _authConfiguration;
    private readonly string _authCallback;

    public PlatformHub(IMediator mediator, ITwoWayEncryptionProvider twoWayEncryptionProvider, AuthConfiguration authConfiguration, OpdexConfiguration opdexConfiguration)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _twoWayEncryptionProvider = twoWayEncryptionProvider ?? throw new ArgumentNullException(nameof(twoWayEncryptionProvider));
        _authConfiguration = authConfiguration ?? throw new ArgumentNullException(nameof(authConfiguration));
        var created = Uri.TryCreate($"{opdexConfiguration.ApiUrl}/{authConfiguration.StratisSignatureAuth.CallbackPath}/callback", UriKind.Absolute, out var uri);
        if (!created) throw new Exception("Unable to create callback URI");
        _authCallback = $"{uri.Authority}{uri.AbsolutePath}";
    }

    /// <summary>
    /// Returns a Stratis ID that can be used for authentication.
    /// </summary>
    /// <returns>Stratis ID.</returns>
    public string GetStratisId()
    {
        var expiry = DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds();
        var encryptedConnectionId = _twoWayEncryptionProvider.Encrypt($"{Context.ConnectionId}{expiry}");
        var uid = Base64Extensions.UrlSafeBase64Encode(encryptedConnectionId);
        var stratisId = new StratisId(_authCallback, uid, expiry);
        return stratisId.ToString();
    }

    public async Task<bool> Reconnect(string previousConnectionId, string sid)
    {
        if (!StratisId.TryParse(sid, out var stratisId) || stratisId.Expired) return false;

        var unixTimeExpiry = stratisId.Expiry;

        var previousUid = Base64Extensions.UrlSafeBase64Encode(_twoWayEncryptionProvider.Encrypt(($"{previousConnectionId}{unixTimeExpiry}")));
        if (stratisId.Uid != previousUid) return false;

        // need to verify message was signed
        var authSuccess = await _mediator.Send(new SelectAuthSuccessByConnectionIdQuery(previousConnectionId));
        if (authSuccess is null || authSuccess.Expiry < DateTime.UtcNow) return false;

        // resend auth
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authConfiguration.Opdex.SigningKey));
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(),
            Expires = DateTime.UtcNow.AddHours(24),
            IssuedAt = DateTime.UtcNow,
            SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
        };

        tokenDescriptor.Subject.AddClaim(new Claim("wallet", authSuccess.Signer.ToString()));
        var admin = await _mediator.Send(new SelectAdminByAddressQuery(authSuccess.Signer, findOrThrow: false), CancellationToken.None);
        if (admin is not null) tokenDescriptor.Subject.AddClaim(new Claim("admin", "true"));

        var jwt = tokenHandler.CreateToken(tokenDescriptor);
        var bearerToken = tokenHandler.WriteToken(jwt);
        await Clients.Caller.OnAuthenticated(bearerToken);

        return true;
    }
}
