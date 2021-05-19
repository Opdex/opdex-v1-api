using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Pools;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Pools;
using Opdex.Platform.Infrastructure.Data.Handlers.Pools;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Pools
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
        public async Task PersistsPool_Success()
        {
            var pool = new LiquidityPool("PoolAddress", 1, 1, 2, 3);
            var command = new PersistLiquidityPoolCommand(pool);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1234L));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(1234);
        }

        [Fact]
        public async Task PersistsPool_Fail()
        {
            var pool = new LiquidityPool("PoolAddress", 1, 1, 2, 3);
            var command = new PersistLiquidityPoolCommand(pool);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(0);
        }
    }
}