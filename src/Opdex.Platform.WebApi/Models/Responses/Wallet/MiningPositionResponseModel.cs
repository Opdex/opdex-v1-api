using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Wallet;

/// <summary>
/// Details of a mining position for an address.
/// </summary>
public class MiningPositionResponseModel
{
    /// <summary>
    /// Address of the miner.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    public Address Address { get; set; }

    /// <summary>
    /// Amount of mining tokens.
    /// </summary>
    /// <example>"500.00000000"</example>
    public FixedDecimal Amount { get; set; }

    /// <summary>
    /// Address of the mining pool.
    /// </summary>
    /// <example>tNgQhNxvachxKGvRonk2S8nrpYi44carYv</example>
    public Address MiningPool { get; set; }

    /// <summary>
    /// Address of the token used for mining.
    /// </summary>
    /// <example>tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L</example>
    public Address MiningToken { get; set; }

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