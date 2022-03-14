
namespace Opdex.Platform.WebApi.Models.Responses.Status;

/// <summary>
/// Status details for the application.
/// </summary>
public class StatusResponseModel
{
    /// <summary>
    /// Commit hash indicating the version of the running application.
    /// </summary>
    /// <example>92ec53dc8388bc835eae5a892b29cb9519de7d97</example>
    public string Commit { get; set; }

    /// <summary>
    /// Unique identifier for the running instance of the application.
    /// </summary>
    /// <example>ae89af2a-9774-4956-aa40-507edd38d200</example>
    public string Identifier { get; set; }

    /// <summary>
    /// Whether the API is under maintenance
    /// </summary>
    public bool UnderMaintenance { get; set; }
}
