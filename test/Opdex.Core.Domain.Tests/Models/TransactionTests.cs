using System.Collections.Generic;
using FluentAssertions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Domain.Models.TransactionEvents;
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
            
            dynamic syncEvent = new System.Dynamic.ExpandoObject();
            syncEvent.Address = "Address";
            syncEvent.Topics = new[] {"OpdexSyncEvent", "Topics"};
            syncEvent.Data = "SomeData";
            syncEvent.Log = new System.Dynamic.ExpandoObject();
            syncEvent.Log.ReserveCrs = 100ul;
            syncEvent.Log.ReserveSrc = "1500";

            logs.Add(syncEvent);

            var receipt = new Transaction(txHash, blockHeight, gasUsed, from, to);
            foreach (var log in logs)
            {
                receipt.DeserializeEvent(log.Address, log.Topics[0], 0, log.Log);    
            }

            receipt.Hash.Should().Be(txHash);
            receipt.BlockHeight.Should().Be(blockHeight);
            receipt.GasUsed.Should().Be(gasUsed);
            receipt.From.Should().Be(from);
            receipt.To.Should().Be(to);
            receipt.Events.Count.Should().Be(1);

            foreach (var eventReceipt in receipt.Events)
            {
                eventReceipt.Address.Should().Be(syncEvent.Address);
                
                var syncEventType = (SyncEvent)eventReceipt;
                syncEventType.ReserveCrs.Should().Be(syncEvent.Log.ReserveCrs);
                syncEventType.ReserveSrc.Should().Be(syncEvent.Log.ReserveSrc);
            }
        }
    }
}