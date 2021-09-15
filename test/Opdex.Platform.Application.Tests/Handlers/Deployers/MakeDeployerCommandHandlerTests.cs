using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Deployers;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Application.Handlers.Deployers;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Deployers
{
    public class MakeDeployerCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly MakeDeployerCommandHandler _handler;

        public MakeDeployerCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new MakeDeployerCommandHandler(_mediator.Object);
        }

        [Fact]
        public void MakeDeployerCommand_InvalidDeployer_ThrowsArgumentNullException()
        {
            // Arrange
            const ulong blockHeight = 10;

            // Act
            void Act() => new MakeDeployerCommand(null, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Deployer must be provided.");
        }

        [Fact]
        public void MakeDeployerCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var deployer = new Deployer(1, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMUARwmH1iT1GLsMroh6zXXN9EjmivLgqq", true, 3, 4);

            // Act
            void Act() => new MakeDeployerCommand(deployer, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task MakeDeployerCommand_Sends_CallCirrusGetSmartContractPropertyQuery_DuringRewind()
        {
            // Arrange
            const ulong blockHeight = 10;
            const string stateKey = MarketDeployerConstants.StateKeys.Owner;
            const SmartContractParameterType propertyType = SmartContractParameterType.Address;
            var deployer = new Deployer(1, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMUARwmH1iT1GLsMroh6zXXN9EjmivLgqq", true, 3, 4);
            deployer.RequireRewind();

            // Act
            try
            {
                await _handler.Handle(new MakeDeployerCommand(deployer, blockHeight), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetSmartContractPropertyQuery>(q => q.Contract == deployer.Address &&
                                                                                                       q.PropertyStateKey == stateKey &&
                                                                                                       q.PropertyType == propertyType &&
                                                                                                       q.BlockHeight == blockHeight),
                                                   CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task MakeDeployerCommand_Sends_PersistDeployerCommand_DuringRewind()
        {
            // Arrange
            const ulong blockHeight = 10;
            const SmartContractParameterType propertyType = SmartContractParameterType.Address;
            Address newOwner = "PMU9EjmGLsMroh6zXXNivLgqqARwmH1iT1";

            var deployer = new Deployer(1, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMUARwmH1iT1GLsMroh6zXXN9EjmivLgqq", true, 3, 4);
            deployer.RequireRewind();

            _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetSmartContractPropertyQuery>(), CancellationToken.None))
                .ReturnsAsync(new SmartContractMethodParameter(newOwner.ToString(), propertyType));

            // Act
            try
            {
                await _handler.Handle(new MakeDeployerCommand(deployer, blockHeight), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<PersistDeployerCommand>(q => q.Deployer.Id == deployer.Id &&
                                                                                      q.Deployer.Owner == newOwner &&
                                                                                      q.Deployer.ModifiedBlock == blockHeight),
                                                   CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task MakeDeployerCommand_Sends_PersistDeployerCommand_NoRewind()
        {
            // Arrange
            const ulong blockHeight = 10;
            var deployer = new Deployer(1, "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN", "PMUARwmH1iT1GLsMroh6zXXN9EjmivLgqq", true, 3, 4);

            // Act
            try
            {
                await _handler.Handle(new MakeDeployerCommand(deployer, blockHeight), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<PersistDeployerCommand>(q => q.Deployer.Id == deployer.Id &&
                                                                                      q.Deployer.Owner == deployer.Owner &&
                                                                                      q.Deployer.ModifiedBlock != blockHeight),
                                                   CancellationToken.None), Times.Once);

            _mediator.Verify(callTo => callTo.Send(It.IsAny<CallCirrusGetSmartContractPropertyQuery>(),
                                                   It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
