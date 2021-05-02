using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs
{
    public class TransferLogTests
    {
        [Fact]
        public void CreatesTransferLog_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic tx = new ExpandoObject();
            tx.from = "From";
            tx.to = "To";
            tx.amount = "87654345678";

            var txLog = new TransferLog(tx, address, sortOrder);

            txLog.Id.Should().Be(0);
            txLog.TransactionId.Should().Be(0);
            txLog.LogType.Should().Be(TransactionLogType.TransferLog);
            txLog.Contract.Should().Be(address);
            txLog.SortOrder.Should().Be(sortOrder);
            txLog.From.Should().Be(tx.from);
            txLog.To.Should().Be(tx.to);
            txLog.Amount.Should().Be(tx.amount);
        }
    }
}