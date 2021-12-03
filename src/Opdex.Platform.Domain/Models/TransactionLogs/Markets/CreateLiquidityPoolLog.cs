using System;
using Newtonsoft.Json;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Domain.Models.TransactionLogs.Markets;

public class CreateLiquidityPoolLog : TransactionLog
{
    public CreateLiquidityPoolLog(dynamic log, Address address, int sortOrder)
        : base(TransactionLogType.CreateLiquidityPoolLog, address, sortOrder)
    {
        Address token = (string)log?.token;
        Address pool = (string)log?.pool;

        if (token == Address.Empty)
        {
            throw new ArgumentNullException(nameof(token), "Token address must be set.");
        }

        if (pool == Address.Empty)
        {
            throw new ArgumentNullException(nameof(pool), "Pool address must be set.");
        }

        Token = token;
        Pool = pool;
    }

    public CreateLiquidityPoolLog(ulong id, ulong transactionId, Address address, int sortOrder, string details)
        : base(TransactionLogType.CreateLiquidityPoolLog, id, transactionId, address, sortOrder)
    {
        var logDetails = DeserializeLogDetails(details);
        Token = logDetails.Token;
        Pool = logDetails.Pool;
    }

    public Address Token { get; }
    public Address Pool { get; }

    private struct LogDetails
    {
        public Address Token { get; set; }
        public Address Pool { get; set; }
    }

    private static LogDetails DeserializeLogDetails(string details)
    {
        return JsonConvert.DeserializeObject<LogDetails>(details);
    }

    public override string SerializeLogDetails()
    {
        return JsonConvert.SerializeObject(new LogDetails
        {
            Token = Token,
            Pool = Pool
        });
    }
}