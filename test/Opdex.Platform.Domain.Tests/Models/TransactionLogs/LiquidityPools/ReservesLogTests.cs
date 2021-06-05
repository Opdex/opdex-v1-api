using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.LiquidityPools
{
    public class ReservesLogTests
    {
        [Fact]
        public void CreateReserveLog_ReserveCrsZero_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.reserveCrs = 0ul;
            txLog.reserveSrc = "957488";

            // Act
            void Act() => new ReservesLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC")]
        [InlineData("100.005")]
        [InlineData("100_000")]
        public void CreateReserveLog_ReserveSrcInvalid_ThrowArgumentOutOfRangeException(string reserve)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.reserveCrs = 500ul;
            txLog.reserveSrc = reserve;

            // Act
            void Act() => new ReservesLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

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