using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs
{
    public class SwapLogTests
    {
        [Fact]
        public void CreatesSwapLog_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.sender = "Sender";
            txLog.to = "To";
            txLog.amountCrsIn = 0ul;
            txLog.amountSrcIn = "876543";
            txLog.amountCrsOut = 2342323ul;
            txLog.amountSrcOut = "0";

            var log = new SwapLog(txLog, address, sortOrder);

            log.Id.Should().Be(0);
            log.TransactionId.Should().Be(0);
            log.LogType.Should().Be(TransactionLogType.SwapLog);
            log.Contract.Should().Be(address);
            log.SortOrder.Should().Be(sortOrder);
            log.Sender.Should().Be(txLog.sender);
            log.To.Should().Be(txLog.to);
            log.AmountCrsIn.Should().Be(txLog.amountCrsIn);
            log.AmountSrcIn.Should().Be(txLog.amountSrcIn);
            log.AmountCrsOut.Should().Be(txLog.amountCrsOut);
            log.AmountSrcOut.Should().Be(txLog.amountSrcOut);
        }
    }
}