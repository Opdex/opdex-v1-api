using System.Collections.Generic;
using FluentAssertions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionLogs;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models
{
    public class TransactionTests
    {
        [Fact]
        public void CreateTransaction_Success()
        {
            const string txHash = "TxHash";
            const ulong blockHeight = ulong.MaxValue;
            const int gasUsed = 90000;
            const string from = "From";
            const string to = "To";
            var logs = new List<dynamic>();
            
            dynamic syncLog = new System.Dynamic.ExpandoObject();
            syncLog.Address = "Address";
            syncLog.Topics = new[] {"OpdexReservesLog", "Topics"};
            syncLog.Data = "SomeData";
            syncLog.Log = new System.Dynamic.ExpandoObject();
            syncLog.Log.ReserveCrs = 100ul;
            syncLog.Log.ReserveSrc = "1500";

            logs.Add(syncLog);

            var receipt = new Transaction(txHash, blockHeight, gasUsed, from, to);
            foreach (var log in logs)
            {
                receipt.DeserializeLog(log.Address, log.Topics[0], 0, log.Log);    
            }

            receipt.Hash.Should().Be(txHash);
            receipt.BlockHeight.Should().Be(blockHeight);
            receipt.GasUsed.Should().Be(gasUsed);
            receipt.From.Should().Be(from);
            receipt.To.Should().Be(to);
            receipt.Logs.Count.Should().Be(1);

            foreach (var logReceipt in receipt.Logs)
            {
                logReceipt.Address.Should().Be(syncLog.Address);
                
                var syncLogType = (ReservesLog)logReceipt;
                syncLogType.ReserveCrs.Should().Be(syncLog.Log.ReserveCrs);
                syncLogType.ReserveSrc.Should().Be(syncLog.Log.ReserveSrc);
            }
        }
    }
}