using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.Tokens;

/// <summary>
/// Request body to add a token.
/// </summary>
public class AddTokenRequest
{
    /// <summary>The address of the SRC token.</summary>
    [Required]
    public Address TokenAddress { get; set; }
}