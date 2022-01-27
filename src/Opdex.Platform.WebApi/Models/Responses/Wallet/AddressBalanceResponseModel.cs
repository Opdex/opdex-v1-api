using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Wallet;

/// <summary>
/// Details of a token balance for an address.
/// </summary>
public class AddressBalanceResponseModel
{
    /// <summary>
    /// Amount of tokens.
    /// </summary>
    /// <example>"10000.00000000"</example>
    public FixedDecimal Balance { get; set; }

    /// <summary>
    /// Address of the token balance holder.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    public Address Address { get; set; }

    /// <summary>
    /// Address of the SRC token.
    /// </summary>
    /// <example>t8WntmWKiLs1BdzoqPGXmPAYzUTpPb3DBw</example>
    public Address Token { get; set; }

    /// <summary>
    /// Block number at which the entity was created.
    /// </summary>
    /// <example>2500000</example>
    public ulong CreatedBlock { get; set; }

    /// <summary>
    /// Block number at which the entity state was last modified.
    /// </summary>
    /// <example>3000000</example>
    public ulong ModifiedBlock { get; set; }
}