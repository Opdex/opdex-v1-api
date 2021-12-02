namespace Opdex.Platform.Common.Constants.SmartContracts;

public static class VaultGovernanceConstants
{
    public static class StateKeys
    {
        public const string VestingDuration = "VA";
        public const string Token = "VB";
        public const string TotalSupply = "VC";
        public const string TotalProposedAmount = "VD";
        public const string Certificate = "VE";
        public const string NextProposalId = "VF";
        public const string Proposal = "VG";
        public const string ProposalVote = "VH";
        public const string ProposalPledge = "VI";
        public const string PledgeMinimum = "VJ";
        public const string ProposalMinimum = "VK";
        public const string ProposalIdByRecipient = "VL";
    }

    public static class Properties
    {
        public const string Token = nameof(StateKeys.Token);
        public const string TotalSupply = nameof(StateKeys.TotalSupply);
        public const string VestingDuration = nameof(StateKeys.VestingDuration);
        public const string NextProposalId = nameof(StateKeys.NextProposalId);
        public const string TotalProposedAmount = nameof(StateKeys.TotalProposedAmount);
        public const string PledgeMinimum = nameof(StateKeys.PledgeMinimum);
        public const string ProposalMinimum = nameof(StateKeys.ProposalMinimum);
    }

    public static class Methods
    {
        public const string GetCertificate = nameof(GetCertificate);
        public const string GetProposal = nameof(GetProposal);
        public const string GetProposalVote = nameof(GetProposalVote);
        public const string GetProposalPledge = nameof(GetProposalPledge);
        public const string GetCertificateProposalIdByRecipient = nameof(GetCertificateProposalIdByRecipient);
        public const string NotifyDistribution = nameof(NotifyDistribution);
        public const string RedeemCertificate = nameof(RedeemCertificate);
        public const string CreateNewCertificateProposal = nameof(CreateNewCertificateProposal);
        public const string CreateRevokeCertificateProposal = nameof(CreateRevokeCertificateProposal);
        public const string CreatePledgeMinimumProposal = nameof(CreatePledgeMinimumProposal);
        public const string CreateProposalMinimumProposal = nameof(CreateProposalMinimumProposal);
        public const string Pledge = nameof(Pledge);
        public const string Vote = nameof(Vote);
        public const string WithdrawVote = nameof(WithdrawVote);
        public const string WithdrawPledge = nameof(WithdrawPledge);
        public const string CompleteProposal = nameof(CompleteProposal);
    }
}
