using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Tokens
{
    public class TransferLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateTransferLog_FromAddressNotSet_ThrowArgumentNullException(string from)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.from = from;
            txLog.to = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = "32981";

            // Act
            void Act() => new TransferLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateTransferLog_ToAddressNotSet_ThrowArgumentNullException(string to)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.from = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.to = to;
            txLog.amount = "32981";

            // Act
            void Act() => new TransferLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC")]
        [InlineData("100.005")]
        [InlineData("100_000")]
        public void CreateTransferLog_AmountInvalid_ThrowArgumentOutOfRangeException(string amount)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.from = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
            txLog.to = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj7";
            txLog.amount = amount;

            // Act
            void Act() => new TransferLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CreatesTransferLog_Success()
        {
            const string address = "Address";
            const int sortOrder = 1;

            dynamic tx = new ExpandoObject();
            tx.from = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
            tx.to = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj7";
            tx.amount = "87654345678";

            var txLog = new TransferLog(tx, address, sortOrder);

            txLog.Id.Should().Be(0);
            txLog.TransactionId.Should().Be(0);
            txLog.LogType.Should().Be(TransactionLogType.TransferLog);
            txLog.Contract.Should().Be(address);
            txLog.SortOrder.Should().Be(sortOrder);
            txLog.From.Should().Be(tx.from);
            txLog.To.Should().Be(tx.to);
            txLog.Amount.Should().Be(tx.amount);
        }
    }
}