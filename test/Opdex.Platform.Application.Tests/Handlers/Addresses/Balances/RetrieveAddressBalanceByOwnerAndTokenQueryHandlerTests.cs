using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Handlers.Addresses.Balances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Addresses.Balances
{
    public class RetrieveAddressBalanceByOwnerAndTokenQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly RetrieveAddressBalanceByOwnerAndTokenQueryHandler _handler;

        public RetrieveAddressBalanceByOwnerAndTokenQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _handler = new RetrieveAddressBalanceByOwnerAndTokenQueryHandler(_mediatorMock.Object);
        }

        [Fact]
        public void SelectAddressBalanceByOwnerAndTokenId_ThrowsArgumentNullException_InvalidOwner()
        {
            // Arrange
            Address owner = Address.Empty;
            const ulong tokenId = 2;

            void Act() => new RetrieveAddressBalanceByOwnerAndTokenQuery(owner, tokenId);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner address must be set.");
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData(null, 0ul)]
        [InlineData("", null)]
        [InlineData("", 0ul)]
        [InlineData(" ", null)]
        [InlineData(" ", 0ul)]
        public void SelectAddressBalanceByOwnerAndTokenId_ThrowsArgumentException_InvalidTokenIdAndAddress(string owner, ulong? tokenId)
        {
            // Arrange

            // Act
            void Act() => new RetrieveAddressBalanceByOwnerAndTokenQuery(owner, tokenId);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner address must be set.");
        }

        [Fact]
        public async Task SelectAddressBalanceByOwnerAndTokenId_Sends_RetrieveTokenByAddressQuery()
        {
            // Arrange
            Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const string token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(new RetrieveAddressBalanceByOwnerAndTokenQuery(address, tokenAddress: token), cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(q => q.Address == token &&
                                                                                               q.FindOrThrow),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalanceByOwnerAndTokenId_Sends_RetrieveTokenByIdQuery()
        {
            // Arrange
            Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong tokenId = 2;
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(new RetrieveAddressBalanceByOwnerAndTokenQuery(address, tokenId), cancellationToken);
            }
            catch { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByIdQuery>(q => q.TokenId == tokenId &&
                                                                                          q.FindOrThrow),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalanceByOwnerAndTokenId_TokenNotFound_FindOrThrowFalse_ReturnsNull()
        {
            // Arrange
            Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong tokenId = 2;
            const bool findOrThrow = false;
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            var response = await _handler.Handle(new RetrieveAddressBalanceByOwnerAndTokenQuery(address, tokenId,
                                                                                                findOrThrow: findOrThrow), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByIdQuery>(q => q.TokenId == tokenId &&
                                                                                          q.FindOrThrow == findOrThrow),
                                                       cancellationToken), Times.Once);

            response.Should().Be(null);
        }

        [Fact]
        public async Task SelectAddressBalanceByOwnerAndTokenId_Sends_SelectAddressBalanceByOwnerAndTokenIdQuery()
        {
            // Arrange
            Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong tokenId = 2;
            const bool findOrThrow = false;
            var cancellationToken = new CancellationTokenSource().Token;

            _mediatorMock
                .Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), cancellationToken))
                .ReturnsAsync(new Token(tokenId, "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", false, "name", "symbol", 8, 100_000_000, UInt256.Parse("100"), 1, 2));

            // Act
            await _handler.Handle(new RetrieveAddressBalanceByOwnerAndTokenQuery(address, tokenId, findOrThrow: findOrThrow), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<SelectAddressBalanceByOwnerAndTokenIdQuery>(q => q.Owner == address &&
                                                                                                              q.TokenId == tokenId &&
                                                                                                              q.FindOrThrow == findOrThrow),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalanceByOwnerAndTokenId_ReturnsSrcTokenAddressBalance()
        {
            // Arrange
            Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong tokenId = 2;
            const bool findOrThrow = false;
            var cancellationToken = new CancellationTokenSource().Token;

            var expectedResult = new AddressBalance(tokenId, address, 100, 1);

            _mediatorMock
                .Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), cancellationToken))
                .ReturnsAsync(new Token(tokenId, "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy", false, "name", "symbol", 8, 100_000_000, UInt256.Parse("100"), 1, 2));

            _mediatorMock
                .Setup(callTo => callTo.Send(It.IsAny<SelectAddressBalanceByOwnerAndTokenIdQuery>(), cancellationToken))
                .ReturnsAsync(expectedResult);

            // Act
            var response = await _handler.Handle(new RetrieveAddressBalanceByOwnerAndTokenQuery(address, tokenId, findOrThrow: findOrThrow), cancellationToken);

            // Assert
            response.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task SelectAddressBalanceByOwnerAndTokenId_Sends_CallCirrusGetAddressBalanceQuery()
        {
            // Arrange
            Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong tokenId = 2;
            const bool findOrThrow = true;
            var cancellationToken = new CancellationTokenSource().Token;

            _mediatorMock
                .Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), cancellationToken))
                .ReturnsAsync(new Token(tokenId, Address.Cirrus, false, "name", "symbol", 8, 100_000_000, 100, 1, 2));

            // Act
            await _handler.Handle(new RetrieveAddressBalanceByOwnerAndTokenQuery(address, tokenId, findOrThrow: findOrThrow), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusGetAddressCrsBalanceQuery>(q => q.Address == address &&
                                                                                                    q.FindOrThrow == findOrThrow),
                                                       cancellationToken), Times.Once);
        }

        [Fact]
        public async Task SelectAddressBalanceByOwnerAndTokenId_Sends_ReturnsCrsTokenAddressBalance()
        {
            // Arrange
            Address address = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong tokenId = 2;
            const bool findOrThrow = true;
            var cancellationToken = new CancellationTokenSource().Token;

            const ulong expectedBalance = 20;
            var expectedResult = new AddressBalance(tokenId, address, expectedBalance, 1);

            _mediatorMock
                .Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), cancellationToken))
                .ReturnsAsync(new Token(tokenId, Address.Cirrus, false, "name", "symbol", 8, 100_000_000, UInt256.Parse("100"), 1, 2));

            _mediatorMock
                .Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetAddressCrsBalanceQuery>(), cancellationToken))
                .ReturnsAsync(expectedBalance);

            // Act
            var response = await _handler.Handle(new RetrieveAddressBalanceByOwnerAndTokenQuery(address, tokenId, findOrThrow: findOrThrow), cancellationToken);

            // Assert
            response.Should().BeEquivalentTo(expectedResult);
        }
    }
}
