using FluentAssertions;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.OHLC;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using Xunit;

namespace Opdex.Platform.Application.Tests.Mappers
{
    public class TokensApplicationMapperProfileTests : PlatformApplicationMapperProfileTests
    {
        [Fact]
        public void From_TokenSnapshot_To_TokenSnapshotDto()
        {
            // Arrange
            var tokenSnapshot = new TokenSnapshot(5, 10, 15, new OhlcDecimalSnapshot(15.0m, 20.0m, 10.5m, 12.0m), SnapshotType.Daily, DateTime.UtcNow.AddDays(-1), DateTime.UtcNow, DateTime.UtcNow);

            // Act
            var dto = _mapper.Map<TokenSnapshotDto>(tokenSnapshot);

            // Assert
            dto.Price.Open.Should().Be(tokenSnapshot.Price.Open);
            dto.Price.Close.Should().Be(tokenSnapshot.Price.Close);
            dto.Price.High.Should().Be(tokenSnapshot.Price.High);
            dto.Price.Low.Should().Be(tokenSnapshot.Price.Low);
            dto.Timestamp.Should().Be(tokenSnapshot.StartDate);
        }
    }
}
