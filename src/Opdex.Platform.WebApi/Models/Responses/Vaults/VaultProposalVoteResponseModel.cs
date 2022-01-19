using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Vaults;

/// <summary>
/// Vault proposal vote details.
/// </summary>
public class VaultProposalVoteResponseModel
{
    /// <summary>
    /// Address of the vault.
    /// </summary>
    /// <example>t7hy4H51KzU6PPCL4QKCdgBGPLV9Jpmf9G</example>
    public Address Vault { get; set; }

    /// <summary>
    /// Id of the proposal stored by the vault.
    /// </summary>
    /// <example>5</example>
    public ulong ProposalId { get; set; }

    /// <summary>
    /// Address of the voter.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    public Address Voter { get; set; }

    /// <summary>
    /// Amount of CRS voted on the proposal. This will not change once the vote period has finished.
    /// </summary>
    /// <example>"500.00000000"</example>
    public FixedDecimal Vote { get; set; }

    /// <summary>
    /// Currently voting CRS balance.
    /// </summary>
    /// <example>"0.00000000"</example>
    public FixedDecimal Balance { get; set; }

    /// <summary>
    /// If the vote is in favor of the proposal.
    /// </summary>
    /// <example>true</example>
    public bool InFavor { get; set; }
}
