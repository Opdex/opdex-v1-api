using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Extensions;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers;

/// <summary>
/// Processor to log and parse transaction errors into user-friendly messages.
/// </summary>
internal class TransactionErrorProcessor
{
    private readonly ILogger<TransactionErrorProcessor> _logger;
    private delegate bool TryParseFriendlyErrorMessage(string rawError, out string friendlyError);

    /// <summary>
    /// Creates a processor to log and parse transaction errors into user-friendly messages.
    /// </summary>
    public TransactionErrorProcessor(ILogger<TransactionErrorProcessor> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Logs any transaction errors that may be thrown from Opdex method calls.
    /// </summary>
    /// <param name="rawError">Raw transaction error.</param>
    /// <returns>Friendly error message.</returns>
    public string ProcessOpdexTransactionError(string rawError)
    {
        return ProcessError(rawError, TransactionErrors.Opdex.TryParseFriendlyErrorMessage);
    }

    private string ProcessError(string rawError, TryParseFriendlyErrorMessage tryParseProcessor)
    {
        var isKnownError = false;
        var errorProperties = new Dictionary<string, object>()
        {
            ["RawError"] = rawError
        };

        if (tryParseProcessor(rawError, out var friendlyError))
        {
            isKnownError = true;
            errorProperties.Add("Error", friendlyError);
        }

        errorProperties.Add("Known", isKnownError);

        using (_logger.BeginScope(errorProperties))
        {
            _logger.LogWarning("Transaction error occurred.");
        }

        return friendlyError ?? "Unexpected error occurred.";
    }
}