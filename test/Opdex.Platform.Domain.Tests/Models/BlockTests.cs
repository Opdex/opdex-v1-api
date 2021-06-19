using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models
{
    public class BlockTests
    {
        [Fact]
        public void CreateBlock_InvalidHeight_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new Block(0, "hash", DateTime.UtcNow, DateTime.UtcNow);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void CreateBlock_InvalidHash_ThrowArgumentNullException(string hash)
        {
            // Arrange
            // Act
            void Act() => new Block(1, hash, DateTime.UtcNow, DateTime.UtcNow);

            // Assert
            Assert.Throws<ArgumentNullException>(Act);
        }

        [Fact]
        public void CreateBlock_InvalidTime_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new Block(1, "hash", default, DateTime.UtcNow);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CreateBlock_InvalidMedianTime_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new Block(1, "hash", DateTime.UtcNow, default);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void CreateBlock_Success()
        {
            const ulong height = ulong.MaxValue;
            const string hash = "Hash";
            var time = DateTime.UtcNow;
            var medianTime = DateTime.UtcNow;

            var block = new Block(height, hash, time, medianTime);

            block.Height.Should().Be(height);
            block.Hash.Should().Be(hash);
            block.Time.Should().Be(time);
            block.MedianTime.Should().Be(medianTime);
        }
    }
}