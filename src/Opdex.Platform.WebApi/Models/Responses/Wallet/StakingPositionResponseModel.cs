using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Wallet;

/// <summary>
/// Details of a staking position for an address.
/// </summary>
public class StakingPositionResponseModel
{
    /// <summary>
    /// Address of the staker.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    public Address Address { get; set; }

    /// <summary>
    /// Amount of tokens staked.
    /// </summary>
    /// <example>"500.00000000"</example>
    public FixedDecimal Amount { get; set; }

    /// <summary>
    /// Address of the liquidity pool.
    /// </summary>
    /// <example>t8WntmWKiLs1BdzoqPGXmPAYzUTpPb3DBw</example>
    public Address LiquidityPool { get; set; }

    /// <summary>
    /// Address of the token used for staking.
    /// </summary>
    /// <example>tTTuKbCR2UnsEByXBp1ynBz91J2yz63h1c</example>
    public Address StakingToken { get; set; }
}