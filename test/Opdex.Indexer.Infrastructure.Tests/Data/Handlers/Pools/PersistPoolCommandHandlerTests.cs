using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;
using Opdex.Indexer.Infrastructure.Data.Handlers;
using Xunit;

namespace Opdex.Indexer.Infrastructure.Tests.Data.Handlers.Pools
{
    public class PersistPoolCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistPoolCommandHandler _handler;
        
        public PersistPoolCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new IndexerInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistPoolCommandHandler>();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new PersistPoolCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task PersistsPool_Success()
        {
            var pool = new Pool("PoolAddress", 1, 1, "1");
            var command = new PersistPoolCommand(pool);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1234L));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(1234);
        }

        [Fact]
        public async Task PersistsPool_Fail()
        {
            var pool = new Pool("PoolAddress", 1, 1, "1");
            var command = new PersistPoolCommand(pool);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(0);
        }
    }
}