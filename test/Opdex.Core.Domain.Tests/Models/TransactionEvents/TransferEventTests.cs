using System.Dynamic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionEvents;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionEvents
{
    public class TransferEventTests
    {
        [Fact]
        public void CreatesTransferEvent_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.From = "From";
            txLog.To = "To";
            txLog.Amount = "87654345678";

            var txEvent = new TransferEvent(txLog, address, sortOrder);

            txEvent.Id.Should().Be(0);
            txEvent.TransactionId.Should().Be(0);
            txEvent.EventType.Should().Be(nameof(TransferEvent));
            txEvent.Address.Should().Be(address);
            txEvent.SortOrder.Should().Be(sortOrder);
            txEvent.From.Should().Be(txLog.From);
            txEvent.To.Should().Be(txLog.To);
            txEvent.Amount.Should().Be(txLog.Amount);
        }
    }
}