using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs
{
    public class ApprovalLogTests
    {
        [Fact]
        public void CreatesApprovalLog_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.Owner = "Owner";
            txLog.Spender = "Spender";
            txLog.Amount = "Amount";

            var log = new ApprovalLog(txLog, address, sortOrder);

            log.Id.Should().Be(0);
            log.TransactionId.Should().Be(0);
            log.LogType.Should().Be(nameof(ApprovalLog));
            log.Contract.Should().Be(address);
            log.SortOrder.Should().Be(sortOrder);
            log.Amount.Should().Be(txLog.Amount);
            log.Spender.Should().Be(txLog.Spender);
            log.Owner.Should().Be(txLog.Owner);
        }
    }
}