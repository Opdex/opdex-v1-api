using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.LiquidityPools;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.LiquidityPools
{
    public class PersistLiquidityPoolCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistLiquidityPoolCommandHandler _handler;

        public PersistLiquidityPoolCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistLiquidityPoolCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistLiquidityPoolCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task InsertLiquidityPool_Success()
        {
            var pool = new LiquidityPool("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", 1, 4, 1, 2);
            var command = new PersistLiquidityPoolCommand(pool);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1234L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(1234);
        }

        [Fact]
        public async Task UpdateLiquidityPool_Success()
        {
            const long id = 99;

            var pool = new LiquidityPool(id, "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", 2, 3, 4, 5, 6);
            var command = new PersistLiquidityPoolCommand(pool);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(id));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(id);
        }

        [Fact]
        public async Task InsertLiquidityPool_Fail()
        {
            var pool = new LiquidityPool("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", 1, 4, 1, 2);
            var command = new PersistLiquidityPoolCommand(pool);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(0);
        }

        [Fact]
        public async Task InsertMiningPool_Throws()
        {
            var pool = new LiquidityPool("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", 1, 4, 1, 2);
            var command = new PersistLiquidityPoolCommand(pool);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>())).Throws<Exception>();

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(0);
        }
    }
}
