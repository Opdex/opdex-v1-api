using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Handlers.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Addresses
{
    public class RetrieveAddressAllowancesByOwnerWithFilterQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RetrieveAddressAllowancesByOwnerWithFilterQueryHandler _handler;

        public RetrieveAddressAllowancesByOwnerWithFilterQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new RetrieveAddressAllowancesByOwnerWithFilterQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery_Send()
        {
            // Arrange
            var owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var tokenId = 5L;
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(new RetrieveAddressAllowancesByOwnerWithFilterQuery(owner, spender, tokenId), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<SelectAddressAllowancesByOwnerWithFilterQuery>(
                query => query.Owner == owner
                      && query.Spender == spender
                      && query.TokenId == tokenId
            ), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handler_SelectAddressAllowanceByTokenIdAndOwnerAndSpenderQuery_Return()
        {
            // Arrange
            var selectResponse = new List<AddressAllowance>
            {
                new AddressAllowance(5, 5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", "500000000", 500, 505)
            };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<SelectAddressAllowancesByOwnerWithFilterQuery>(),
                                                      It.IsAny<CancellationToken>()))
                         .ReturnsAsync(selectResponse);

            // Act
            var response = await _handler.Handle(new RetrieveAddressAllowancesByOwnerWithFilterQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj"), CancellationToken.None);

            // Assert
            response.Should().BeEquivalentTo(selectResponse);
        }
    }
}
