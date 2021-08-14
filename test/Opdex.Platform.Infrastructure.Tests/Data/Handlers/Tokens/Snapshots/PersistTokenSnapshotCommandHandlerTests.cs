using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.OHLC;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Snapshots;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens.Snapshots
{
    public class PersistTokenSnapshotCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTokenSnapshotCommandHandler _handler;

        public PersistTokenSnapshotCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistTokenSnapshotCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistTokenSnapshotCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task PersistsTokenSnapshot_Insert_Success()
        {
            var snapshot = new TokenSnapshot(1, 2, SnapshotType.Daily, DateTime.UtcNow);
            var command = new PersistTokenSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task PersistsTokenSnapshot_Update_Success()
        {
            var snapshot = new TokenSnapshot(1, 2, 3, new OhlcDecimalSnapshot(), SnapshotType.Daily, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow);

            var command = new PersistTokenSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task PersistTokenSnapshot_Fail()
        {
            var snapshot = new TokenSnapshot(1, 2, SnapshotType.Daily, DateTime.UtcNow);
            var command = new PersistTokenSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeFalse();
        }

        [Fact]
        public async Task PersistTokenSnapshot_ThrownException()
        {
            var snapshot = new TokenSnapshot(1, 3, SnapshotType.Daily, DateTime.UtcNow);
            var command = new PersistTokenSnapshotCommand(snapshot);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Throws(new Exception());

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeFalse();
        }
    }
}
