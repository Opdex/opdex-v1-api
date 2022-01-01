using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Application.Handlers.Indexer;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Indexer;

public class MakeIndexerLockCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeIndexerLockCommandHandler _handler;

    public MakeIndexerLockCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeIndexerLockCommandHandler(_mediator.Object);
    }

    [Fact]
    public async Task Handle_Send_PersistIndexerLockCommand()
    {
        // Arrange
        var token = CancellationToken.None;
        var reason = IndexLockReason.Indexing;

        // Act
        try
        {
            await _handler.Handle(new MakeIndexerLockCommand(reason), token);
        }
        catch (Exception) { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistIndexerLockCommand>(command => command.Reason == reason), CancellationToken.None), Times.Once);
    }
}
