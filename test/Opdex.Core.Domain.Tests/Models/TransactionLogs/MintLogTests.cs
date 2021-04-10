using System.Dynamic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionLogs;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionLogs
{
    public class MintLogTests
    {
        [Fact]
        public void CreatesMintLog_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.Sender = "Sender";
            txLog.AmountCrs = 1234ul;
            txLog.AmountSrc = "83475";

            var log = new MintLog(txLog, address, sortOrder);

            log.Id.Should().Be(0);
            log.TransactionId.Should().Be(0);
            log.LogType.Should().Be(nameof(MintLog));
            log.Contract.Should().Be(address);
            log.SortOrder.Should().Be(sortOrder);
            log.Sender.Should().Be(txLog.Sender);
            log.AmountCrs.Should().Be(txLog.AmountCrs);
            log.AmountSrc.Should().Be(txLog.AmountSrc);
        }
    }
}