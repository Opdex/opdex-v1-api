using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Handlers.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Addresses
{
    public class RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQueryHandler _handler;

        public RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery_Send()
        {
            // Arrange
            var tokenId = 5L;
            var owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var findOrThrow = false;
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(new RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery(tokenId, owner, spender, findOrThrow),
                                  cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery>(
                query => query.TokenId == tokenId
                      && query.Owner == owner
                      && query.Spender == spender
                      && query.FindOrThrow == findOrThrow
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handler_SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery_Return()
        {
            // Arrange
            var selectResponse = new AddressAllowance(5, 5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", "500000000", 500, 505);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery>(),
                                                      It.IsAny<CancellationToken>()))
                         .ReturnsAsync(selectResponse);

            // Act
            var response = await _handler.Handle(new RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery(5,
                "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk"), default);

            // Assert
            response.Should().Be(selectResponse);
        }
    }
}
