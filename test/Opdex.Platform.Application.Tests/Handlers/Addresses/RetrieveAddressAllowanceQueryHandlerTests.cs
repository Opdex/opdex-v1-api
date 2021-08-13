using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Handlers.Addresses;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Addresses
{
    public class RetrieveAddressAllowanceQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RetrieveAddressAllowanceQueryHandler _handler;

        public RetrieveAddressAllowanceQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new RetrieveAddressAllowanceQueryHandler(_mediatorMock.Object);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void RetrieveAddressAllowance_ThrowsArgumentNullException_InvalidOwner(string owner)
        {
            // Arrange
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            Address spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

            // Act
            void Act() => new RetrieveAddressAllowanceQuery(owner, spender, token);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner must be provided.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void RetrieveAddressAllowance_ThrowsArgumentNullException_InvalidSpender(string spender)
        {
            // Arrange
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            Address owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

            // Act
            void Act() => new RetrieveAddressAllowanceQuery(owner, spender, token);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Spender must be provided.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void RetrieveAddressAllowance_ThrowsArgumentNullException_InvalidToken(string token)
        {
            // Arrange
            Address spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            Address owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";

            // Act
            void Act() => new RetrieveAddressAllowanceQuery(owner, spender, token);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Token must be provided.");
        }

        [Fact]
        public async Task RetrieveAddressAllowance_Sends_RetrieveTokenByAddressQuery()
        {
            // Arrange
            var request = new RetrieveAddressAllowanceQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                           "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                           "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK");

            var token = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, token);
            } catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(q => q.Address == request.Token),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RetrieveAddressAllowance_Sends_CallCirrusGetSrcTokenAllowanceQuery()
        {
            // Arrange
            var request = new RetrieveAddressAllowanceQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                            "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                            "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK");

            var cancellationToken = new CancellationTokenSource().Token;

            var token = new Token(1, "address", false, "Opdex", "ODX", 8, 100_000_000, "100", 1, 1);

            _mediatorMock
                .Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), cancellationToken))
                .ReturnsAsync(token);

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            } catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusGetSrcTokenAllowanceQuery>(q => q.Token == request.Token &&
                                                                                                       q.Owner == request.Owner &&
                                                                                                       q.Spender == request.Spender),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RetrieveAddressAllowance_Returns_AddressAllowance()
        {
            // Arrange
            var request = new RetrieveAddressAllowanceQuery("PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                                                            "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj",
                                                            "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK");

            var cancellationToken = new CancellationTokenSource().Token;

            var token = new Token(1, "address", false, "Opdex", "ODX", 8, 100_000_000, "100", 1, 1);

            var allowance = new UInt256("10000");

            _mediatorMock
                .Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), cancellationToken))
                .ReturnsAsync(token);

            _mediatorMock
                .Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetSrcTokenAllowanceQuery>(), cancellationToken))
                .ReturnsAsync(allowance);

            // Act
            var response = await _handler.Handle(request, cancellationToken);

            // Assert
            response.TokenId.Should().Be(1);
            response.Owner.Should().Be(request.Owner);
            response.Spender.Should().Be(request.Spender);
            response.Allowance.Should().Be(allowance);
        }
    }
}
