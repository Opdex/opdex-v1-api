using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses
{
    public class GetAddressAllowanceForTokenQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetAddressAllowanceForTokenQueryHandler _handler;

        public GetAddressAllowanceForTokenQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();
            _handler = new GetAddressAllowanceForTokenQueryHandler(_mapperMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_RetrieveTokenByAddressQuery_Send()
        {
            // Arrange
            var token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var request = new GetAddressAllowanceForTokenQuery(token, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl");
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == token && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery_Send()
        {
            // Arrange
            var tokenId = 5L;
            var owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            var spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl";
            var request = new GetAddressAllowanceForTokenQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", owner, spender);
            var cancellationToken = new CancellationTokenSource().Token;

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new Token(tokenId, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", false, "Wrapped BTC", "WBTC", 8, 1000000, "2100000000000000", 5, 10));

            // Act
            await _handler.Handle(request, cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery>(query => query.TokenId == tokenId
                                                                                                                             && query.Owner == owner
                                                                                                                             && query.Spender == spender
                                                                                                                             && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_AddressAllowanceFound_ReturnMapped()
        {
            // Arrange
            var request = new GetAddressAllowanceForTokenQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl");
            var addressAllowance = new AddressAllowance(5, 5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk", "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl", "50000000", 5, 5);
            var addressAllowanceDto = new AddressAllowanceDto { Amount = "5000000000" };

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new Token(5, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", false, "Wrapped BTC", "WBTC", 8, 1000000, "2100000000000000", 5, 10));
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(addressAllowance);

            _mapperMock.Setup(callTo => callTo.Map<AddressAllowanceDto>(addressAllowance)).Returns(addressAllowanceDto);

            // Act
            var response = await _handler.Handle(request, CancellationToken.None);

            // Assert
            response.Should().Be(addressAllowanceDto);
        }
    }
}
