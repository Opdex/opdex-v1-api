using System.Dynamic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionLogs;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionLogs
{
    public class TransferLogTests
    {
        [Fact]
        public void CreatesTransferLog_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic tx = new ExpandoObject();
            tx.From = "From";
            tx.To = "To";
            tx.Amount = "87654345678";

            var txLog = new TransferLog(tx, address, sortOrder);

            txLog.Id.Should().Be(0);
            txLog.TransactionId.Should().Be(0);
            txLog.LogType.Should().Be(nameof(TransferLog));
            txLog.Address.Should().Be(address);
            txLog.SortOrder.Should().Be(sortOrder);
            txLog.From.Should().Be(tx.From);
            txLog.To.Should().Be(tx.To);
            txLog.Amount.Should().Be(tx.Amount);
        }
    }
}