using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Indexer.Infrastructure.Abstractions.Data.Commands;
using Opdex.Indexer.Infrastructure.Data.Handlers;
using Xunit;

namespace Opdex.Indexer.Infrastructure.Tests.Data.Handlers
{
    public class PersistPairCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistPairCommandHandler _handler;
        
        public PersistPairCommandHandlerTests()
        {
            _dbContext = new Mock<IDbContext>();
            var mapper = new MapperConfiguration(config => config.AddProfile(new IndexerInfrastructureMapperProfile())).CreateMapper();
            _handler = new PersistPairCommandHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task PersistsPair_Success()
        {
            var pair = new Pair("PairAddress", 1, 1m, 1m);
            var command = new PersistPairCommand(pair);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1234L));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(1234);
        }

        [Fact]
        public async Task PersistsPair_Fail()
        {
            var pair = new Pair("PairAddress", 1, 1m, 1m);
            var command = new PersistPairCommand(pair);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(0);
        }
    }
}