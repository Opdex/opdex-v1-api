using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs
{
    public class BurnLogTests
    {
        [Fact]
        public void CreatesBurnLog_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.sender = "Sender";
            txLog.to = "To";
            txLog.amountCrs = 1234ul;
            txLog.amountSrc = "83475";
            txLog.amountLpt = "23423";

            var log = new BurnLog(txLog, address, sortOrder);

            log.Id.Should().Be(0);
            log.TransactionId.Should().Be(0);
            log.LogType.Should().Be(TransactionLogType.BurnLog);
            log.Contract.Should().Be(address);
            log.SortOrder.Should().Be(sortOrder);
            log.Sender.Should().Be(txLog.sender);
            log.To.Should().Be(txLog.to);
            log.AmountCrs.Should().Be(txLog.amountCrs);
            log.AmountSrc.Should().Be(txLog.amountSrc);
            log.AmountLpt.Should().Be(txLog.amountLpt);
        }
    }
}