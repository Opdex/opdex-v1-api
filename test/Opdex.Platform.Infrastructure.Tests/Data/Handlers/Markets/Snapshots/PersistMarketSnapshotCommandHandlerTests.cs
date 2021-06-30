using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Common;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Pools.Snapshots;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Markets;
using Opdex.Platform.Infrastructure.Data.Handlers.Markets.Snapshots;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Markets.Snapshots
{
    public class PersistMarketSnapshotCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistMarketSnapshotCommandHandler _handler;

        public PersistMarketSnapshotCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistMarketSnapshotCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistMarketSnapshotCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task PersistsMarketSnapshot_Insert_Success()
        {
            var snapshot = new MarketSnapshot(1, SnapshotType.Daily, DateTime.UtcNow);
            var command = new PersistMarketSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task PersistsMarketSnapshot_Update_Success()
        {
            var snapshot = new MarketSnapshot(1, 2, 3m, 4m, new StakingSnapshot(), new RewardsSnapshot(), SnapshotType.Daily,
                                              DateTime.UtcNow, DateTime.UtcNow.AddDays(1));

            var command = new PersistMarketSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task PersistMarketSnapshot_Fail()
        {
            var snapshot = new MarketSnapshot(1, SnapshotType.Daily, DateTime.UtcNow);
            var command = new PersistMarketSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task PersistMarketSnapshot_ThrownException()
        {
            var snapshot = new MarketSnapshot(1, SnapshotType.Daily, DateTime.UtcNow);
            var command = new PersistMarketSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Throws(new Exception());

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeFalse();
        }
    }
}
