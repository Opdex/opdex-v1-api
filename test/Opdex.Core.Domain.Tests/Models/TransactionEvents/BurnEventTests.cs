using System.Dynamic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionEvents;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionEvents
{
    public class BurnEventTests
    {
        [Fact]
        public void CreatesBurnEvent_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.Sender = "Sender";
            txLog.To = "To";
            txLog.AmountCrs = 1234ul;
            txLog.AmountSrc = "83475";

            var txEvent = new BurnEvent(txLog, address, sortOrder);

            txEvent.Id.Should().Be(0);
            txEvent.TransactionId.Should().Be(0);
            txEvent.EventType.Should().Be(nameof(BurnEvent));
            txEvent.Address.Should().Be(address);
            txEvent.SortOrder.Should().Be(sortOrder);
            txEvent.Sender.Should().Be(txLog.Sender);
            txEvent.To.Should().Be(txLog.To);
            txEvent.AmountCrs.Should().Be(txLog.AmountCrs);
            txEvent.AmountSrc.Should().Be(txLog.AmountSrc);
        }
    }
}