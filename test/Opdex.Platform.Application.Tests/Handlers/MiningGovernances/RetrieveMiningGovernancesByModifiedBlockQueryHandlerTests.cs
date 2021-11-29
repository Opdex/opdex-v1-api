using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Handlers.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.MiningGovernances
{
    public class RetrieveMiningGovernancesByModifiedBlockQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly RetrieveMiningGovernancesByModifiedBlockQueryHandler _handler;

        public RetrieveMiningGovernancesByModifiedBlockQueryHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new RetrieveMiningGovernancesByModifiedBlockQueryHandler(_mediator.Object);
        }

        [Fact]
        public void RetrieveMiningGovernancesByModifiedBlockQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            void Act() => new RetrieveMiningGovernancesByModifiedBlockQuery(0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task RetrieveMiningGovernancesByModifiedBlockQuery_Sends_SelectMiningGovernancesByModifiedBlockQuery()
        {
            // Arrange
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new RetrieveMiningGovernancesByModifiedBlockQuery(blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<SelectMiningGovernancesByModifiedBlockQuery>(q => q.BlockHeight == blockHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
