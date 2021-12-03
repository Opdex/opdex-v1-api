using System;

namespace Opdex.Platform.Common.Exceptions;

/// <summary>
/// An exception thrown for a request to index something that cannot be indexed.
/// </summary>
public class AlreadyIndexedException : Exception
{
    /// <summary>
    /// Creates a cannot invalid exception which specifies the reason.
    /// </summary>
    /// <param name="message">The reason why indexing cannot occur.</param>
    public AlreadyIndexedException(string message) : base(message)
    {
    }
}