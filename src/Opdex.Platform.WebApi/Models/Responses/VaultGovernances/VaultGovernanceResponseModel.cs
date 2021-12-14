using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.VaultGovernances;

/// <summary>
/// Vault details.
/// </summary>
public class VaultGovernanceResponseModel
{
    /// <summary>
    /// Address of the vault.
    /// </summary>
    /// <example>tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i</example>
    [NotNull]
    public Address Vault { get; set; }

    /// <summary>
    /// Address of the governance token.
    /// </summary>
    /// <example>tBeY2UAVsbHoS9jwEnT2kMmRsJwUHK1j1L</example>
    [NotNull]
    public Address Token { get; set; }

    /// <summary>
    /// Amount of governance tokens that can be put toward new proposals.
    /// </summary>
    /// <example>"1000000.00000000"</example>
    [NotNull]
    public FixedDecimal TokensUnassigned { get; set; }

    /// <summary>
    /// Amount of governance tokens currently locked for active proposals.
    /// </summary>
    /// <example>"1000000.00000000"</example>
    [NotNull]
    public FixedDecimal TokensProposed { get; set; }

    /// <summary>
    /// Total amount of governance tokens in the vault.
    /// </summary>
    /// <example>"5000000.00000000"</example>
    [NotNull]
    public FixedDecimal TokensLocked { get; set; }

    /// <summary>
    /// Minimum amount of CRS tokens required to be pledged, for a proposal to move to a vote.
    /// </summary>
    /// <example>"25000.00000000"</example>
    [NotNull]
    public FixedDecimal TotalPledgeMinimum { get; set; }

    /// <summary>
    /// Minimum amount of CRS tokens required to be voted with, for a proposal to be considered.
    /// </summary>
    /// <example>"200000.00000000"</example>
    [NotNull]
    public FixedDecimal TotalVoteMinimum { get; set; }

    /// <summary>
    /// Number of blocks that a certificate is vested for, before it can be redeemed.
    /// </summary>
    /// <example>250000</example>
    [NotNull]
    public ulong VestingDuration { get; set; }
}
