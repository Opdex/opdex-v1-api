using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models.Blocks;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Blocks
{
    public class BlockReceiptTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CreateBlockReceipt_InvalidHash_ThrowsArgumentNullException(string hash)
        {
            // Arrange
            // Act
            void Act() => new BlockReceipt(hash, 12, DateTime.UtcNow, DateTime.UtcNow, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(hash)} must have a value.");
        }

        [Fact]
        public void CreateBlockReceipt_InvalidHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const ulong height = 0;

            // Act
            void Act() => new BlockReceipt("hash", height, DateTime.UtcNow, DateTime.UtcNow, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(height)} must have a value greater than 0.");
        }

        [Fact]
        public void CreateBlockReceipt_InvalidTime_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var time = default(DateTime);

            // Act
            void Act() => new BlockReceipt("hash", 1, time, DateTime.UtcNow, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(time)} must have a valid value.");
        }

        [Fact]
        public void CreateBlockReceipt_InvalidMedianTime_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var medianTime = default(DateTime);

            // Act
            void Act() => new BlockReceipt("hash", 1, DateTime.UtcNow, medianTime, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(medianTime)} must have a valid value.");
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void CreateBlockReceipt_InvalidMerkleRoot_ThrowsArgumentNullException(string merkleRoot)
        {
            // Arrange
            // Act
            void Act() => new BlockReceipt("hash", 1, DateTime.UtcNow, DateTime.UtcNow, "previousBlockHash", "nextBlockHash", merkleRoot, new string[0]);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(merkleRoot)} must have a value.");
        }

        [Fact]
        public void CreateBlockReceipt_Success()
        {
            // Arrange
            var hash = "hash";
            var height = 1ul;
            var time = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));
            var medianTime = DateTime.UtcNow;
            var previousBlockHash = "previousBlockHash";
            var nextBlockHash = "nextBlockHash";
            var merkleRoot = "merkleRoot";
            var txHashes = new [] {"txHash1"};

            // Act
            var blockReceipt = new BlockReceipt(hash, height, time, medianTime, previousBlockHash, nextBlockHash, merkleRoot, txHashes);

            // Assert
            blockReceipt.Hash.Should().Be(hash);
            blockReceipt.Height.Should().Be(height);
            blockReceipt.Time.Should().Be(time);
            blockReceipt.MedianTime.Should().Be(medianTime);
            blockReceipt.PreviousBlockHash.Should().Be(previousBlockHash);
            blockReceipt.NextBlockHash.Should().Be(nextBlockHash);
            blockReceipt.MerkleRoot.Should().Be(merkleRoot);
            blockReceipt.TxHashes.Should().BeEquivalentTo(txHashes);
        }

        [Fact]
        public void BlockReceipt_IsNewYearFromPrevious_True()
        {
            // Arrange
            var currentTime = DateTime.UtcNow;
            var previousTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(365));
            var blockReceipt = new BlockReceipt("hash", 1, DateTime.UtcNow, currentTime, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Act
            var result = blockReceipt.IsNewYearFromPrevious(previousTime);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BlockReceipt_IsNewYearFromPrevious_False()
        {
            // Arrange
            var currentTime = new DateTime(2021, 6, 21);
            var previousTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));
            var blockReceipt = new BlockReceipt("hash", 1, DateTime.UtcNow, currentTime, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Act
            var result = blockReceipt.IsNewYearFromPrevious(previousTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void BlockReceipt_IsNewMonthFromPrevious_True()
        {
            // Arrange
            var currentTime = new DateTime(2021, 6, 1);
            var previousTime = new DateTime(2021, 5, 30);
            var blockReceipt = new BlockReceipt("hash", 1, DateTime.UtcNow, currentTime, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Act
            var result = blockReceipt.IsNewMonthFromPrevious(previousTime);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BlockReceipt_IsNewMonthFromPrevious_False()
        {
            // Arrange
            var currentTime = new DateTime(2021, 6, 23);
            var previousTime = new DateTime(2021, 6, 22);
            var blockReceipt = new BlockReceipt("hash", 1, DateTime.UtcNow, currentTime, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Act
            var result = blockReceipt.IsNewMonthFromPrevious(previousTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void BlockReceipt_IsNewDayFromPrevious_True()
        {
            // Arrange
            var currentTime = new DateTime(2021, 6, 21);
            var previousTime = new DateTime(2021, 6, 20);
            var blockReceipt = new BlockReceipt("hash", 1, DateTime.UtcNow, currentTime, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Act
            var result = blockReceipt.IsNewDayFromPrevious(previousTime);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BlockReceipt_IsNewDayFromPrevious_False()
        {
            // Arrange
            var currentTime = new DateTime(2021, 6, 21);
            var previousTime = new DateTime(2021, 6, 21);
            var blockReceipt = new BlockReceipt("hash", 1, DateTime.UtcNow, currentTime, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Act
            var result = blockReceipt.IsNewDayFromPrevious(previousTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void BlockReceipt_IsNewHourFromPrevious_True()
        {
            // Arrange
            var currentTime = new DateTime(2021, 6, 21, 12, 0, 0);
            var previousTime = new DateTime(2021, 6, 21, 11, 0, 0);
            var blockReceipt = new BlockReceipt("hash", 1, DateTime.UtcNow, currentTime, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Act
            var result = blockReceipt.IsNewHourFromPrevious(previousTime);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BlockReceipt_IsNewHourFromPrevious_False()
        {
            // Arrange
            var currentTime = new DateTime(2021, 6, 21, 12, 30, 0);
            var previousTime = new DateTime(2021, 6, 21, 12, 0, 0);
            var blockReceipt = new BlockReceipt("hash", 1, DateTime.UtcNow, currentTime, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Act
            var result = blockReceipt.IsNewHourFromPrevious(previousTime);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void BlockReceipt_IsNewMinuteFromPrevious_True()
        {
            // Arrange
            var currentTime = new DateTime(2021, 6, 21, 12, 10, 0);
            var previousTime = new DateTime(2021, 6, 21, 11, 9, 0);
            var blockReceipt = new BlockReceipt("hash", 1, DateTime.UtcNow, currentTime, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Act
            var result = blockReceipt.IsNewMinuteFromPrevious(previousTime);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void BlockReceipt_IsNewMinuteFromPrevious_False()
        {
            // Arrange
            var currentTime = new DateTime(2021, 6, 21, 12, 11, 0);
            var previousTime = new DateTime(2021, 6, 21, 12, 11, 30);
            var blockReceipt = new BlockReceipt("hash", 1, DateTime.UtcNow, currentTime, "previousBlockHash", "nextBlockHash", "merkleRoot", new string[0]);

            // Act
            var result = blockReceipt.IsNewMinuteFromPrevious(previousTime);

            // Assert
            result.Should().BeFalse();
        }
    }
}