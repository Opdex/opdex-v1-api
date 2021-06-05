using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.LiquidityPools
{
    public class SwapLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateSwapLog_SenderAddressNotSet_ThrowArgumentNullException(string sender)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = sender;
            txLog.to = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amountCrsIn = 0ul;
            txLog.amountSrcIn = "876543";
            txLog.amountCrsOut = 2342323ul;
            txLog.amountSrcOut = "0";

            // Act
            void Act() => new SwapLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateSwapLog_ToAddressNotSet_ThrowArgumentNullException(string to)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.to = to;
            txLog.amountCrsIn = 0ul;
            txLog.amountSrcIn = "876543";
            txLog.amountCrsOut = 2342323ul;
            txLog.amountSrcOut = "0";

            // Act
            void Act() => new SwapLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void CreateSwapLog_CrsInAndOutBothZero_ThrowArgumentException()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.to = "PO2p2uVqojah5kcXzHiBtV8LVDVGVAgvk4";
            txLog.amountCrsIn = 0ul;
            txLog.amountSrcIn = "876543";
            txLog.amountCrsOut = 0ul;
            txLog.amountSrcOut = "0";

            // Act
            void Act() => new SwapLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC")]
        [InlineData("100.005")]
        [InlineData("100_000")]
        public void CreateSwapLog_SrcInNotValid_ThrowArgumentOutOfRangeException(string srcIn)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.to = "PO2p2uVqojah5kcXzHiBtV8LVDVGVAgvk4";
            txLog.amountCrsIn = 0ul;
            txLog.amountSrcIn = srcIn;
            txLog.amountCrsOut = 2342323ul;
            txLog.amountSrcOut = "0";

            // Act
            void Act() => new SwapLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void CreateSwapLog_SrcOutNotValid_ThrowArgumentOutOfRangeException(string srcOut)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.sender = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.to = "PO2p2uVqojah5kcXzHiBtV8LVDVGVAgvk4";
            txLog.amountCrsIn = 0ul;
            txLog.amountSrcIn = "4392";
            txLog.amountCrsOut = 2342323ul;
            txLog.amountSrcOut = srcOut;

            // Act
            void Act() => new SwapLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

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