using Newtonsoft.Json;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.VaultGovernances;
using System;

namespace Opdex.Platform.Domain.Models.TransactionLogs.VaultGovernances;

public class CreateVaultProposalLog : TransactionLog
{
    public CreateVaultProposalLog(dynamic log, Address address, int sortOrder)
            : base(TransactionLogType.CreateVaultProposalLog, address, sortOrder)
    {
        ulong proposalId = (ulong)log?.proposalId;
        Address wallet = (string)log?.wallet;
        UInt256 amount = UInt256.Parse((string)log?.amount);
        VaultProposalType type = (VaultProposalType)(byte)log?.type;
        VaultProposalStatus status = (VaultProposalStatus)(byte)log?.status;
        ulong expiration = (ulong)log?.expiration;
        string description = (string)log?.description;

        if (proposalId < 1) throw new ArgumentOutOfRangeException(nameof(proposalId), "Proposal id must be greater than 0.");
        if (wallet == Address.Empty) throw new ArgumentNullException(nameof(wallet), "Wallet must be set.");
        if (amount == UInt256.Zero) throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than zero.");
        if (!type.IsValid()) throw new ArgumentOutOfRangeException(nameof(type), "Proposal type must be valid.");
        if (status != VaultProposalStatus.Pledge) throw new ArgumentOutOfRangeException(nameof(status), "Proposal status must be pledge.");
        if (expiration == 0) throw new ArgumentOutOfRangeException(nameof(expiration), "Expiration must be set.");
        if (!description.HasValue()) throw new ArgumentNullException(nameof(description), "Description must be set.");

        ProposalId = proposalId;
        Wallet = wallet;
        Amount = amount;
        Type = type;
        Status = status;
        Expiration = expiration;
        Description = description;
    }

    public CreateVaultProposalLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.CreateVaultProposalLog, id, transactionId, address, sortOrder)
    {
        var logDetails = DeserializeLogDetails(details);
        ProposalId = logDetails.ProposalId;
        Wallet = logDetails.Wallet;
        Amount = logDetails.Amount;
        Type = logDetails.Type;
        Status = logDetails.Status;
        Expiration = logDetails.Expiration;
        Description = logDetails.Description;
    }

    public ulong ProposalId { get; }
    public Address Wallet { get; }
    public UInt256 Amount { get; }
    public VaultProposalType Type { get; }
    public VaultProposalStatus Status { get; }
    public ulong Expiration { get; }
    public string Description { get; }

    private struct LogDetails
    {
        public ulong ProposalId { get; set; }
        public Address Wallet { get; set; }
        public UInt256 Amount { get; set; }
        public VaultProposalType Type { get; set; }
        public VaultProposalStatus Status { get; set; }
        public ulong Expiration { get; set; }
        public string Description { get; set; }
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
            Wallet = Wallet,
            Amount = Amount,
            Type = Type,
            Status = Status,
            Expiration = Expiration,
            Description = Description
        });
    }
}
