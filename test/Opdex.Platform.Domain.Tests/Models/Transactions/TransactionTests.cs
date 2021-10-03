using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Transactions
{
    public class TransactionTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateNew_Transaction_InvalidTxHash_ThrowArgumentNullException(string txHash)
        {
            // Arrange
            const ulong blockHeight = ulong.MaxValue;
            const int gasUsed = 90000;
            Address from = "PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT";
            Address to = "PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW";
            const bool success = true;

            // Act
            void Act() => new Transaction(txHash, blockHeight, gasUsed, from, to, success, null, null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Transaction hash must be set.");
        }

        [Fact]
        public void CreateNew_Transaction_InvalidFrom_ThrowArgumentNullException()
        {
            // Arrange
            Address from = Address.Empty;
            const string txHash = "TxHash";
            const ulong blockHeight = ulong.MaxValue;
            const int gasUsed = 90000;
            Address to = "PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW";
            const bool success = true;

            // Act
            void Act() => new Transaction(txHash, blockHeight, gasUsed, from, to, success, null, null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("From address must be set.");
        }

        [Fact]
        public void CreateNew_Transaction_InvalidTo_ThrowArgumentNullException()
        {
            // Arrange
            Address to = Address.Empty;
            const string txHash = "TxHash";
            const ulong blockHeight = ulong.MaxValue;
            const int gasUsed = 90000;
            Address from = "PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT";
            const bool success = true;

            // Act
            void Act() => new Transaction(txHash, blockHeight, gasUsed, from, to, success, null, null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("To address must be set.");
        }

        [Fact]
        public void CreateNew_Transaction_InvalidBlockHeight_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const string txHash = "TxHash";
            const ulong blockHeight = 0;
            const int gasUsed = 90000;
            Address from = "PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT";
            Address to = "PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW";
            const bool success = true;

            // Act
            void Act() => new Transaction(txHash, blockHeight, gasUsed, from, to, success, null, null);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than 0.");
        }

        [Fact]
        public void CreateNew_Transaction_InvalidGasUsed_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            const string txHash = "TxHash";
            const ulong blockHeight = ulong.MaxValue;
            const int gasUsed = 0;
            Address from = "PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT";
            Address to = "PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW";
            const bool success = true;

            // Act
            void Act() => new Transaction(txHash, blockHeight, gasUsed, from, to, success, null, null);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Transaction gas must be set.");
        }

        [Fact]
        public void CreateNew_Transaction_Success()
        {
            const string txHash = "TxHash";
            const ulong blockHeight = ulong.MaxValue;
            const int gasUsed = 90000;
            Address from = "PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT";
            Address to = "PSxx8BBVDpB5qHKmm7RGLDVaEL8p9NWbZW";
            const bool success = true;

            dynamic reservesLog = new System.Dynamic.ExpandoObject();
            reservesLog.reserveCrs = 100ul;
            reservesLog.reserveSrc = "1500";

            var logs = new List<TransactionLog>
            {
                new ReservesLog(reservesLog, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 0)
            };

            var receipt = new Transaction(txHash, blockHeight, gasUsed, from, to, success, null, logs);

            receipt.Hash.Should().Be(txHash);
            receipt.BlockHeight.Should().Be(blockHeight);
            receipt.GasUsed.Should().Be(gasUsed);
            receipt.From.Should().Be(from);
            receipt.To.Should().Be(to);
            receipt.NewContractAddress.Should().Be(Address.Empty);
            receipt.Success.Should().Be(success);
            receipt.Logs.Should().BeEquivalentTo(logs);
        }

        [Fact]
        public void CreatePersisted_Transaction_Success()
        {
            const ulong id = 1;
            const string txHash = "TxHash";
            const ulong blockHeight = ulong.MaxValue;
            const int gasUsed = 90000;
            Address from = "PJpR65NLUpTFgs8mJxdSC7bbwgyadJEVgT";
            Address to = null;
            const bool success = true;
            Address newContractAddress = "PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM";

            var receipt = new Transaction(id, txHash, blockHeight, gasUsed, from, to, success, newContractAddress);

            receipt.Id.Should().Be(id);
            receipt.Hash.Should().Be(txHash);
            receipt.BlockHeight.Should().Be(blockHeight);
            receipt.GasUsed.Should().Be(gasUsed);
            receipt.From.Should().Be(from);
            receipt.To.Should().Be(to);
            receipt.NewContractAddress.Should().Be(newContractAddress);
            receipt.Success.Should().Be(success);
            receipt.Logs.Should().BeEmpty();
        }
    }
}
