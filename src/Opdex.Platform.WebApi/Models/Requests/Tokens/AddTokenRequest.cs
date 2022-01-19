using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Tokens;

/// <summary>
/// Request to begin tracking a token.
/// </summary>
public class AddTokenRequest
{
    /// <summary>Address of the SRC token.</summary>
    /// <example>tF83sdXXt2nTkL7UyEYDVFMK4jTuYMbmR3</example>
    [Required]
    public Address Token { get; set; }
}