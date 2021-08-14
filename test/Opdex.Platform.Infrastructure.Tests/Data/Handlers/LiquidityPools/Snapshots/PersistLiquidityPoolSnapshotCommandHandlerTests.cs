using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.LiquidityPools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools.Snapshots;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.LiquidityPools.Snapshots
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
        public async Task PersistsLiquidityPoolSnapshot_Insert_Success()
        {
            var snapshot = new LiquidityPoolSnapshot(1, SnapshotType.Daily, DateTime.UtcNow);
            var command = new PersistLiquidityPoolSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task PersistsLiquidityPoolSnapshot_Update_Success()
        {
            var snapshot = new LiquidityPoolSnapshot(1, 2, 3, new ReservesSnapshot(), new RewardsSnapshot(), new StakingSnapshot(), new VolumeSnapshot(),
                new CostSnapshot(), SnapshotType.Daily, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow);

            var command = new PersistLiquidityPoolSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task PersistLiquidityPoolSnapshot_Fail()
        {
            var snapshot = new LiquidityPoolSnapshot(1, SnapshotType.Daily, DateTime.UtcNow);
            var command = new PersistLiquidityPoolSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task PersistLiquidityPoolSnapshot_ThrownException()
        {
            var snapshot = new LiquidityPoolSnapshot(1, SnapshotType.Daily, DateTime.UtcNow);
            var command = new PersistLiquidityPoolSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Throws(new Exception());

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeFalse();
        }
    }
}
