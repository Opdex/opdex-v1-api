using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Auth;

public class AuthSuccessEntity
{
    public string ConnectionId { get; set; }
    public Address Signer { get; set; }
    public DateTime Expiry { get; set; }
}
