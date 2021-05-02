using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs
{
    public class ReservesLogTests
    {
        [Fact]
        public void CreatesReservesLog_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic txLog = new ExpandoObject();
            txLog.reserveCrs = 876543456789ul;
            txLog.reserveSrc = "87654345678";

            var log = new ReservesLog(txLog, address, sortOrder);

            log.Id.Should().Be(0);
            log.TransactionId.Should().Be(0);
            log.LogType.Should().Be(TransactionLogType.ReservesLog);
            log.Contract.Should().Be(address);
            log.SortOrder.Should().Be(sortOrder);
            log.ReserveCrs.Should().Be(txLog.reserveCrs);
            log.ReserveSrc.Should().Be(txLog.reserveSrc);
        }
    }
}