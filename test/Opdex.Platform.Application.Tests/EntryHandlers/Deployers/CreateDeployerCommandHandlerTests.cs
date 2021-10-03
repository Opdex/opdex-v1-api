using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.EntryCommands.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.EntryHandlers.Deployers;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Deployers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Deployers
{
    public class CreateDeployerCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateDeployerCommandHandler _handler;

        public CreateDeployerCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateDeployerCommandHandler(_mediator.Object);
        }

        [Fact]
        public void CreateDeployerCommand_InvalidDeployer_ThrowsArgumentNullException()
        {
            // Arrange
            Address owner = "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN";
            const ulong blockHeight = 10;

            // Act
            void Act() => new CreateDeployerCommand(null, owner, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Deployer address must be provided.");
        }

        [Fact]
        public void CreateDeployerCommand_InvalidOwner_ThrowsArgumentNullException()
        {
            // Arrange
            Address deployer = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;

            // Act
            void Act() => new CreateDeployerCommand(deployer, Address.Empty, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Owner address must be provided.");
        }

        [Fact]
        public void CreateDeployerCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address owner = "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN";
            Address deployer = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";

            // Act
            void Act() => new CreateDeployerCommand(deployer, owner, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CreateDeployerCommand_Sends_RetrieveDeployerByAddressQuery()
        {
            // Arrange
            Address owner = "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN";
            Address deployer = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateDeployerCommand(deployer, owner, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveDeployerByAddressQuery>(q => q.Address == deployer &&
                                                                                              !q.FindOrThrow),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateDeployerCommand_Sends_MakeDeployerCommand_IsNewDeployer()
        {
            // Arrange
            Address owner = "P1iT1GLsMroh6zXXNMU9EjmivLgqqARwmH";
            Address deployer = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;

            // Act
            await _handler.Handle(new CreateDeployerCommand(deployer, owner, blockHeight), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeDeployerCommand>(q => q.Deployer.Address == deployer &&
                                                                                   q.Deployer.Id == 0 &&
                                                                                   q.Deployer.Owner == owner),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateDeployerCommand_Returns_NoUpdatesToMake()
        {
            // Arrange
            Address owner = "P1iT1GLsMroh6zXXNMU9EjmivLgqqARwmH";
            Address deployer = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
            const ulong blockHeight = 10;

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveDeployerByAddressQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new Deployer(1, deployer, Address.Empty, owner, true, 2, 3));

            // Act
            await _handler.Handle(new CreateDeployerCommand(deployer, owner, blockHeight), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<MakeDeployerCommand>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
