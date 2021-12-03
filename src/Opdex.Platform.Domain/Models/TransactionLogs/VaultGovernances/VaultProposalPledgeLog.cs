using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;

public class VaultProposalPledgeLog : TransactionLog
{
    public VaultProposalPledgeLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.VaultProposalPledgeLog, address, sortOrder)
    {
        ulong proposalId = (ulong)log?.proposalId;
        Address pledger = (string)log?.pledger;
        ulong pledgeAmount = (ulong)log?.pledgeAmount;
        ulong pledgerAmount = (ulong)log?.pledgerAmount;
        ulong proposalPledgeAmount = (ulong)log?.proposalPledgeAmount;
        bool pledgeMinimumMet = (bool)log?.pledgeMinimumMet;

        if (proposalId < 1) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than 0.");
        if (pledger == Address.Empty) throw new ArgumentNullException(nameof(pledger), "Pledger must be set.");
        if (pledgeAmount == 0) throw new ArgumentOutOfRangeException(nameof(pledgeAmount), "Pledge amount must be greater than zero.");
        if (pledgerAmount < pledgeAmount) throw new ArgumentOutOfRangeException(nameof(pledgerAmount), "Pledger amount must be greater than or equal to pledge amount.");
        if (proposalPledgeAmount < pledgerAmount) throw new ArgumentOutOfRangeException(nameof(proposalPledgeAmount), "Proposal pledge amount must be greater than or equal to pledger amount.");

        ProposalId = proposalId;
        Pledger = pledger;
        PledgeAmount = pledgeAmount;
        PledgerAmount = pledgerAmount;
        ProposalPledgeAmount = proposalPledgeAmount;
        PledgeMinimumMet = pledgeMinimumMet;
    }

    public VaultProposalPledgeLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.VaultProposalPledgeLog, id, transactionId, address, sortOrder)
    {
        var logDetails = DeserializeLogDetails(details);
        ProposalId = logDetails.ProposalId;
        Pledger = logDetails.Pledger;
        PledgeAmount = logDetails.PledgeAmount;
        PledgerAmount = logDetails.PledgerAmount;
        ProposalPledgeAmount = logDetails.ProposalPledgeAmount;
        PledgeMinimumMet = logDetails.PledgeMinimumMet;
    }

    public ulong ProposalId { get; }
    public Address Pledger { get; }
    public ulong PledgeAmount { get; }
    public ulong PledgerAmount { get; }
    public ulong ProposalPledgeAmount { get; }
    public bool PledgeMinimumMet { get; }

    private struct LogDetails
    {
        public ulong ProposalId { get; set; }
        public Address Pledger { get; set; }
        public ulong PledgeAmount { get; set; }
        public ulong PledgerAmount { get; set; }
        public ulong ProposalPledgeAmount { get; set; }
        public bool PledgeMinimumMet { get; set; }
    }

    private static LogDetails DeserializeLogDetails(string details)
    {
        return JsonConvert.DeserializeObject<LogDetails>(details);
    }

    public override string SerializeLogDetails()
    {
        return JsonConvert.SerializeObject(new LogDetails
        {
            ProposalId = ProposalId,
            Pledger = Pledger,
            PledgeAmount = PledgeAmount,
            PledgerAmount = PledgerAmount,
            ProposalPledgeAmount = ProposalPledgeAmount,
            PledgeMinimumMet = PledgeMinimumMet
        });
    }
}
