using Newtonsoft.Json;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Tokens;

public class SupplyChangeLog : TransactionLog
{
    public SupplyChangeLog(dynamic log, Address contract, int sortOrder)
        : base(TransactionLogType.SupplyChangeLog, contract, sortOrder)
    {
        PreviousSupply = UInt256.Parse((string)log?.previousSupply);
        TotalSupply = UInt256.Parse((string)log?.totalSupply);
    }

    public SupplyChangeLog(ulong id, ulong transactionId, Address contract, int sortOrder, string details)
        : base(TransactionLogType.SupplyChangeLog, id, transactionId, contract, sortOrder)
    {
        var logDetails = DeserializeLogDetails(details);
        PreviousSupply = logDetails.PreviousSupply;
        TotalSupply = logDetails.TotalSupply;
    }

    public UInt256 PreviousSupply { get; }
    public UInt256 TotalSupply { get; }

    private struct LogDetails
    {
        public UInt256 PreviousSupply { get; set; }
        public UInt256 TotalSupply { get; set; }
    }

    private static LogDetails DeserializeLogDetails(string details)
    {
        return JsonConvert.DeserializeObject<LogDetails>(details);
    }

    public override string SerializeLogDetails()
    {
        return JsonConvert.SerializeObject(new LogDetails
        {
            PreviousSupply = PreviousSupply,
            TotalSupply = TotalSupply
        });
    }
}
