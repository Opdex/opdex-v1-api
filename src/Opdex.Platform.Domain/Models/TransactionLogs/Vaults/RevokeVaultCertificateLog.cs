using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Vaults;

public class RevokeVaultCertificateLog : TransactionLog
{
    public RevokeVaultCertificateLog(dynamic log, Address address, int sortOrder)
        : base(TransactionLogType.RevokeVaultCertificateLog, address, sortOrder)
    {
        Address owner = (string)log?.owner;
        UInt256 oldAmount = UInt256.Parse((string)log?.oldAmount);
        UInt256 newAmount = UInt256.Parse((string)log?.newAmount);
        ulong vestedBlock = log?.vestedBlock;

        if (owner == Address.Empty)
        {
            throw new ArgumentNullException(nameof(owner), "Owner must be set.");
        }

        if (vestedBlock < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(vestedBlock), "Vested block must be greater than 0.");
        }

        if (oldAmount == UInt256.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(oldAmount), "Old amount must be greater than 0.");
        }

        Owner = owner;
        OldAmount = oldAmount;
        NewAmount = newAmount;
        VestedBlock = vestedBlock;
    }

    public RevokeVaultCertificateLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.RevokeVaultCertificateLog, id, transactionId, address, sortOrder)
    {
        var logDetails = DeserializeLogDetails(details);
        Owner = logDetails.Owner;
        OldAmount = logDetails.OldAmount;
        NewAmount = logDetails.NewAmount;
        VestedBlock = logDetails.VestedBlock;
    }

    public Address Owner { get; }
    public UInt256 OldAmount { get; }
    public UInt256 NewAmount { get; }
    public ulong VestedBlock { get; }

    private struct LogDetails
    {
        public Address Owner { get; set; }
        public UInt256 OldAmount { get; set; }
        public UInt256 NewAmount { get; set; }
        public ulong VestedBlock { get; set; }
    }

    private static LogDetails DeserializeLogDetails(string details)
    {
        return JsonConvert.DeserializeObject<LogDetails>(details);
    }

    public override string SerializeLogDetails()
    {
        return JsonConvert.SerializeObject(new LogDetails
        {
            Owner = Owner,
            OldAmount = OldAmount,
            NewAmount = NewAmount,
            VestedBlock = VestedBlock
        });
    }
}