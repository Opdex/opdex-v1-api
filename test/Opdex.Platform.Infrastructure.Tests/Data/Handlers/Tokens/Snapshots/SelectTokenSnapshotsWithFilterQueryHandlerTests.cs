using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Snapshots;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Snapshots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens.Snapshots
{
    public class SelectTokenSnapshotsWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectTokenSnapshotsWithFilterQueryHandler _handler;

        public SelectTokenSnapshotsWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectTokenSnapshotsWithFilterQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectTokenSnapshotsWithFilter_Success()
        {
            const ulong tokenId = 8;
            const ulong marketId = 4;
            var startDate = new DateTime(2021, 6, 21, 12, 0, 0);
            var endDate = new DateTime(2021, 6, 21, 15, 0, 0);
            const SnapshotType snapshotType = SnapshotType.Daily;

            var expectedEntity = new TokenSnapshotEntity
            {
                Id = 1,
                MarketId = marketId,
                TokenId = tokenId,
                SnapshotTypeId = (int)snapshotType,
                StartDate = new DateTime(2021, 6, 21),
                EndDate = new DateTime(2021, 6, 21, 23, 59, 59),
                ModifiedDate = DateTime.UtcNow,
                Price = new OhlcEntity<decimal> { Open = 1.23m, High = 9.87m, Low = 1.1m, Close = 4.87m },
            };

            var entities = new List<TokenSnapshotEntity> { expectedEntity };

            var cursor = new SnapshotCursor(Interval.OneHour, startDate, endDate, default, default, PagingDirection.Forward, default);
            var command = new SelectTokenSnapshotsWithFilterQuery(tokenId, marketId, cursor);

            _dbContext.Setup(db => db.ExecuteQueryAsync<TokenSnapshotEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(entities.AsEnumerable()));

            var results = await _handler.Handle(command, CancellationToken.None);

            var resultsList = results.ToList();

            for (var i = 0; i < resultsList.Count; i++)
            {
                var result = resultsList[i];

                result.Id.Should().Be(expectedEntity.Id);
                result.TokenId.Should().Be(tokenId);
                result.MarketId.Should().Be(marketId);
                result.Price.Open.Should().Be(expectedEntity.Price.Open);
                result.Price.High.Should().Be(expectedEntity.Price.High);
                result.Price.Low.Should().Be(expectedEntity.Price.Low);
                result.Price.Close.Should().Be(expectedEntity.Price.Close);
                result.StartDate.Should().Be(expectedEntity.StartDate);
                result.EndDate.Should().Be(expectedEntity.EndDate);
                result.ModifiedDate.Should().Be(expectedEntity.ModifiedDate);
            }
        }

        [Fact]
        public async Task SelectTokenSnapshotsWithFilter_Returns_EmptyList()
        {
            const ulong tokenId = 1;
            const ulong marketId = 2;
            var startDate = new DateTime(2021, 6, 21, 12, 0, 0);
            var endDate = new DateTime(2021, 6, 21, 15, 0, 0);

            var cursor = new SnapshotCursor(Interval.OneDay, startDate, endDate, default, default, PagingDirection.Forward, default);
            var command = new SelectTokenSnapshotsWithFilterQuery(tokenId, marketId, cursor);

            _dbContext.Setup(db => db.ExecuteQueryAsync<TokenSnapshotEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<IEnumerable<TokenSnapshotEntity>>(null));

            var results = await _handler.Handle(command, CancellationToken.None);

            results.Count().Should().Be(0);
        }
    }
}
