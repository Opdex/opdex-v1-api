using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Governances;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Governances;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Governances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Governances
{
    public class GetMiningGovernanceByAddressQueryHandlerTests
    {
        private readonly Mock<IModelAssembler<MiningGovernance, MiningGovernanceDto>> _assemblerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetMiningGovernanceByAddressQueryHandler _handler;

        public GetMiningGovernanceByAddressQueryHandlerTests()
        {
            _assemblerMock = new Mock<IModelAssembler<MiningGovernance, MiningGovernanceDto>>();
            _mediatorMock = new Mock<IMediator>();
            _handler = new GetMiningGovernanceByAddressQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
        }

        [Fact]
        public async Task RetrieveMiningGovernanceByAddressQuery_Send()
        {
            // Arrange
            const string governance = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var request = new GetMiningGovernanceByAddressQuery(governance);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernanceByAddressQuery>(query => query.Address == governance && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task HappyPath_MiningGovernance_Assemble()
        {
            // Arrange
            const string governance = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var request = new GetMiningGovernanceByAddressQuery(governance);
            var retrieveRequest = new RetrieveMiningGovernanceByAddressQuery(governance);
            var cancellationToken = new CancellationTokenSource().Token;

            var miningGovernance = new MiningGovernance(5, governance, 10, 100, 1000, 4, UInt256.Parse("500"), 10_000, 10_001);

            _mediatorMock.Setup(callTo => callTo.Send(retrieveRequest, cancellationToken)).ReturnsAsync(miningGovernance);

            // Act
            var response = await _handler.Handle(request, cancellationToken);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(It.IsAny<MiningGovernance>()), Times.Once());
        }
    }
}
