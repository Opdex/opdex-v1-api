using System.Dynamic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionLogs;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionLogs
{
    public class SwapLogTests
    {
        [Fact]
        public void CreatesSwapLog_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.Sender = "Sender";
            txLog.To = "To";
            txLog.AmountCrsIn = 0ul;
            txLog.AmountSrcIn = "876543";
            txLog.AmountCrsOut = 2342323ul;
            txLog.AmountSrcOut = "0";

            var log = new SwapLog(txLog, address, sortOrder);

            log.Id.Should().Be(0);
            log.TransactionId.Should().Be(0);
            log.LogType.Should().Be(nameof(SwapLog));
            log.Address.Should().Be(address);
            log.SortOrder.Should().Be(sortOrder);
            log.Sender.Should().Be(txLog.Sender);
            log.To.Should().Be(txLog.To);
            log.AmountCrsIn.Should().Be(txLog.AmountCrsIn);
            log.AmountSrcIn.Should().Be(txLog.AmountSrcIn);
            log.AmountCrsOut.Should().Be(txLog.AmountCrsOut);
            log.AmountSrcOut.Should().Be(txLog.AmountSrcOut);
        }
    }
}