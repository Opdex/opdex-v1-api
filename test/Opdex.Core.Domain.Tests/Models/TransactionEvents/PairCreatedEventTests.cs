using System.Dynamic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionEvents;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionEvents
{
    public class PairCreatedEventTests
    {
        [Fact]
        public void CreatesPairCreatedEvent_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.Pair = "Pair";
            txLog.Token = "Token";

            var txEvent = new PairCreatedEvent(txLog, address, sortOrder);

            txEvent.Id.Should().Be(0);
            txEvent.TransactionId.Should().Be(0);
            txEvent.EventType.Should().Be(nameof(PairCreatedEvent));
            txEvent.Address.Should().Be(address);
            txEvent.SortOrder.Should().Be(sortOrder);
            txEvent.Pair.Should().Be(txLog.Pair);
            txEvent.Token.Should().Be(txLog.Token);
        }
    }
}