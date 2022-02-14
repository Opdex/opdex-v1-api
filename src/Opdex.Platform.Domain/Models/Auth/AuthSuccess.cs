using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Domain.Models.Auth;

public class AuthSuccess
{
    public AuthSuccess(string connectionId, Address signer, DateTime expiry)
    {
        if (!connectionId.HasValue()) throw new ArgumentNullException(nameof(connectionId), "Connection Id must be provided.");
        if (signer.IsZero || signer == Address.Empty) throw new ArgumentNullException(nameof(signer), "Signer must be a valid network address.");
        if (expiry.Equals(default)) throw new ArgumentOutOfRangeException(nameof(expiry), "Expiry must be a valid date time.");

        ConnectionId = connectionId;
        Signer = signer;
        Expiry = expiry;
    }

    public string ConnectionId { get; }
    public Address Signer { get; }
    public DateTime Expiry { get; }
}
