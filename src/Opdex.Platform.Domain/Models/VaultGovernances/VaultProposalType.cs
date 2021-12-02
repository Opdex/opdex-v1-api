namespace Opdex.Platform.Domain.Models.VaultGovernances;

public enum VaultProposalType : byte
{
    Create = 1,
    Revoke = 2,
    PledgeMinimum = 3,
    ProposalMinimum = 4
}
