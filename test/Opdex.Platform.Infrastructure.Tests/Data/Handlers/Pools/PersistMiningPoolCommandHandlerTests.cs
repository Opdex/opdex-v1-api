using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Pools
{
    public class PersistMiningPoolCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistMiningPoolCommandHandler _handler;

        public PersistMiningPoolCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistMiningPoolCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistMiningPoolCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task InsertMiningPool_Success()
        {
            var pool = new MiningPool(1, "Address", 2);
            var command = new PersistMiningPoolCommand(pool);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1234L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(1234);
        }

        [Fact]
        public async Task UpdateMiningPool_Success()
        {
            const long id = 99;

            var pool = new MiningPool(id, 2, "Address", "3", "4", 5, 6, 7);
            var command = new PersistMiningPoolCommand(pool);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(id));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(id);
        }

        [Fact]
        public async Task InsertMiningPool_Fail()
        {
            var pool = new MiningPool(1, "Address", 2);
            var command = new PersistMiningPoolCommand(pool);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(0);
        }

        [Fact]
        public async Task InsertMiningPool_Throws()
        {
            var pool = new MiningPool(1, "Address", 2);
            var command = new PersistMiningPoolCommand(pool);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>())).Throws<Exception>();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(0);
        }
    }
}
