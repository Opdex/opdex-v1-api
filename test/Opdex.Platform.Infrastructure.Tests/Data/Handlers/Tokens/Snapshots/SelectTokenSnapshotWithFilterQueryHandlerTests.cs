using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.OHLC;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Snapshots;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens.Snapshots
{
    public class SelectTokenSnapshotWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectTokenSnapshotWithFilterQueryHandler _handler;

        public SelectTokenSnapshotWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectTokenSnapshotWithFilterQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectTokenSnapshotWithFilter_Success()
        {
            const  ulong tokenId = 8;
            const ulong marketId = 9;
            var blockTime = new DateTime(2021, 6, 21, 12, 0, 0);
            const SnapshotType snapshotType = SnapshotType.Daily;

            var expectedEntity = new TokenSnapshotEntity
            {
                Id = 123454,
                TokenId = tokenId,
                MarketId = marketId,
                SnapshotTypeId = (int)snapshotType,
                StartDate = new DateTime(2021, 6, 21),
                EndDate = new DateTime(2021, 6, 21, 23, 59, 59),
                ModifiedDate = DateTime.UtcNow,
                Price = new OhlcDecimalEntity { Open = 1m, High = 5m, Low = .5m, Close = 4m}
            };

            var command = new SelectTokenSnapshotWithFilterQuery(tokenId, marketId, blockTime, snapshotType);

            _dbContext.Setup(db => db.ExecuteFindAsync<TokenSnapshotEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedEntity));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedEntity.Id);
            result.TokenId.Should().Be(tokenId);
            result.MarketId.Should().Be(marketId);
            result.SnapshotType.Should().Be(snapshotType);
            result.StartDate.Should().Be(expectedEntity.StartDate);
            result.EndDate.Should().Be(expectedEntity.EndDate);
            result.ModifiedDate.Should().Be(expectedEntity.ModifiedDate);
            result.Price.Open.Should().Be(expectedEntity.Price.Open);
            result.Price.High.Should().Be(expectedEntity.Price.High);
            result.Price.Low.Should().Be(expectedEntity.Price.Low);
            result.Price.Close.Should().Be(expectedEntity.Price.Close);
        }

        [Fact]
        public async Task SelectTokenSnapshotWithFilter_Returns_NewInstance()
        {
            const  ulong tokenId = 1;
            const ulong marketId = 4;
            var blockTime = new DateTime(2021, 6, 21, 12, 0, 0);
            const SnapshotType snapshotType = SnapshotType.Daily;

            var command = new SelectTokenSnapshotWithFilterQuery(tokenId, marketId, blockTime, snapshotType);

            _dbContext.Setup(db => db.ExecuteFindAsync<TokenSnapshotEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<TokenSnapshotEntity>(null));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().NotBeNull();
            result.EndDate.Should().Be(blockTime.ToEndOf(snapshotType));
            result.StartDate.Should().Be(blockTime.ToStartOf(snapshotType));
            result.TokenId.Should().Be(tokenId);
            result.MarketId.Should().Be(marketId);
            result.SnapshotType.Should().Be(snapshotType);
            result.Price.Open.Should().Be(0m);
            result.Price.High.Should().Be(0m);
            result.Price.Low.Should().Be(0m);
            result.Price.Close.Should().Be(0m);
        }
    }
}
