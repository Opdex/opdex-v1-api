using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Requests.Transactions;

/// <summary>
/// Request to notify a user that their transaction has been broadcast.
/// </summary>
public class TransactionBroadcastNotificationRequest
{
    /// <summary>
    /// SHA-256 transaction hash.
    /// </summary>
    /// <example>402aa2241adb7b04d07d4dbc89f8aae72fa1c11f9bd2bd9013222cd774ed39fe</example>
    public Sha256 TransactionHash { get; set; }
}
