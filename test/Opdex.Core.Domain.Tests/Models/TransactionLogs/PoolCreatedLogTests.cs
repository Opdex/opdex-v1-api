using System.Dynamic;
using FluentAssertions;
using Opdex.Core.Domain.Models.TransactionLogs;
using Xunit;

namespace Opdex.Core.Domain.Tests.Models.TransactionLogs
{
    public class LiquidityPoolCreatedLogTests
    {
        [Fact]
        public void CreatesLiquidityPoolCreatedLog_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.Pool = "Pool";
            txLog.Token = "Token";

            var log = new LiquidityPoolCreatedLog(txLog, address, sortOrder);

            log.Id.Should().Be(0);
            log.TransactionId.Should().Be(0);
            log.LogType.Should().Be(nameof(LiquidityPoolCreatedLog));
            log.Address.Should().Be(address);
            log.SortOrder.Should().Be(sortOrder);
            log.Pool.Should().Be(txLog.Pool);
            log.Token.Should().Be(txLog.Token);
        }
    }
}