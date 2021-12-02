using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Domain.Models.VaultGovernances;

/// <summary>
/// Vault that CRS holders can vote on proposals for the allocation of vested certificates. Certificates entitle the holder to Opdex governance upon completion of the vesting period.
/// </summary>
public class VaultGovernance : BlockAudit
{
    public VaultGovernance(Address address, ulong tokenId, ulong vestingDuration, ulong pledgeMinimum, ulong proposalMinimum, ulong createdBlock)
        : base(createdBlock)
    {
        Address = address != Address.Empty ? address : throw new ArgumentNullException(nameof(address), "Address must be set.");
        TokenId = tokenId > 0 ? tokenId : throw new ArgumentOutOfRangeException(nameof(tokenId), "TokenId must be greater than zero.");
        VestingDuration = vestingDuration > 0 ? vestingDuration : throw new ArgumentOutOfRangeException(nameof(vestingDuration), "Vesting duration must be greater than zero.");
        PledgeMinimum = pledgeMinimum;
        ProposalMinimum = proposalMinimum;
    }

    public VaultGovernance(ulong id, Address address, ulong tokenId, UInt256 unassignedSupply, ulong vestingDuration, UInt256 proposedSupply,
                           ulong pledgeMinimum, ulong proposalMinimum, ulong createdBlock, ulong modifiedBlock) : base(createdBlock, modifiedBlock)
    {
        Id = id;
        Address = address;
        TokenId = tokenId;
        UnassignedSupply = unassignedSupply;
        VestingDuration = vestingDuration;
        ProposedSupply = proposedSupply;
        PledgeMinimum = pledgeMinimum;
        ProposalMinimum = proposalMinimum;
    }

    public ulong Id { get; }
    public Address Address { get; }
    public ulong TokenId { get; }
    public UInt256 UnassignedSupply { get; }
    public ulong VestingDuration { get; }
    public UInt256 ProposedSupply { get; }
    public ulong PledgeMinimum { get; }
    public ulong ProposalMinimum { get; }
}
