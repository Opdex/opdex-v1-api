using System.Dynamic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionEvents;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionEvents
{
    public class PoolCreatedEventTests
    {
        [Fact]
        public void CreatesPoolCreatedEvent_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.Pool = "Pool";
            txLog.Token = "Token";

            var txEvent = new PoolCreatedEvent(txLog, address, sortOrder);

            txEvent.Id.Should().Be(0);
            txEvent.TransactionId.Should().Be(0);
            txEvent.EventType.Should().Be(nameof(PoolCreatedEvent));
            txEvent.Address.Should().Be(address);
            txEvent.SortOrder.Should().Be(sortOrder);
            txEvent.Pool.Should().Be(txLog.Pool);
            txEvent.Token.Should().Be(txLog.Token);
        }
    }
}