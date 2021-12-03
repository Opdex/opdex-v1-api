using System;
using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Blocks;

public class BlockTests
{
    [Fact]
    public void CreateBlock_InvalidHeight_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        static void Act() => new Block(0, new Sha256(5340958239), DateTime.UtcNow, DateTime.UtcNow);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }

    [Fact]
    public void CreateBlock_InvalidTime_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        static void Act() => new Block(1, new Sha256(5340958239), default, DateTime.UtcNow);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }

    [Fact]
    public void CreateBlock_InvalidMedianTime_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        static void Act() => new Block(1, new Sha256(5340958239), DateTime.UtcNow, default);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }

    [Fact]
    public void CreateBlock_Success()
    {
        const ulong height = ulong.MaxValue;
        Sha256 hash = new Sha256(4239843924835);
        var time = DateTime.UtcNow;
        var medianTime = DateTime.UtcNow;

        var block = new Block(height, hash, time, medianTime);

        block.Height.Should().Be(height);
        block.Hash.Should().Be(hash);
        block.Time.Should().Be(time);
        block.MedianTime.Should().Be(medianTime);
    }
}