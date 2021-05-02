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
            txLog.owner = "Owner";
            txLog.spender = "Spender";
            txLog.amount = "1";
            txLog.oldAmount = "0";

            var log = new ApprovalLog(txLog, address, sortOrder);

            log.Id.Should().Be(0);
            log.TransactionId.Should().Be(0);
            log.LogType.Should().Be(TransactionLogType.ApprovalLog);
            log.Contract.Should().Be(address);
            log.SortOrder.Should().Be(sortOrder);
            log.Amount.Should().Be(txLog.amount);
            log.OldAmount.Should().Be(txLog.oldAmount);
            log.Spender.Should().Be(txLog.spender);
            log.Owner.Should().Be(txLog.owner);
        }
    }
}