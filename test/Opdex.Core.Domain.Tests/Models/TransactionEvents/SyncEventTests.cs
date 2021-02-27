using System.Dynamic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionEvents;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionEvents
{
    public class SyncEventTests
    {
        [Fact]
        public void CreatesSyncEvent_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.ReserveCrs = 876543456789ul;
            txLog.ReserveSrc = "87654345678";

            var txEvent = new SyncEvent(txLog, address, sortOrder);

            txEvent.Id.Should().Be(0);
            txEvent.TransactionId.Should().Be(0);
            txEvent.EventType.Should().Be(nameof(SyncEvent));
            txEvent.Address.Should().Be(address);
            txEvent.SortOrder.Should().Be(sortOrder);
            txEvent.ReserveCrs.Should().Be(txLog.ReserveCrs);
            txEvent.ReserveSrc.Should().Be(txLog.ReserveSrc);
        }
    }
}