using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Opdex.Platform.WebApi.Models.Requests.Auth;

/// <summary>
/// Callback query parameters for Stratis Open Auth Protocol.
/// </summary>
public class StratisOpenAuthCallbackQuery
{
    /// <summary>
    /// The unique identifier of the Stratis ID.
    /// </summary>
    /// <example>4e8a8445762c491fa7c5cf74a0a745e5</example>
    [BindRequired]
    public string Uid { get; set; }

    /// <summary>
    ///  Unix timestamp indicating when the signature expires.
    /// </summary>
    /// <example>1637244295</example>
    public long Exp { get; set; }
}