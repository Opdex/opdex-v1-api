using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Domain.Models.Auth;

public class AuthSuccess
{
    public AuthSuccess(string connectionId, Address signer, DateTime expiry)
    {
        ConnectionId = connectionId;
        Signer = signer;
        Expiry = expiry;
    }
    
    public string ConnectionId { get; }
    public Address Signer { get; }
    public DateTime Expiry { get; }
}
