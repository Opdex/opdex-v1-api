using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Mappers;

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
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
        if (!tryParseProcessor(rawError, out var friendlyError))
        {
            using (_logger.BeginScope(new Dictionary<string, object> { ["RawError"] = rawError }))
            {
                _logger.LogWarning("Unknown Opdex transaction error found");
            }
        }

        return friendlyError ?? "Unexpected error occurred.";
    }
}
