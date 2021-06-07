using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.Vault;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Vault
{
    public class CreateVaultCertificateLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateVaultCertificateLog_OwnerAddressNotSet_ThrowArgumentNullException(string owner)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.owner = owner;
            txLog.amount = "32528";
            txLog.vestedBlock = 1234ul;

            // Act
            void Act() => new CreateVaultCertificateLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void CreateVaultCertificateLog_AmountInvalid_ThrowArgumentOutOfRangeException(string amount)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj2";
            txLog.amount = amount;
            txLog.vestedBlock = 1234ul;

            // Act
            void Act() => new CreateVaultCertificateLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CreateVaultCertificateLog_VestedBlockZero_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj2";
            txLog.amount = "43892";
            txLog.vestedBlock = 0ul;

            // Act
            void Act() => new CreateVaultCertificateLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CreateVaultCertificateLog_ValidArguments_SetProperties()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj2";
            txLog.amount = "43892";
            txLog.vestedBlock = 1000ul;

            // Act
            var log = new CreateVaultCertificateLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            log.Owner.Should().Be(txLog.owner);
            log.Amount.Should().Be(txLog.amount);
            log.VestedBlock.Should().Be(txLog.vestedBlock);
        }
    }
}