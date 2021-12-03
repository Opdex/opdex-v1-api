using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Handlers.Tokens;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Tokens;

public class MakeTokenCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly MakeTokenCommandHandler _handler;

    public MakeTokenCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new MakeTokenCommandHandler(_mediator.Object);
    }

    [Fact]
    public void MakeTokenCommand_InvalidToken_ThrowsArgumentNullException()
    {
        // Arrange
        const ulong blockHeight = 10;

        // Act
        void Act() => new MakeTokenCommand(null, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Token must be provided.");
    }

    [Fact]
    public void MakeTokenCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        var token = new Token(1, "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10);
        const ulong blockHeight = 0;

        // Act
        void Act() => new MakeTokenCommand(token, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
    }

    [Fact]
    public async Task MakeTokenCommand_Sends_CallCirrusGetStandardTokenContractSummaryQuery()
    {
        // Arrange
        var token = new Token(1, "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10);
        const ulong blockHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new MakeTokenCommand(token, blockHeight, refreshTotalSupply: true), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetStandardTokenContractSummaryQuery>(q => q.Token == token.Address &&
                                                                                                          q.IncludeBaseProperties == false &&
                                                                                                          q.IncludeTotalSupply == true &&
                                                                                                          q.BlockHeight == blockHeight),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task MakeTokenCommand_Skips_CallCirrusGetStandardTokenContractSummaryQuery()
    {
        // Arrange
        var token = new Token(1, "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10);
        const ulong blockHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new MakeTokenCommand(token, blockHeight, refreshTotalSupply: false), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task MakeTokenCommand_Sends_PersistTokenCommand()
    {
        // Arrange
        var token = new Token(1, "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8", true, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10);
        const ulong blockHeight = 10;

        UInt256 updatedTotalSupply = 5000000;

        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var summary = new StandardTokenContractSummary(blockHeight);
                summary.SetTotalSupply(updatedTotalSupply);
                return summary;
            });

        // Act
        await _handler.Handle(new MakeTokenCommand(token, blockHeight, refreshTotalSupply: true), CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<PersistTokenCommand>(q => q.Token.Id == token.Id &&
                                                                               q.Token.Address == token.Address &&
                                                                               q.Token.TotalSupply == updatedTotalSupply),
                                               CancellationToken.None), Times.Once);
    }
}