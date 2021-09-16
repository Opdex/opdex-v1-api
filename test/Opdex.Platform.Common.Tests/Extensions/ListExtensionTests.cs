using FluentAssertions;
using Opdex.Platform.Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Opdex.Platform.Common.Tests.Extensions
{
    public class ListExtensionTests
    {
        [Fact]
        public void Chunk_SplitsList()
        {
            // Arrange
            var example = new List<int> {1,2,3,4,5,6,7,8,9,10};
            const int itemsPer = 2;

            // Act
            var chunks = example.Chunk(itemsPer);

            // Assert
            chunks.Should().NotBeNullOrEmpty();
            chunks.Count().Should().Be(5);
        }

        [Fact]
        public void Chunk_SplitsList_WithRemainder()
        {
            // Arrange
            var example = new List<int> {1,2,3,4,5,6,7,8,9,10,11};
            const int itemsPer = 2;

            // Act
            var chunks = example.Chunk(itemsPer);

            // Assert
            chunks.Should().NotBeNullOrEmpty();
            chunks.Count().Should().Be(6);
            chunks.Last().First().Should().Be(11);
        }

        [Fact]
        public void Chunk_Splits_EmptyList_ReturnEmptyList()
        {
            // Arrange
            var example = new List<int>();
            const int itemsPer = 2;

            // Act
            var chunks = example.Chunk(itemsPer);

            // Assert
            chunks.Should().BeEmpty();
        }
    }
}
