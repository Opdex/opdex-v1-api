using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Wrapped;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Tokens;

public class CreateTokenCommandHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly CreateTokenCommandHandler _handler;

    public CreateTokenCommandHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new CreateTokenCommandHandler(_mediator.Object, new NullLogger<CreateTokenCommandHandler>());
    }

    [Fact]
    public void CreateTokenCommand_InvalidTokenAddress_ThrowsArgumentNullException()
    {
        // Arrange
        Address tokenAddress = Address.Empty;
        const ulong blockHeight = 10;
        var attributes = new[] { TokenAttributeType.Provisional };

        // Act
        void Act() => new CreateTokenCommand(tokenAddress, attributes, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Token address must be provided.");
    }

    [Fact]
    public void CreateTokenCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong blockHeight = 0;
        var attributes = new[] { TokenAttributeType.Provisional };

        // Act
        void Act() => new CreateTokenCommand(tokenAddress, attributes, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
    }

    [Fact]
    public async Task CreateTokenCommand_Sends_RetrieveTokenByAddressQuery()
    {
        // Arrange
        Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong blockHeight = 10;
        var attributes = new[] { TokenAttributeType.Provisional };

        // Act
        try
        {
            await _handler.Handle(new CreateTokenCommand(tokenAddress, attributes, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(q => q.Address == tokenAddress),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task CreateTokenCommand_DoesNotAlreadyExist_SendsCallCirrusGetStandardTokenContractSummaryQuery()
    {
        // Arrange
        Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong blockHeight = 10;
        var attributes = new[] { TokenAttributeType.Provisional };

        // Act
        try
        {
            await _handler.Handle(new CreateTokenCommand(tokenAddress, attributes, blockHeight), CancellationToken.None);
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
    public async Task CreateTokenCommand_DoesNotAlreadyExist_SendsCallCirrusGetInterfluxTokenContractSummaryQuery()
    {
        // Arrange
        Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong blockHeight = 10;

        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var summary = new StandardTokenContractSummary(blockHeight);
                summary.SetBaseProperties("ChainLink (InterFlux)", "iLINK", 18);
                summary.SetTotalSupply(UInt256.Parse("1000000000000000000000000000000000000000"));
                return summary;
            });
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((ulong)5);

        // Act
        try
        {
            await _handler.Handle(new CreateTokenCommand(tokenAddress, Array.Empty<TokenAttributeType>(), blockHeight), CancellationToken.None);
        }
        catch
        {
            // ignored
        }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetInterfluxTokenContractSummaryQuery>(
                q => q.BlockHeight == blockHeight &&  q.Token == tokenAddress),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTokenCommand_InterfluxToken_PersistAttribute()
    {
        // Arrange
        Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong blockHeight = 10;

        var interfluxSummary = new InterfluxTokenContractSummary();
        interfluxSummary.SetInterfluxDetails(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), ExternalChainType.Ethereum, "0x514910771af9ca656af840dff83e8264ecf986ca");
        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var summary = new StandardTokenContractSummary(blockHeight);
                summary.SetBaseProperties("ChainLink (InterFlux)", "iLINK", 18);
                summary.SetTotalSupply(UInt256.Parse("1000000000000000000000000000000000000000"));
                return summary;
            });
        ulong tokenId = 5;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenId);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetInterfluxTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(interfluxSummary);

        // Act
        try
        {
            await _handler.Handle(new CreateTokenCommand(tokenAddress, Array.Empty<TokenAttributeType>(), blockHeight), CancellationToken.None);
        }
        catch
        {
            // ignored
        }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<MakeTokenAttributeCommand>(
                c => c.TokenAttribute.TokenId == tokenId
                     && c.TokenAttribute.AttributeType == TokenAttributeType.Interflux),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTokenCommand_InterfluxToken_PersistTokenMapping()
    {
        // Arrange
        Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong blockHeight = 10;

        var interfluxSummary = new InterfluxTokenContractSummary();
        var nativeChain = ExternalChainType.Ethereum;
        var nativeAddress = "0x514910771af9ca656af840dff83e8264ecf986ca";
        interfluxSummary.SetInterfluxDetails(new Address("PBHvTPaLKo5cVYBFdTfTgtjqfybLMJJ8W5"), nativeChain, nativeAddress);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var summary = new StandardTokenContractSummary(blockHeight);
                summary.SetBaseProperties("ChainLink (InterFlux)", "iLINK", 18);
                summary.SetTotalSupply(UInt256.Parse("1000000000000000000000000000000000000000"));
                return summary;
            });
        ulong tokenId = 5;
        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenId);
        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetInterfluxTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(interfluxSummary);

        // Act
        try
        {
            await _handler.Handle(new CreateTokenCommand(tokenAddress, Array.Empty<TokenAttributeType>(), blockHeight), CancellationToken.None);
        }
        catch
        {
            // ignored
        }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<MakeTokenWrappedCommand>(
                c => c.Wrapped.TokenId == tokenId
                     && c.Wrapped.NativeChain == nativeChain
                     && c.Wrapped.NativeAddress == nativeAddress),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTokenCommand_Returns_AlreadyExists()
    {
        // Arrange
        Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong blockHeight = 10;
        const ulong tokenId = 999;
        var attributes = new[] { TokenAttributeType.Provisional };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Token(tokenId, tokenAddress, "Bitcoin", "BTC", 8, 100_000_000, 2_100_000_000_000_000, 9, 10));

        // Act
        var response = await _handler.Handle(new CreateTokenCommand(tokenAddress, attributes, blockHeight), CancellationToken.None);

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
        var attributes = new[] { TokenAttributeType.NonProvisional };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var summary = new StandardTokenContractSummary(blockHeight);
                summary.SetBaseProperties(name, symbol, decimals);
                summary.SetTotalSupply(UInt256.Parse("500000"));
                return summary;
            });

        // Act
        try
        {
            await _handler.Handle(new CreateTokenCommand(tokenAddress, attributes, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<MakeTokenCommand>(q => q.Token.Address == tokenAddress &&
                                                                            q.Token.Name == name &&
                                                                            q.Token.Symbol == symbol &&
                                                                            q.Token.Decimals == decimals &&
                                                                            q.BlockHeight == blockHeight &&
                                                                            q.RefreshTotalSupply == false),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateTokenCommand_Sends_MakeTokenAttributesCommand()
    {
        // Arrange
        Address tokenAddress = "PNG9Xh2WU8q87nq2KGFTtoSPBDE7FiEUa8";
        const ulong blockHeight = 10;
        const string name = "Bitcoin";
        const string symbol = "BTC";
        const int decimals = 8;
        const ulong tokenId = 99;
        var attributes = new[] { TokenAttributeType.NonProvisional };

        _mediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                var summary = new StandardTokenContractSummary(blockHeight);
                summary.SetBaseProperties(name, symbol, decimals);
                summary.SetTotalSupply(UInt256.Parse("500000"));
                return summary;
            });

        _mediator.Setup(callTo => callTo.Send(It.IsAny<MakeTokenCommand>(),It.IsAny<CancellationToken>())).ReturnsAsync(tokenId);

        // Act
        await _handler.Handle(new CreateTokenCommand(tokenAddress, attributes, blockHeight), CancellationToken.None);

        // Assert
        foreach (TokenAttributeType attribute in attributes)
        {
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeTokenAttributeCommand>(q => q.TokenAttribute.TokenId == tokenId &&
                                                                                         q.TokenAttribute.AttributeType == attribute),
                                                   It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
