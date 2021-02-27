using System.Dynamic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionEvents;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionEvents
{
    public class ApprovalEventTests
    {
        [Fact]
        public void CreatesApprovalEvent_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.Owner = "Owner";
            txLog.Spender = "Spender";
            txLog.Amount = "Amount";

            var txEvent = new ApprovalEvent(txLog, address, sortOrder);

            txEvent.Id.Should().Be(0);
            txEvent.TransactionId.Should().Be(0);
            txEvent.EventType.Should().Be(nameof(ApprovalEvent));
            txEvent.Address.Should().Be(address);
            txEvent.SortOrder.Should().Be(sortOrder);
            txEvent.Amount.Should().Be(txLog.Amount);
            txEvent.Spender.Should().Be(txLog.Spender);
            txEvent.Owner.Should().Be(txLog.Owner);
        }
    }
}