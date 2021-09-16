using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Addresses;
using Opdex.Platform.Application.Handlers.Addresses.Balances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Addresses.Balances
{
    public class MakeAddressBalanceCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly MakeAddressBalanceCommandHandler _handler;

        public MakeAddressBalanceCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new MakeAddressBalanceCommandHandler(_mediator.Object);
        }

        [Fact]
        public void MakeAddressBalanceCommand_InvalidAddressBalance_ThrowsArgumentNullException()
        {
            // Arrange
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong blockHeight = 10;

            // Act
            void Act() => new MakeAddressBalanceCommand(null, token, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Address balance must be provided.");
        }

        [Fact]
        public void MakeAddressBalanceCommand_InvalidToken_ThrowsArgumentNullException()
        {
            // Arrange
            var balance = new AddressBalance(1, 2, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 10, 3, 4);
            const ulong blockHeight = 10;

            // Act
            void Act() => new MakeAddressBalanceCommand(balance, null, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Token address must be provided.");
        }

        [Fact]
        public void MakeAddressBalanceCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var balance = new AddressBalance(1, 2, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 10, 3, 4);
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";

            // Act
            void Act() => new MakeAddressBalanceCommand(balance, token, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task MakeAddressBalanceCommand_Sends_CallCirrusGetSrcTokenBalanceQuery()
        {
            // Arrange
            var balance = new AddressBalance(1, 2, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 10, 3, 4);
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new MakeAddressBalanceCommand(balance, token, blockHeight), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetSrcTokenBalanceQuery>(q => q.Token == token &&
                                                                                                 q.Owner == balance.Owner &&
                                                                                                 q.BlockHeight == blockHeight),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task MakeAddressBalanceCommand_Sends_PersistAddressBalanceCommand()
        {
            // Arrange
            var balance = new AddressBalance(1, 2, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", 10, 3, 4);
            Address token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk";
            const ulong blockHeight = 10;
            UInt256 expectedBalance = 100;

            _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetSrcTokenBalanceQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedBalance);

            // Act
            try
            {
                await _handler.Handle(new MakeAddressBalanceCommand(balance, token, blockHeight), CancellationToken.None);
            } catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<PersistAddressBalanceCommand>(q => q.AddressBalance.Balance == expectedBalance &&
                                                                                            q.AddressBalance.Owner == balance.Owner),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
