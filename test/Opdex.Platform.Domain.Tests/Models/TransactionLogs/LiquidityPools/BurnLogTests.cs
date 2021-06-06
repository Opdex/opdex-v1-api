using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.LiquidityPools
{
    public class BurnLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateBurnLog_SenderAddressNotSet_ThrowArgumentNullException(string sender)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = sender;
            txLog.to = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amountCrs = 1234ul;
            txLog.amountSrc = "83475";
            txLog.amountLpt = "23423";

            // Act
            void Act() => new BurnLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateBurnLog_ToAddressNotSet_ThrowArgumentNullException(string to)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.to = to;
            txLog.amountCrs = 1234ul;
            txLog.amountSrc = "83475";
            txLog.amountLpt = "23423";

            // Act
            void Act() => new BurnLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void CreateBurnLog_AmountCrsZero_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.to = "PO2p2uVqojah5kcXzHiBtV8LVDVGVAgvk4";
            txLog.amountCrs = 0ul;
            txLog.amountSrc = "83475";
            txLog.amountLpt = "23423";

            // Act
            void Act() => new BurnLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void CreateBurnLog_AmountSrcInvalid_ThrowArgumentOutOfRangeException(string amountSrc)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.to = "PO2p2uVqojah5kcXzHiBtV8LVDVGVAgvk4";
            txLog.amountCrs = 100ul;
            txLog.amountSrc = amountSrc;
            txLog.amountLpt = "23423";

            // Act
            void Act() => new BurnLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void CreateBurnLog_AmountLptInvalid_ThrowArgumentOutOfRangeException(string amountLpt)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.to = "PO2p2uVqojah5kcXzHiBtV8LVDVGVAgvk4";
            txLog.amountCrs = 100ul;
            txLog.amountSrc = "378901";
            txLog.amountLpt = amountLpt;

            // Act
            void Act() => new BurnLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

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