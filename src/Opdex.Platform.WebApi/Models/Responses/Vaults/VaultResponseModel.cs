using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Vaults;

/// <summary>
/// Vault details.
/// </summary>
public class VaultResponseModel
{
    /// <summary>
    /// Address of the vault.
    /// </summary>
    /// <example>t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G</example>
    public Address Vault { get; set; }

    /// <summary>
    /// Address of the governance token.
    /// </summary>
    /// <example>tTTuKbCR2UnsEByXBp1ynBz91J2yz63h1c</example>
    public Address Token { get; set; }

    /// <summary>
    /// Amount of governance tokens that can be put toward new proposals.
    /// </summary>
    /// <example>"1000000.00000000"</example>
    public FixedDecimal TokensUnassigned { get; set; }

    /// <summary>
    /// Amount of governance tokens currently locked for active proposals.
    /// </summary>
    /// <example>"1000000.00000000"</example>
    public FixedDecimal TokensProposed { get; set; }

    /// <summary>
    /// Total amount of governance tokens in the vault.
    /// </summary>
    /// <example>"5000000.00000000"</example>
    public FixedDecimal TokensLocked { get; set; }

    /// <summary>
    /// Minimum amount of CRS tokens required to be pledged, for a proposal to move to a vote.
    /// </summary>
    /// <example>"25000.00000000"</example>
    public FixedDecimal TotalPledgeMinimum { get; set; }

    /// <summary>
    /// Minimum amount of CRS tokens required to be voted with, for a proposal to be considered.
    /// </summary>
    /// <example>"200000.00000000"</example>
    public FixedDecimal TotalVoteMinimum { get; set; }

    /// <summary>
    /// Number of blocks that a certificate is vested for, before it can be redeemed.
    /// </summary>
    /// <example>250000</example>
    public ulong VestingDuration { get; set; }

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
