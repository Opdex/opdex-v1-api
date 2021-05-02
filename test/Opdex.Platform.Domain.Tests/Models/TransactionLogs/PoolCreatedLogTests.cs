using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs
{
    public class LiquidityPoolCreatedLogTests
    {
        [Fact]
        public void CreatesLiquidityPoolCreatedLog_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.pool = "Pool";
            txLog.token = "Token";

            var log = new LiquidityPoolCreatedLog(txLog, address, sortOrder);

            log.Id.Should().Be(0);
            log.TransactionId.Should().Be(0);
            log.LogType.Should().Be(TransactionLogType.LiquidityPoolCreatedLog);
            log.Contract.Should().Be(address);
            log.SortOrder.Should().Be(sortOrder);
            log.Pool.Should().Be(txLog.pool);
            log.Token.Should().Be(txLog.token);
        }
    }
}