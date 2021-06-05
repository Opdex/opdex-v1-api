using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs
{
    public class TransactionLogTests
    {
        [Fact]
        public void CreateTransactionLog_UnknownLogType_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new FakeTransactionLog(TransactionLogType.Unknown, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 1);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateTransactionLog_InvalidContract_ThrowArgumentNullException(string contract)
        {
            // Arrange
            // Act
            void Act() => new FakeTransactionLog(TransactionLogType.ApprovalLog, contract, 1);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void CreateTransactionLog_NegativeSortOrder_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new FakeTransactionLog(TransactionLogType.ApprovalLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", -1);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CreateTransactionLog_ValidArguments_PropertiesAreSet()
        {
            // Arrange
            var logType = TransactionLogType.ChangeMarketOwnerLog;
            var contract = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            var sortOrder = 5;

            // Act
            var transactionLog = new FakeTransactionLog(logType, contract, sortOrder);

            // Assert
            transactionLog.LogType.Should().Be(logType);
            transactionLog.Contract.Should().Be(contract);
            transactionLog.SortOrder.Should().Be(sortOrder);
            transactionLog.TransactionId.Should().Be(0);
        }

        [Fact]
        public void SetTransactionId_ArgumentNotGreaterThanZero_DoNotSet()
        {
            // Arrange
            var transactionLog = new FakeTransactionLog(TransactionLogType.ApprovalLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Act
            transactionLog.SetTransactionId(-1);

            // Assert
            transactionLog.TransactionId.Should().Be(0);
        }

        [Fact]
        public void SetTransactionId_GreaterThanZero_SetProperty()
        {
            // Arrange
            var transactionLog = new FakeTransactionLog(TransactionLogType.ApprovalLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Act
            transactionLog.SetTransactionId(5555);

            // Assert
            transactionLog.TransactionId.Should().Be(5555);
        }

        [Fact]
        public void SetTransactionId_AlreadySet_DoNotUpdate()
        {
            // Arrange
            var transactionLog = new FakeTransactionLog(TransactionLogType.ApprovalLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);
            transactionLog.SetTransactionId(1111);

            // Act
            transactionLog.SetTransactionId(5555);

            // Assert
            transactionLog.TransactionId.Should().Be(1111);
        }

        class FakeTransactionLog : TransactionLog
        {
            public FakeTransactionLog(TransactionLogType logType, string contract, int sortOrder) : base(logType, contract, sortOrder)
            {
            }

            public int SerializeLogDetailsCallCount { get; private set; }

            public override string SerializeLogDetails()
            {
                SerializeLogDetailsCallCount++;
                return "";
            }
        }
    }
}