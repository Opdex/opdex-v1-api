using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Tokens
{
    public class CreateTokenCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly CreateTokenCommandHandler _handler;

        public CreateTokenCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _handler = new CreateTokenCommandHandler(_mediator.Object);
        }

        [Fact]
        public void CreateTokenCommand_InvalidTokenAddress_ThrowsArgumentNullException()
        {
            // Arrange
            Address tokenAddress = Address.Empty;
            const ulong blockHeight = 10;

            // Act
            void Act() => new CreateTokenCommand(tokenAddress, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Contains("Token address must be provided.");
        }

        [Fact]
        public void CreateTokenCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong blockHeight = 0;

            // Act
            void Act() => new CreateTokenCommand(tokenAddress, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CreateTokenCommand_Sends_RetrieveTokenByAddressQuery()
        {
            // Arrange
            Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateTokenCommand(tokenAddress, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(q => q.Address == tokenAddress),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public async Task CreateTokenCommand_Sends_CallCirrusGetStandardTokenContractSummaryQuery()
        {
            // Arrange
            Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateTokenCommand(tokenAddress, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetStandardTokenContractSummaryQuery>(q => q.BlockHeight == blockHeight &&
                                                                                                              q.Token == tokenAddress &&
                                                                                                              q.IncludeBaseProperties == true &&
                                                                                                              q.IncludeTotalSupply == true),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CreateTokenCommand_Returns_AlreadyExists()
        {
            // Arrange
            Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong blockHeight = 10;
            const  ulong tokenId = 999;

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Token(tokenId, tokenAddress, true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10));

            // Act
            var response = await _handler.Handle(new CreateTokenCommand(tokenAddress, blockHeight), CancellationToken.None);

            // Assert
            response.Should().Be(tokenId);
            _mediator.Verify(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CreateTokenCommand_Sends_MakeTokenCommand()
        {
            // Arrange
            Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
            const ulong blockHeight = 10;

            const string name = "Bitcoin";
            const string symbol = "BTC";
            const int decimals = 8;

            _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    var summary = new StandardTokenContractSummary(blockHeight);
                    summary.SetBaseProperties(name, symbol, decimals);
                    summary.SetTotalSupply(UInt256.Parse("500000"));
                    return summary;
                });

            // Act
            await _handler.Handle(new CreateTokenCommand(tokenAddress, blockHeight), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeTokenCommand>(q => q.Token.Address == tokenAddress &&
                                                                                q.Token.Name == name &&
                                                                                q.Token.Symbol == symbol &&
                                                                                q.Token.Decimals == decimals &&
                                                                                q.BlockHeight == blockHeight &&
                                                                                q.RefreshTotalSupply == false),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
