using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks;
using Opdex.Platform.Infrastructure.Data.Handlers.Blocks;
using System.Data;
using Xunit;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Blocks;

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
    public async Task PersistsBlockCommand_Sends_PersistCommand()
    {
        // Arrange
        var block = new Block(ulong.MaxValue, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
        var command = new PersistBlockCommand(block);

        // Act
        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch { }

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteCommandAsync(It.Is<DatabaseQuery>(q => q.Parameters != null &&
                                                                                         q.Type == CommandType.Text &&
                                                                                         q.Sql.Contains("INSERT INTO block"))), Times.Once);
    }

    [Fact]
    public async Task PersistsBlockCommand_Success()
    {
        // Arrange
        var block = new Block(ulong.MaxValue, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
        var command = new PersistBlockCommand(block);

        _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task PersistsBlockCommand_Fail()
    {
        // Arrange
        var block = new Block(ulong.MaxValue, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
        var command = new PersistBlockCommand(block);

        _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>())).ReturnsAsync(0);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().BeFalse();
    }
}