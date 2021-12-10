using Microsoft.AspNetCore.SignalR;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Encryption;
using Opdex.Platform.Common.Extensions;
using SSAS.NET;
using System;

namespace Opdex.Platform.Infrastructure.Clients.SignalR;

public class PlatformHub : Hub<IPlatformClient>
{
    private readonly ITwoWayEncryptionProvider _twoWayEncryptionProvider;
    private readonly string _authCallback;

    public PlatformHub(ITwoWayEncryptionProvider twoWayEncryptionProvider, AuthConfiguration authConfiguration, OpdexConfiguration opdexConfiguration)
    {
        _twoWayEncryptionProvider = twoWayEncryptionProvider ?? throw new ArgumentNullException(nameof(twoWayEncryptionProvider));
        var created = Uri.TryCreate($"{opdexConfiguration.ApiUrl}/{authConfiguration.StratisSignatureAuth.CallbackPath}", UriKind.Absolute, out var uri);
        if (!created) throw new Exception("Unable to create callback URI");
        _authCallback = $"{uri.Authority}{uri.AbsolutePath}";
    }

    /// <summary>
    /// Returns a Stratis ID that can be used for authentication.
    /// </summary>
    /// <returns>Stratis ID.</returns>
    public string GetStratisId()
    {
        var encryptedConnectionId = _twoWayEncryptionProvider.Encrypt(Context.ConnectionId);
        var uid = Base64Extensions.UrlSafeBase64Encode(encryptedConnectionId);
        var stratisId = new StratisId(_authCallback, uid, DateTimeOffset.UtcNow.AddMinutes(5).ToUnixTimeSeconds());
        return stratisId.ToString();
    }
}
