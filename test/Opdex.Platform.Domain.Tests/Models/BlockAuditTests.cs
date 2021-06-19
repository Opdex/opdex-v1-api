using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models
{
    public class BlockAuditTests
    {
        [Fact]
        public void Constructor_CreatedBlockBeforeGenesis_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            ulong createdBlock = 0;
            ulong modifiedBlock = 100_000;

            // Act
            void Act() => new FakeBlockAudit(createdBlock, modifiedBlock);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void Constructor_ModifiedBlockBeforeCreatedBlock_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            ulong createdBlock = 100_001;
            ulong modifiedBlock = 100_000;

            // Act
            void Act() => new FakeBlockAudit(createdBlock, modifiedBlock);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Fact]
        public void Constructor_ValidArgumentsDefaultModifiedBlock_PropertiesSet()
        {
            // Arrange
            ulong createdBlock = 100_000;
            ulong modifiedBlock = 0;

            // Act
            var blockAudit = new FakeBlockAudit(createdBlock, modifiedBlock);

            // Assert
            blockAudit.CreatedBlock.Should().Be(createdBlock);
            blockAudit.ModifiedBlock.Should().Be(createdBlock);
        }

        [Fact]
        public void Constructor_ValidArgumentsNonDefaultModifiedBlock_PropertiesSet()
        {
            // Arrange
            ulong createdBlock = 100_000;
            ulong modifiedBlock = 100_001;

            // Act
            var blockAudit = new FakeBlockAudit(createdBlock, modifiedBlock);

            // Assert
            blockAudit.CreatedBlock.Should().Be(createdBlock);
            blockAudit.ModifiedBlock.Should().Be(modifiedBlock);
        }

        [Fact]
        public void SetModifiedBlock_PreviousBlock_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            var blockAudit = new FakeBlockAudit(100_000, 100_100);

            // Act
            void Act() => blockAudit.SetModifiedBlock(100_099);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act);
        }

        [Theory]
        [InlineData(100_001, 100_001)]
        [InlineData(100_001, 100_002)]
        public void SetModifiedBlock_SameOrMoreRecentBlock_PropertyUpdated(ulong originalBlock, ulong updatedBlock)
        {
            // Arrange
            var blockAudit = new FakeBlockAudit(100_000, originalBlock);

            // Act
            blockAudit.SetModifiedBlock(updatedBlock);

            // Assert
            blockAudit.ModifiedBlock.Should().Be(updatedBlock);
        }

        class FakeBlockAudit : BlockAudit
        {
            public FakeBlockAudit(ulong createdBlock, ulong modifiedBlock = 0)
                : base(createdBlock, modifiedBlock)
            {
            }

            internal new void SetModifiedBlock(ulong block) => base.SetModifiedBlock(block);
        }
    }
}