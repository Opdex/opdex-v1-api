using System.Collections.Generic;
using FluentAssertions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models
{
    public class TransactionTests
    {
        // [Fact]
        // public void CreateTransaction_Success()
        // {
        //     const string txHash = "TxHash";
        //     const ulong blockHeight = ulong.MaxValue;
        //     const int gasUsed = 90000;
        //     const string from = "From";
        //     const string to = "To";
        //     const bool success = true;
        //     var logs = new List<dynamic>();
        //
        //     dynamic syncLog = new System.Dynamic.ExpandoObject();
        //     syncLog.Address = "Address";
        //     syncLog.Topics = new[] {"ReservesLog", "Topics"};
        //     syncLog.Data = "SomeData";
        //     syncLog.Log = new System.Dynamic.ExpandoObject();
        //     syncLog.Log.reserveCrs = 100ul;
        //     syncLog.Log.reserveSrc = "1500";
        //
        //     logs.Add(syncLog);
        //
        //     var receipt = new Transaction(txHash, blockHeight, gasUsed, from, to, success);
        //     foreach (var log in logs)
        //     {
        //         receipt.DeserializeLog(log.Address, log.Topics[0], 0, log.Log);
        //     }
        //
        //     receipt.Hash.Should().Be(txHash);
        //     receipt.BlockHeight.Should().Be(blockHeight);
        //     receipt.GasUsed.Should().Be(gasUsed);
        //     receipt.From.Should().Be(from);
        //     receipt.To.Should().Be(to);
        //     receipt.Logs.Count.Should().Be(1);
        //
        //     foreach (var logReceipt in receipt.Logs)
        //     {
        //         logReceipt.Contract.Should().Be(syncLog.Address);
        //
        //         var syncLogType = (ReservesLog)logReceipt;
        //         syncLogType.ReserveCrs.Should().Be(syncLog.Log.reserveCrs);
        //         syncLogType.ReserveSrc.Should().Be(syncLog.Log.reserveSrc);
        //     }
        // }
    }
}
