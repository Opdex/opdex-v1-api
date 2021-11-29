using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Queries.MiningGovernances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.MiningGovernances;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.MiningGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.MiningGovernances
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
            const string miningGovernanceAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var request = new GetMiningGovernanceByAddressQuery(miningGovernanceAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernanceByAddressQuery>(query => query.Address == miningGovernanceAddress && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task HappyPath_MiningGovernance_Assemble()
        {
            // Arrange
            const string miningGovernanceAddress = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var request = new GetMiningGovernanceByAddressQuery(miningGovernanceAddress);
            var retrieveRequest = new RetrieveMiningGovernanceByAddressQuery(miningGovernanceAddress);
            var cancellationToken = new CancellationTokenSource().Token;

            var miningGovernance = new MiningGovernance(5, miningGovernanceAddress, 10, 100, 1000, 4, UInt256.Parse("500"), 10_000, 10_001);

            _mediatorMock.Setup(callTo => callTo.Send(retrieveRequest, cancellationToken)).ReturnsAsync(miningGovernance);

            // Act
            var response = await _handler.Handle(request, cancellationToken);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(It.IsAny<MiningGovernance>()), Times.Once());
        }
    }
}
