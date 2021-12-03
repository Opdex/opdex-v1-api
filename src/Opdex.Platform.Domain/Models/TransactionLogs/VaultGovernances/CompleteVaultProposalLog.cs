using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;

public class CompleteVaultProposalLog : TransactionLog
{
    public CompleteVaultProposalLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.CompleteVaultProposalLog, address, sortOrder)
    {
        ulong proposalId = (ulong)log?.proposalId;
        bool approved = (bool)log?.approved;

        if (proposalId < 1) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than 0.");

        ProposalId = proposalId;
        Approved = approved;
    }

    public CompleteVaultProposalLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.CompleteVaultProposalLog, id, transactionId, address, sortOrder)
    {
        var logDetails = DeserializeLogDetails(details);
        ProposalId = logDetails.ProposalId;
        Approved = logDetails.Approved;
    }

    public ulong ProposalId { get; }
    public bool Approved { get; }

    private struct LogDetails
    {
        public ulong ProposalId { get; set; }
        public bool Approved { get; set; }
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
            Approved = Approved
        });
    }
}
