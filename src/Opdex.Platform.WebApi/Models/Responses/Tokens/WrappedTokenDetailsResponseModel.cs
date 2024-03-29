using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens;

/// <summary>
/// Details of the token that is wrapped
/// </summary>
public class WrappedTokenDetailsResponseModel
{
    /// <summary>
    /// Cirrus address of the custodian that provides the bridge
    /// </summary>
    public Address Custodian { get; set; }

    /// <summary>
    /// Chain which the token is wrapped from
    /// </summary>
    public ExternalChainType Chain { get; set; }

    /// <summary>
    /// Address of the token on the external chain, or null if the token is the base token of the external chain
    /// </summary>
    public string Address { get; set; }

    /// <summary>
    /// True if the custodian is configured as trusted; otherwise false.
    /// </summary>
    public bool Trusted { get; set; }

    /// <summary>
    /// True if the native token is seen as valid; otherwise false.
    /// </summary>
    public bool Validated { get; set; }

    /// <summary>
    /// Block number at which the entity state was created.
    /// </summary>
    public ulong CreatedBlock { get; set; }

    /// <summary>
    /// Block number at which the entity state was last modified.
    /// </summary>
    public ulong ModifiedBlock { get; set; }
}
