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

namespace Opdex.Indexer.Infrastructure.Tests.Data.Handlers
{
    public class PersistTokenCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTokenCommandHandler _handler;
        
        public PersistTokenCommandHandlerTests()
        {
            _dbContext = new Mock<IDbContext>();
            var mapper = new MapperConfiguration(config => config.AddProfile(new IndexerInfrastructureMapperProfile())).CreateMapper();
            _handler = new PersistTokenCommandHandler(_dbContext.Object, mapper, new NullLogger<PersistTokenCommandHandler>());
        }

        [Fact]
        public async Task PersistsToken_Success()
        {
            var token = new Token("TokenAddress", "TokenName", "TKN", 8, 100_000_000, 500_000_000);
            var command = new PersistTokenCommand(token);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1234L));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(1234);
        }

        [Fact]
        public async Task PersistsToken_Fail()
        {
            var token = new Token("TokenAddress", "TokenName", "TKN", 8, 100_000_000, 500_000_000);
            var command = new PersistTokenCommand(token);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(0);
        }
    }
}