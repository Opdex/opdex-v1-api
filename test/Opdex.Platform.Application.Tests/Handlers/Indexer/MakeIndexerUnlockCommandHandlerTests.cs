using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Indexer;
using Opdex.Platform.Application.Handlers.Indexer;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Indexer;

public class MakeIndexerUnlockCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeIndexerUnlockCommandHandlerWrapper _handler;

    public MakeIndexerUnlockCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeIndexerUnlockCommandHandlerWrapper(_mediator.Object);
    }

    [Fact]
    public async Task Send_PersistIndexerUnlockCommand()
    {
        // Arrange
        var token = CancellationToken.None;

        // Act
        try
        {
            await _handler.Handle(new MakeIndexerUnlockCommand(), token);
        }
        catch (Exception) { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<PersistIndexerUnlockCommand>(), token), Times.Once);
    }

    public class MakeIndexerUnlockCommandHandlerWrapper : MakeIndexerUnlockCommandHandler
    {
        public MakeIndexerUnlockCommandHandlerWrapper(IMediator mediator) : base(mediator)
        {
        }

        public new async Task Handle(MakeIndexerUnlockCommand command, CancellationToken cancellationToken)
        {
            await base.Handle(command, cancellationToken);
        }
    }
}