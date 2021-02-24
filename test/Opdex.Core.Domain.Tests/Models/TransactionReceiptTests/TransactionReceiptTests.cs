using System.Collections.Generic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionReceipt;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionReceiptTests
{
    public class TransactionReceiptTests
    {
        [Fact]
        public void CreateTransactionReceipt_Success()
        {
            const string txHash = "TxHash";
            const ulong blockHeight = ulong.MaxValue;
            const int gasUsed = 90000;
            const string from = "From";
            const string to = "To";
            const bool success = true;
            var logs = new List<dynamic>();
            
            dynamic syncEvent = new System.Dynamic.ExpandoObject();
            syncEvent.Address = "Address";
            syncEvent.Topics = new[] {"SyncEvent", "Topics"};
            syncEvent.Data = "SomeData";
            syncEvent.Log = new System.Dynamic.ExpandoObject();
            syncEvent.Log.ReserveCrs = 100ul;
            syncEvent.Log.ReserveSrc = "1500";

            logs.Add(syncEvent);

            var receipt = new TransactionReceipt(txHash, blockHeight, gasUsed, from, to, success, logs.ToArray());

            receipt.Hash.Should().Be(txHash);
            receipt.BlockHeight.Should().Be(blockHeight);
            receipt.GasUsed.Should().Be(gasUsed);
            receipt.From.Should().Be(from);
            receipt.To.Should().Be(to);
            receipt.Events.Count.Should().Be(1);

            foreach (var eventReceipt in receipt.Events)
            {
                eventReceipt.Address.Should().Be(syncEvent.Address);
                
                var syncEventType = (SyncEvent)eventReceipt.Event;
                syncEventType.ReserveCrs.Should().Be(syncEvent.Log.ReserveCrs);
                syncEventType.ReserveSrc.Should().Be(syncEvent.Log.ReserveSrc);
            }
        }
    }
}