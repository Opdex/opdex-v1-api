using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Common;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.OHLC;
using Opdex.Platform.Domain.Models.Pools.Snapshot;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools.Snapshots;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Pools
{
    public class PersistLiquidityPoolSnapshotCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistLiquidityPoolSnapshotCommandHandler _handler;

        public PersistLiquidityPoolSnapshotCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistLiquidityPoolSnapshotCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistLiquidityPoolSnapshotCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task PersistsLiquidityPoolSnapshot_Success()
        {
            var snapshot = new LiquidityPoolSnapshot
            (
                0,
                1,
                2,
                new ReservesSnapshot("111", "123", 1.25m),
                new RewardsSnapshot(1234.23m, 987.21m),
                new StakingSnapshot("8765434", 37.21m),
                new VolumeSnapshot("333", "142", 1.28m),
                new CostSnapshot(new OhlcBigIntSnapshot("123", "321", "99", "321"), new OhlcBigIntSnapshot("321", "9876", "100", "602")),
                SnapshotType.Daily,
                DateTime.UtcNow,
                DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                DateTime.UtcNow
            );

            var command = new PersistLiquidityPoolSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task PersistsLiquidityPoolSnapshot_Fail()
        {
            var snapshot = new LiquidityPoolSnapshot
            (
                0,
                1,
                2,
                new ReservesSnapshot("111","123", 1.25m),
                new RewardsSnapshot(1234.23m, 987.21m),
                new StakingSnapshot("8765434", 37.21m),
                new VolumeSnapshot("333", "142", 1.28m),
                new CostSnapshot(new OhlcBigIntSnapshot("123", "321", "99", "321"), new OhlcBigIntSnapshot("321", "9876", "100", "602")),
                SnapshotType.Daily,
                DateTime.UtcNow,
                DateTime.UtcNow.Add(TimeSpan.FromDays(1)),
                DateTime.UtcNow
            );

            var command = new PersistLiquidityPoolSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeFalse();
        }
    }
}