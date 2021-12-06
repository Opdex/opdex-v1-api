using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Responses.VaultGovernances;

/// <summary>
/// Vault proposal details.
/// </summary>
public class VaultProposalResponseModel
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
    /// Id of the proposal stored by the vault.
    /// </summary>
    /// <example>5</example>
    [NotNull]
    [Range(1, double.MaxValue)]
    public ulong ProposalId { get; set; }

    /// <summary>
    /// Address of the proposal creator.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    [NotNull]
    public Address Creator { get; set; }

    /// <summary>
    /// Address of the beneficiary if it's a certificate proposal, otherwise the proposal creator.
    /// </summary>
    /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
    [NotNull]
    public Address Wallet { get; set; }

    /// <summary>
    /// Proposal amount. For a certificate proposal, this is the governance token amount. For a minimum amount change proposal, this is the proposed CRS minimum.
    /// </summary>
    /// <example>"2500000.00000000"</example>
    [NotNull]
    public FixedDecimal Amount { get; set; }

    /// <summary>
    /// Description for the proposal.
    /// </summary>
    /// <example>OCP-1: Request for certificate. See https://www.example.com for details.</example>
    [NotNull]
    [MaxLength(200)]
    public string Description { get; set; }

    /// <summary>
    /// Proposal type.
    /// </summary>
    /// <example>Create</example>
    [NotNull]
    public VaultProposalType Type { get; set; }

    /// <summary>
    /// Status of the proposal.
    /// </summary>
    /// <example>Vote</example>
    [NotNull]
    public VaultProposalStatus Status { get; set; }

    /// <summary>
    /// Block number that the proposal status expires, for an active proposal.
    /// </summary>
    /// <example>500000</example>
    [NotNull]
    public ulong Expiration { get; set; }

    /// <summary>
    /// Yes vote CRS weight.
    /// </summary>
    /// <example>"500000.00000000"</example>
    [NotNull]
    public FixedDecimal YesAmount { get; set; }

    /// <summary>
    /// No vote CRS weight.
    /// </summary>
    /// <example>"200000.00000000"</example>
    public FixedDecimal NoAmount { get; set; }

    /// <summary>
    /// Pledged CRS amount.
    /// </summary>
    /// <example>"25000.00000000"</example>
    [NotNull]
    public FixedDecimal PledgeAmount { get; set; }

    /// <summary>
    /// If the proposal has been approved.
    /// </summary>
    /// <example>false</example>
    [NotNull]
    public bool Approved { get; set; }
}
