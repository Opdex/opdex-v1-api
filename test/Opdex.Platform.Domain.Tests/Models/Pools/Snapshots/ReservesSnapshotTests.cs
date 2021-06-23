using System;
using FluentAssertions;
using Opdex.Platform.Domain.Models.Pools.Snapshots;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Pools.Snapshots
{
    public class ReservesSnapshotTests
    {
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        [InlineData("1.23")]
        public void CreateReservesSnapshot_InvalidReserveSrc_ThrowsArgumentOutOfRangeException(string reserveSrc)
        {
            // Arrange
            // Act
            void Act() => new ReservesSnapshot(12345, reserveSrc, 25.00m);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(reserveSrc)} must be a numeric value.");
        }

        [Fact]
        public void CreateReservesSnapshot_InvalidReserveUsd_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const decimal reserveUsd = -1.00m;

            // Act
            void Act() => new ReservesSnapshot(12345, "9876", reserveUsd);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(reserveUsd)} must be greater or equal to 0.");
        }

        [Fact]
        public void CreateReservesSnapshot_Success()
        {
            // Arrange
            const ulong reserveCrs = 123;
            const string reserveSrc = "321";
            const decimal reserveUsd = 1.23m;

            // Act
            var snapshot = new ReservesSnapshot(reserveCrs, reserveSrc, reserveUsd);

            // Assert
            snapshot.Crs.Should().Be(reserveCrs);
            snapshot.Src.Should().Be(reserveSrc);
            snapshot.Usd.Should().Be(reserveUsd);
        }

        [Fact]
        public void CreateReservesSnapshot_Default_Success()
        {
            // Arrange
            // Act
            var snapshot = new ReservesSnapshot();

            // Assert
            snapshot.Crs.Should().Be(0ul);
            snapshot.Src.Should().Be("0");
            snapshot.Usd.Should().Be(0.00m);
        }
    }
}
