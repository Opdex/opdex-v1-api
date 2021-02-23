using System.Linq;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionReceipt;
using Opdex.Core.Domain.Models.TransactionReceipt.LogEvents;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionReceiptTests
{
    public class TransactionLogTests
    {
        [Fact]
        public void CreateTransactionLog_Success()
        {
            dynamic log = new System.Dynamic.ExpandoObject();
            dynamic logEvent = new System.Dynamic.ExpandoObject();
            logEvent.ReserveCrs = 100ul;
            logEvent.ReserveSrc = "1500";
            
            log.Address = "SomeAddress";
            log.Topics = new[] { "SyncEvent" };
            log.Log = logEvent;

            var transactionLog = new TransactionLog(log);

            transactionLog.Address.Should().Be(log.Address);
            transactionLog.Topics.First().Should().Be(nameof(SyncEvent));
            
            var transactionLogEvent = (SyncEvent)transactionLog.Event;
            transactionLogEvent.ReserveCrs.Should().Be(logEvent.ReserveCrs);
            transactionLogEvent.ReserveSrc.Should().Be(logEvent.ReserveSrc);
        }
    }
}