using System.Dynamic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionEvents;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionEvents
{
    public class SwapEventTests
    {
        [Fact]
        public void CreatesSwapEvent_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.Sender = "Sender";
            txLog.To = "To";
            txLog.AmountCrsIn = 0ul;
            txLog.AmountSrcIn = "876543";
            txLog.AmountCrsOut = 2342323ul;
            txLog.AmountSrcOut = "0";

            var txEvent = new SwapEvent(txLog, address, sortOrder);

            txEvent.Id.Should().Be(0);
            txEvent.TransactionId.Should().Be(0);
            txEvent.EventType.Should().Be(nameof(SwapEvent));
            txEvent.Address.Should().Be(address);
            txEvent.SortOrder.Should().Be(sortOrder);
            txEvent.Sender.Should().Be(txLog.Sender);
            txEvent.To.Should().Be(txLog.To);
            txEvent.AmountCrsIn.Should().Be(txLog.AmountCrsIn);
            txEvent.AmountSrcIn.Should().Be(txLog.AmountSrcIn);
            txEvent.AmountCrsOut.Should().Be(txLog.AmountCrsOut);
            txEvent.AmountSrcOut.Should().Be(txLog.AmountSrcOut);
        }
    }
}