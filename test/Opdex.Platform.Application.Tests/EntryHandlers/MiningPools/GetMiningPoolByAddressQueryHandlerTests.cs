using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.MiningPools;
using Opdex.Platform.Domain.Models.MiningPools;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.MiningPools
{
    public class GetMiningPoolByAddressQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<MiningPool, MiningPoolDto>> _miningPoolAssemblerMock;

        private readonly GetMiningPoolByAddressQueryHandler _handler;

        public GetMiningPoolByAddressQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _miningPoolAssemblerMock = new Mock<IModelAssembler<MiningPool, MiningPoolDto>>();
            _handler = new GetMiningPoolByAddressQueryHandler(_mediatorMock.Object, _miningPoolAssemblerMock.Object);
        }

        [Fact]
        public async Task Handler_RetrieveMiningPoolByAddressQuery_Send()
        {
            // Arrange
            var miningPool = "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(new GetMiningPoolByAddressQuery(miningPool), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMiningPoolByAddressQuery>(query => query.Address == miningPool
                                                                                                && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_MiningPoolDto_Assemble()
        {
            // Arrange
            var miningPool = new MiningPool(5, 10, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", "500201035", "4249200", 530039, 505, 510);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPool);

            // Act
            await _handler.Handle(new GetMiningPoolByAddressQuery("PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm"), CancellationToken.None);

            // Assert
            _miningPoolAssemblerMock.Verify(callTo => callTo.Assemble(miningPool), Times.Once);
        }

        [Fact]
        public async Task Handle_MiningPoolDto_Return()
        {
            // Arrange
            var dto = new MiningPoolDto();

            var miningPool = new MiningPool(5, 10, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", "500201035", "4249200", 530039, 505, 510);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPool);
            _miningPoolAssemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningPool>())).ReturnsAsync(dto);

            // Act
            var response = await _handler.Handle(new GetMiningPoolByAddressQuery("PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm"), CancellationToken.None);

            // Assert
            response.Should().Be(dto);
        }
    }
}
