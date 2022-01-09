namespace Opdex.Platform.Domain.Models.Vaults;

public enum VaultProposalType : byte
{
    Create = 1,
    Revoke = 2,
    TotalPledgeMinimum = 3,
    TotalVoteMinimum = 4
}
