using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

public class ApprovalLog : TransactionLog
{
    public ApprovalLog(dynamic log, Address address, int sortOrder)
        : base(TransactionLogType.ApprovalLog, address, sortOrder)
    {
        Address owner = (string)log?.owner;
        Address spender = (string)log?.spender;
        UInt256 amount = UInt256.Parse((string)log?.amount);
        UInt256 oldAmount = UInt256.Parse((string)log?.oldAmount);

        if (owner == Address.Empty)
        {
            throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
        }

        if (spender == Address.Empty)
        {
            throw new ArgumentNullException(nameof(spender), "Spender address must be set.");
        }

        Owner = owner;
        Spender = spender;
        Amount = amount;
        OldAmount = oldAmount;
    }

    public ApprovalLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.ApprovalLog, id, transactionId, address, sortOrder)
    {
        var logDetails = DeserializeLogDetails(details);
        Owner = logDetails.Owner;
        Spender = logDetails.Spender;
        Amount = logDetails.Amount;
        OldAmount = logDetails.OldAmount;
    }

    public Address Owner { get; }
    public Address Spender { get; }
    public UInt256 Amount { get; }
    public UInt256 OldAmount { get; }

    private struct LogDetails
    {
        public Address Owner { get; set; }
        public Address Spender { get; set; }
        public UInt256 Amount { get; set; }
        public UInt256 OldAmount { get; set; }
    }

    private LogDetails DeserializeLogDetails(string details)
    {
        return JsonConvert.DeserializeObject<LogDetails>(details);
    }

    public override string SerializeLogDetails()
    {
        return JsonConvert.SerializeObject(new LogDetails
        {
            Owner = Owner,
            Spender = Spender,
            Amount = Amount,
            OldAmount = OldAmount
        });
    }
}