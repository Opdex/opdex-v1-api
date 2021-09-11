using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Addresses;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses
{
    public class GetAddressBalanceByTokenQueryHandlerTests
    {
        private readonly Mock<IModelAssembler<AddressBalance, AddressBalanceDto>> _assemblerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetAddressBalanceByTokenQueryHandler _handler;

        public GetAddressBalanceByTokenQueryHandlerTests()
        {
            _assemblerMock = new Mock<IModelAssembler<AddressBalance, AddressBalanceDto>>();
            _mediatorMock = new Mock<IMediator>();
            _handler = new GetAddressBalanceByTokenQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
        }

        [Fact]
        public async Task Handle_RetrieveAddressBalanceByTokenAddressAndOwnerQuery_Send()
        {
            // Arrange
            Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(new GetAddressBalanceByTokenQuery(address, token), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveAddressBalanceByOwnerAndTokenQuery>(
                query => query.Owner == address
                      && query.TokenAddress == token
                      && query.FindOrThrow
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_Assembler_Return()
        {
            // Arrange
            var dto = new AddressBalanceDto();

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalanceByOwnerAndTokenQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new AddressBalance(5, 10, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", 500000000, 500, 505));
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressBalance>())).ReturnsAsync(dto);

            // Act
            var response = await _handler.Handle(new GetAddressBalanceByTokenQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"), CancellationToken.None);

            // Assert
            response.Should().Be(dto);
        }
    }
}
