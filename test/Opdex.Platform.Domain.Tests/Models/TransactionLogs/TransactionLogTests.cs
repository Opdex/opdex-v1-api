using System;
using FluentAssertions;
using Opdex.Platform.Common.Models;
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
            static void Act() => new FakeTransactionLog((TransactionLogType)100, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 1);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CreateTransactionLog_InvalidContract_ThrowArgumentNullException()
        {
            // Arrange
            var contract = Address.Empty;

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
            var logType = TransactionLogType.ClaimPendingMarketOwnershipLog;
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
            transactionLog.SetTxId(0);

            // Assert
            transactionLog.TransactionId.Should().Be(0);
        }

        [Fact]
        public void SetTransactionId_GreaterThanZero_SetProperty()
        {
            // Arrange
            var transactionLog = new FakeTransactionLog(TransactionLogType.ApprovalLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Act
            transactionLog.SetTxId(5555);

            // Assert
            transactionLog.TransactionId.Should().Be(5555);
        }

        [Fact]
        public void SetTransactionId_AlreadySet_DoNotUpdate()
        {
            // Arrange
            var transactionLog = new FakeTransactionLog(TransactionLogType.ApprovalLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);
            transactionLog.SetTxId(1111);

            // Act
            transactionLog.SetTxId(5555);

            // Assert
            transactionLog.TransactionId.Should().Be(1111);
        }

        class FakeTransactionLog : TransactionLog
        {
            public FakeTransactionLog(TransactionLogType logType, Address contract, int sortOrder) : base(logType, contract, sortOrder)
            {
            }

            public int SerializeLogDetailsCallCount { get; private set; }

            public override string SerializeLogDetails()
            {
                SerializeLogDetailsCallCount++;
                return "";
            }

            public void SetTxId(ulong id)
            {
                SetTransactionId(id);
            }
        }
    }
}
