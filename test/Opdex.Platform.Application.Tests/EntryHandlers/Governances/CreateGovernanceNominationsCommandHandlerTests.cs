using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances.Nominations;
using Opdex.Platform.Application.EntryHandlers.Governances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Governances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Governances
{
    public class CreateGovernanceNominationsCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateGovernanceNominationsCommandHandler _handler;

        public CreateGovernanceNominationsCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateGovernanceNominationsCommandHandler(_mediator.Object);
        }

        [Fact]
        public void CreateGovernanceNominationsCommand_InvalidGovernance_ThrowsArgumentNullException()
        {
            // Arrange
            Address governance = Address.Empty;
            const ulong blockHeight = 10;

            // Act
            void Act() => new CreateGovernanceNominationsCommand(governance, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Governance address must be provided.");
        }

        [Fact]
        public void CreateGovernanceNominationsCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address governance = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            const ulong blockHeight = 0;

            // Act
            void Act() => new CreateGovernanceNominationsCommand(governance, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CreateGovernanceNominationsCommand_Sends_RetrieveMiningGovernanceByAddressQuery()
        {
            // Arrange
            Address governance = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateGovernanceNominationsCommand(governance, blockHeight), CancellationToken.None);
            } catch {}

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernanceByAddressQuery>(q => q.Address == governance &&
                                                                                                      q.FindOrThrow == true),
                                                                             It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateGovernanceNominationsCommand_Sends_MakeGovernanceNominationsCommand()
        {
            // Arrange
            Address governanceAddress = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            const ulong blockHeight = 10;

            var governance = new MiningGovernance(1, governanceAddress, 2, 3, 4, 5, 6, 7, 8);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernanceByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(governance);

            // Act
            try
            {
                await _handler.Handle(new CreateGovernanceNominationsCommand(governanceAddress, blockHeight), CancellationToken.None);
            } catch {}

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeGovernanceNominationsCommand>(q => q.Governance == governance &&
                                                                                                q.BlockHeight == blockHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
