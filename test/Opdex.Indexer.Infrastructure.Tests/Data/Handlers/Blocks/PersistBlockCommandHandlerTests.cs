using System;
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

namespace Opdex.Indexer.Infrastructure.Tests.Data.Handlers.Blocks
{
    public class PersistBlockCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistBlockCommandHandler _handler;
        
        public PersistBlockCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new IndexerInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistBlockCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistBlockCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task PersistsBlock_Success()
        {
            var block = new Block(ulong.MaxValue, "BlockHash", DateTime.UtcNow, DateTime.UtcNow);
            var command = new PersistBlockCommand(block);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task PersistsBlock_Fail()
        {
            var block = new Block(ulong.MaxValue, "BlockHash", DateTime.UtcNow, DateTime.UtcNow);
            var command = new PersistBlockCommand(block);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeFalse();
        }
    }
}