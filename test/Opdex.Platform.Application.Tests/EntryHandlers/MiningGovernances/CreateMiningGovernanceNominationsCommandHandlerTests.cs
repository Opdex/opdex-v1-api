using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances.Nominations;
using Opdex.Platform.Application.EntryHandlers.MiningGovernances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.MiningGovernances
{
    public class CreateMiningGovernanceNominationsCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateMiningGovernanceNominationsCommandHandler _handler;

        public CreateMiningGovernanceNominationsCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateMiningGovernanceNominationsCommandHandler(_mediator.Object);
        }

        [Fact]
        public void CreateMiningGovernanceNominationsCommand_InvalidGovernance_ThrowsArgumentNullException()
        {
            // Arrange
            Address miningGovernance = Address.Empty;
            const ulong blockHeight = 10;

            // Act
            void Act() => new CreateMiningGovernanceNominationsCommand(miningGovernance, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Mining governance address must be provided.");
        }

        [Fact]
        public void CreateMiningGovernanceNominationsCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address miningGovernance = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            const ulong blockHeight = 0;

            // Act
            void Act() => new CreateMiningGovernanceNominationsCommand(miningGovernance, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CreateMiningGovernanceNominationsCommand_Sends_RetrieveMiningGovernanceByAddressQuery()
        {
            // Arrange
            Address miningGovernance = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateMiningGovernanceNominationsCommand(miningGovernance, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernanceByAddressQuery>(q => q.Address == miningGovernance &&
                                                                                                      q.FindOrThrow == true),
                                                                             It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateMiningGovernanceNominationsCommand_Sends_MakeMiningGovernanceNominationsCommand()
        {
            // Arrange
            Address miningGovernanceAddress = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            const ulong blockHeight = 10;

            var miningGovernance = new MiningGovernance(1, miningGovernanceAddress, 2, 3, 4, 5, 6, 7, 8);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(miningGovernance);

            // Act
            try
            {
                await _handler.Handle(new CreateMiningGovernanceNominationsCommand(miningGovernanceAddress, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeMiningGovernanceNominationsCommand>(q => q.MiningGovernance == miningGovernance &&
                                                                                                q.BlockHeight == blockHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
