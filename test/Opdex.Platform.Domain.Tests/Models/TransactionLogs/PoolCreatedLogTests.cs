using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs
{
    public class CreateLiquidityPoolLogTests
    {
        [Fact]
        public void CreatesCreateLiquidityPoolLog_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.pool = "Pool";
            txLog.token = "Token";

            var log = new CreateLiquidityPoolLog(txLog, address, sortOrder);

            log.Id.Should().Be(0);
            log.TransactionId.Should().Be(0);
            log.LogType.Should().Be(TransactionLogType.CreateLiquidityPoolLog);
            log.Contract.Should().Be(address);
            log.SortOrder.Should().Be(sortOrder);
            log.Pool.Should().Be(txLog.pool);
            log.Token.Should().Be(txLog.token);
        }
    }
}