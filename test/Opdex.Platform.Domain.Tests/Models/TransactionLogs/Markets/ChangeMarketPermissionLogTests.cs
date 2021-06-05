using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Markets
{
    public class ChangeMarketPermissionLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateChangeMarketPermissionLog_FromAddressNotSet_ThrowArgumentNullException(string from)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.address = from;
            txLog.permission = (byte)1;
            txLog.isAuthorized = true;

            // Act
            void Act() => new ChangeMarketPermissionLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void CreateChangeMarketPermissionLog_ValidArguments_SetProperties()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.address = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj1";
            txLog.permission = (byte)1;
            txLog.isAuthorized = true;

            // Act
            var log = new ChangeMarketPermissionLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            log.Address.Should().Be(txLog.address);
            log.Permission.Should().Be(txLog.permission);
            log.IsAuthorized.Should().Be(txLog.isAuthorized);
        }
    }
}