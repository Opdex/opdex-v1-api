using System.Dynamic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionEvents;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionEvents
{
    public class MintEventTests
    {
        [Fact]
        public void CreatesMintEvent_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.Sender = "Sender";
            txLog.AmountCrs = 1234ul;
            txLog.AmountSrc = "83475";

            var txEvent = new MintEvent(txLog, address, sortOrder);

            txEvent.Id.Should().Be(0);
            txEvent.TransactionId.Should().Be(0);
            txEvent.EventType.Should().Be(nameof(MintEvent));
            txEvent.Address.Should().Be(address);
            txEvent.SortOrder.Should().Be(sortOrder);
            txEvent.Sender.Should().Be(txLog.Sender);
            txEvent.AmountCrs.Should().Be(txLog.AmountCrs);
            txEvent.AmountSrc.Should().Be(txLog.AmountSrc);
        }
    }
}