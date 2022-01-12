namespace Opdex.Platform.WebApi.Models.Responses.Transactions;

/// <summary>
/// Error details for an Opdex transaction
/// </summary>
public class TransactionErrorResponseModel
{
    /// <summary>
    /// Friendly, human-readable error message
    /// </summary>
    public string Friendly { get; set; }

    /// <summary>
    /// Raw stack trace error returned by the full node for a failed smart contract transaction execution
    /// </summary>
    public string Raw { get; set; }
}
