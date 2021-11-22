using Microsoft.AspNetCore.SignalR;
using Opdex.Platform.Common.Encryption;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Clients.SignalR
{
    public class PlatformHub : Hub<IPlatformClient>
    {
        private readonly ITwoWayEncryptionProvider _twoWayEncryptionProvider;

        public PlatformHub(ITwoWayEncryptionProvider twoWayEncryptionProvider)
        {
            _twoWayEncryptionProvider = twoWayEncryptionProvider ?? throw new ArgumentNullException(nameof(twoWayEncryptionProvider));
        }

        /// <summary>
        /// Returns the connection id as an encrypted string.
        /// </summary>
        /// <returns>Encrypted connection id.</returns>
        public string GetEncryptedConnectionId()
        {
            var encryptedConnectionId = _twoWayEncryptionProvider.Encrypt(Context.ConnectionId);
            return Base64Extensions.UrlSafeBase64Encode(encryptedConnectionId);
        }
    }
}
