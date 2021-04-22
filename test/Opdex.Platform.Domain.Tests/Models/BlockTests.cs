using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models
{
    public class BlockTests
    {
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