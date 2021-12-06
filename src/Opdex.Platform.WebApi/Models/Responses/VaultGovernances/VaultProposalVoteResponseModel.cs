using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.VaultGovernances;

/// <summary>
/// Vault proposal vote details.
/// </summary>
public class VaultProposalVoteResponseModel
{
    /// <summary>
    /// Address of the vault.
    /// </summary>
    /// <example>tS1PEGC4VsovkDgib1MD3eYNv5BL2FAC3i</example>
    [NotNull]
    public Address Vault { get; set; }

    /// <summary>
    /// Id of the proposal stored by the vault.
    /// </summary>
    /// <example>5</example>
    [NotNull]
    [Range(1, double.MaxValue)]
    public ulong ProposalId { get; set; }

    /// <summary>
    /// Address of the voter.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    [NotNull]
    public Address Voter { get; set; }

    /// <summary>
    /// Amount of CRS voting on the proposal.
    /// </summary>
    /// <example>"500.00000000"</example>
    [NotNull]
    public FixedDecimal Vote { get; set; }

    /// <summary>
    /// If the vote is in favor of the proposal.
    /// </summary>
    /// <example>true</example>
    [NotNull]
    public bool InFavor { get; set; }
}
