using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Tokens
{
    public class ApprovalLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateApprovalLog_OwnerNotSet_ThrowArgumentNullException(string owner)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.owner = owner;
            txLog.spender = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5";
            txLog.amount = "1";
            txLog.oldAmount = "0";

            // Act
            void Act() => new ApprovalLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateApprovalLog_SpenderNotSet_ThrowArgumentNullException(string spender)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj3";
            txLog.spender = spender;
            txLog.amount = "1";
            txLog.oldAmount = "0";

            // Act
            void Act() => new ApprovalLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void CreateApprovalLog_Success()
        {
            Address address = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
            const int sortOrder = 1;

            UInt256 amount = 1;
            UInt256 oldAmount = 0;

            dynamic txLog = new ExpandoObject();
            txLog.owner = "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN";
            txLog.spender = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";
            txLog.amount = amount.ToString();
            txLog.oldAmount = oldAmount.ToString();

            var log = new ApprovalLog(txLog, address, sortOrder);

            log.Id.Should().Be(0);
            log.TransactionId.Should().Be(0);
            log.LogType.Should().Be(TransactionLogType.ApprovalLog);
            log.Contract.Should().Be(address);
            log.SortOrder.Should().Be(sortOrder);
            log.Amount.Should().Be(amount);
            log.OldAmount.Should().Be(oldAmount);
            log.Spender.Should().Be(txLog.spender);
            log.Owner.Should().Be(txLog.owner);
        }
    }
}
