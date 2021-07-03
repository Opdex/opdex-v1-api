using System;
using System.Dynamic;
using FluentAssertions;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.TransactionLogs.Vaults
{
    public class RevokeVaultCerificateLogTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("  ")]
        public void CreateRevokeVaultCertificateLog_OwnerAddressNotSet_ThrowArgumentNullException(string owner)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.owner = owner;
            txLog.oldAmount = "32528";
            txLog.newAmount = "25578";
            txLog.vestedBlock = 1234ul;

            // Act
            void Act() => new RevokeVaultCertificateLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void CreateRevokeVaultCertificateLog_OldAmountInvalid_ThrowArgumentOutOfRangeException(string oldAmount)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj2";
            txLog.oldAmount = oldAmount;
            txLog.newAmount = "226373";
            txLog.vestedBlock = 1234ul;

            // Act
            void Act() => new RevokeVaultCertificateLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

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
        public void CreateRevokeVaultCertificateLog_NewAmountInvalid_ThrowArgumentOutOfRangeException(string newAmount)
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj2";
            txLog.oldAmount = "5258892303324235";
            txLog.newAmount = newAmount;
            txLog.vestedBlock = 1234ul;

            // Act
            void Act() => new RevokeVaultCertificateLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CreateRevokeVaultCertificateLog_VestedBlockZero_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj2";
            txLog.oldAmount = "43892";
            txLog.newAmount = "226373";
            txLog.vestedBlock = 0ul;

            // Act
            void Act() => new RevokeVaultCertificateLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CreateRevokeVaultCertificateLog_ValidArguments_SetProperties()
        {
            // Arrange
            dynamic txLog = new ExpandoObject();
            txLog.owner = "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj2";
            txLog.oldAmount = "43892";
            txLog.newAmount = "226373";
            txLog.vestedBlock = 1000ul;

            // Act
            var log = new RevokeVaultCertificateLog(txLog, "PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5", 5);

            // Assert
            log.Owner.Should().Be(txLog.owner);
            log.OldAmount.Should().Be(txLog.oldAmount);
            log.NewAmount.Should().Be(txLog.newAmount);
            log.VestedBlock.Should().Be(txLog.vestedBlock);
        }
    }
}