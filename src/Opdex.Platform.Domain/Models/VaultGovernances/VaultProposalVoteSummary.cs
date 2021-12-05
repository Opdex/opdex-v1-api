namespace Opdex.Platform.Domain.Models.VaultGovernances;

public class VaultProposalVoteSummary
{
    public VaultProposalVoteSummary(bool inFavor, ulong amount)
    {
        InFavor = inFavor;
        Amount = amount;
    }

    public bool InFavor { get; }
    public ulong Amount { get; }
}
