using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;

public class VaultProposalWithdrawPledgeLog : TransactionLog
{
    public VaultProposalWithdrawPledgeLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.VaultProposalWithdrawPledgeLog, address, sortOrder)
    {
        ulong proposalId = (ulong)log?.proposalId;
        Address pledger = (string)log?.pledger;
        ulong withdrawAmount = (ulong)log?.withdrawAmount;
        ulong pledgerAmount = (ulong)log?.pledgerAmount;
        ulong proposalPledgeAmount = (ulong)log?.proposalPledgeAmount;
        bool pledgeWithdrawn = (bool)log?.pledgeWithdrawn;

        if (proposalId < 1) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than 0.");
        if (pledger == Address.Empty) throw new ArgumentNullException(nameof(pledger), "Pledger must be set.");

        ProposalId = proposalId;
        Pledger = pledger;
        WithdrawAmount = withdrawAmount;
        PledgerAmount = pledgerAmount;
        ProposalPledgeAmount = proposalPledgeAmount;
        PledgeWithdrawn = pledgeWithdrawn;
    }

    public VaultProposalWithdrawPledgeLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.VaultProposalWithdrawPledgeLog, id, transactionId, address, sortOrder)
    {
        var logDetails = DeserializeLogDetails(details);
        ProposalId = logDetails.ProposalId;
        Pledger = logDetails.Pledger;
        WithdrawAmount = logDetails.WithdrawAmount;
        PledgerAmount = logDetails.PledgerAmount;
        ProposalPledgeAmount = logDetails.ProposalPledgeAmount;
        PledgeWithdrawn = logDetails.PledgeWithdrawn;
    }

    public ulong ProposalId { get; }
    public Address Pledger { get; }
    public ulong WithdrawAmount { get; }
    public ulong PledgerAmount { get; }
    public ulong ProposalPledgeAmount { get; }
    public bool PledgeWithdrawn { get; }

    private struct LogDetails
    {
        public ulong ProposalId { get; set; }
        public Address Pledger { get; set; }
        public ulong WithdrawAmount { get; set; }
        public ulong PledgerAmount { get; set; }
        public ulong ProposalPledgeAmount { get; set; }
        public bool PledgeWithdrawn { get; set; }
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
            WithdrawAmount = WithdrawAmount,
            PledgerAmount = PledgerAmount,
            ProposalPledgeAmount = ProposalPledgeAmount,
            PledgeWithdrawn = PledgeWithdrawn
        });
    }
}
