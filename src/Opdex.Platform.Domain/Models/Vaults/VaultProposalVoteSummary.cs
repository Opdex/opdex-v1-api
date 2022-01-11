namespace Opdex.Platform.Domain.Models.Vaults;

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
