using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.EntryHandlers.Deployers;
using Opdex.Platform.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Deployers
{
    public class CreateRewindDeployersCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateRewindDeployersCommandHandler _handler;

        public CreateRewindDeployersCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateRewindDeployersCommandHandler(_mediator.Object);
        }

        [Fact]
        public void CreateRewindDeployersCommand_InvalidRewindHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const ulong rewindHeight = 0;

            // Act
            void Act() => new CreateRewindDeployersCommand(rewindHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Rewind height must be greater than zero.");
        }

        [Fact]
        public async Task CreateRewindDeployersCommand_Sends_RetrieveDeployersByModifiedBlockQuery()
        {
            // Arrange
            const ulong rewindHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateRewindDeployersCommand(rewindHeight), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveDeployersByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateRewindDeployersCommand_Sends_MakeDeployerCommand()
        {
            // Arrange
            const ulong rewindHeight = 10;

            var deployers = new List<Deployer>
            {
                new Deployer(1, "PT1GLsMroh6zXXNMU9EjmivLgqqARwmH1i", "PT1GLsMroLgqqARwmH1ih6zXXNMU9Ejmiv", true, 2, 3),
                new Deployer(2, "PMU9EjmivLgh6zXXNqqARwmH1iT1GLsMro", "PMUqARwmH1iT1GLsMro9EjmivLgh6zXXNq", true, 3, 4),
                new Deployer(3, "PMULsMroh6zXXN9EjmivLgqqARwmH1iT1G", "PMULsMroh6qqARwmH1iT1GzXXN9EjmivLg", true, 4, 5),
                new Deployer(4, "PMU9Ej6zXXNmivLgqqARwmH1iT1GLsMroh", "PXNmivLgqqARwmH1iT1GLsMrohMU9Ej6zX", true, 5, 6)
            };

            _mediator.Setup(callTo => callTo.Send(It.Is<RetrieveDeployersByModifiedBlockQuery>(q => q.BlockHeight == rewindHeight),
                                                  It.IsAny<CancellationToken>())).ReturnsAsync(deployers);

            // Act
            await _handler.Handle(new CreateRewindDeployersCommand(rewindHeight), CancellationToken.None);

            // Assert
            foreach (var deployer in deployers)
            {
                _mediator.Verify(callTo => callTo.Send(It.Is<MakeDeployerCommand>(q => q.BlockHeight == rewindHeight &&
                                                                                       q.Deployer.Id == deployer.Id &&
                                                                                       q.Deployer.Address == deployer.Address &&
                                                                                       q.Deployer.Owner == deployer.Owner &&
                                                                                       q.Deployer.Rewind == true &&
                                                                                       q.Deployer.ModifiedBlock == deployer.ModifiedBlock),
                                                       It.IsAny<CancellationToken>()), Times.Once);
            }
        }
    }
}
