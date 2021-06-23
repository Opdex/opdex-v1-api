using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Opdex.Platform.Infrastructure.Data.Handlers;
using Opdex.Platform.Infrastructure.Data.Handlers.Blocks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Blocks
{
    public class PersistBlockCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistBlockCommandHandler _handler;
        
        public PersistBlockCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
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