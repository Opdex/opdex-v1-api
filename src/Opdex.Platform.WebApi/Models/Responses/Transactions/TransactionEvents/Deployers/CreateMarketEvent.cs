using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Deployers;

/// <summary>
/// Create market event.
/// </summary>
public class CreateMarketEvent : TransactionEvent
{
    /// <summary>
    /// Address of the market.
    /// </summary>
    /// <example>t7RorA7xQCMVYKPM1ibPE1NSswaLbpqLQb</example>
    public Address Market { get; set; }

    /// <summary>
    /// Address of the market owner.
    /// </summary>
    /// <example>tHYHem7cLKgoLkeb792yn4WayqKzLrjJak</example>
    public Address Owner { get; set; }

    /// <summary>
    /// Address of the market router.
    /// </summary>
    /// <example>tAFxpxRdcV9foADqD6gK3c8sY5MeANzFp5</example>
    public Address Router { get; set; }

    /// <summary>
    /// If the market authenticates pool creators.
    /// </summary>
    /// <example>false</example>
    public bool AuthPoolCreators { get; set; }

    /// <summary>
    /// If the market authenticates liquidity providers.
    /// </summary>
    /// <example>false</example>
    public bool AuthProviders { get; set; }

    /// <summary>
    /// If the market authenticates traders.
    /// </summary>
    /// <example>false</example>
    public bool AuthTraders { get; set; }

    /// <summary>
    /// Market transaction fee. This is a value between 0-10 which correlates to a fee of between 0-1%.
    /// </summary>
    /// <example>3</example>
    public uint TransactionFee { get; set; }

    /// <summary>
    /// Address of the staking token.
    /// </summary>
    /// <example>tTTuKbCR2UnsEByXBp1ynBz91J2yz63h1c</example>
    public Address StakingToken { get; set; }

    /// <summary>
    /// If the transaction fee for the market is enabled.
    /// </summary>
    /// <example>true</example>
    public bool EnableMarketFee { get; set; }
}