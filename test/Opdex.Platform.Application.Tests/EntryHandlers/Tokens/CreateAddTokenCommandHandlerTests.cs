using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.Commands.Tokens.Wrapped;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Application.Exceptions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Tokens;

public class CreateAddTokenCommandHandlerTests
{
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IMediator> _mockMediator;
    private readonly CreateAddTokenCommandHandler _handler;
    private readonly Address _multiSigContractAddress;

    public CreateAddTokenCommandHandlerTests()
    {
        _mockMapper = new Mock<IMapper>();
        _mockMediator = new Mock<IMediator>();
        _multiSigContractAddress = new Address("PVBs1gH81rtEDzzz852jBqdm55jf9h60P8xC");
        var interfluxConfig = new InterfluxConfiguration { MultiSigContractAddress = _multiSigContractAddress };
        _handler = new CreateAddTokenCommandHandler(_mockMapper.Object, _mockMediator.Object, interfluxConfig, new NullLogger<CreateAddTokenCommandHandler>());
    }

    [Fact]
    public async Task Handle_TokenAlreadyIndexed_ThrowTokenAlreadyIndexedException()
    {
        // Arrange
        Address tokenAddress = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
        var token = new Token(1, tokenAddress, "Bitcoin", "BTC", 8, 100_000_000, 10000000, 2, 3);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

        // Act
        Task Act() => _handler.Handle(new CreateAddTokenCommand(tokenAddress), CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<TokenAlreadyIndexedException>(Act);
        exception.Token.Should().Be(tokenAddress);
    }

    [Fact]
    public async Task Handle_GetBestBlockReceiptQuery_Send()
    {
        // Arrange
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Token)null);

        // Act
        try
        {
            await _handler.Handle(new CreateAddTokenCommand("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"), CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mockMediator.Verify(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CallCirrusGetStandardTokenContractSummaryQuery_Send()
    {
        // Arrange
        Address tokenAddress = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
        var blockReceipt = new BlockReceipt(Sha256.Parse("59218de9ed9bc1df4400fdf4b968ec6ca42baccbdb7f25c923345ba84d5eb5b3"), 5000, DateTime.Now, DateTime.Now,
                                            Sha256.Parse("bec4d5e4f8d01f8741ccd268504d8b9a1086273ad2fca90eed55096da9bc910b"),
                                            Sha256.Parse("4d35ea70ce77c42fad7852045694c3c0ec30a7aa069e9cec769f344f14d3d9f9"),
                                            Sha256.Parse("ad0bea2255c37ec43daa5446f2599b9f23bd36ec79ab52c2323470d0e08b718d"), Enumerable.Empty<Sha256>());
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Token)null);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);

        // Act
        try
        {
            await _handler.Handle(new CreateAddTokenCommand(tokenAddress), CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mockMediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetStandardTokenContractSummaryQuery>(query => query.Token == tokenAddress
                                                                                                                  && query.BlockHeight == blockReceipt.Height
                                                                                                                  && query.IncludeBaseProperties
                                                                                                                  && query.IncludeTotalSupply), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CallCirrusGetInterfluxTokenContractSummaryQuery_Send()
    {
        // Arrange
        Address tokenAddress = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
        var blockReceipt = new BlockReceipt(Sha256.Parse("59218de9ed9bc1df4400fdf4b968ec6ca42baccbdb7f25c923345ba84d5eb5b3"), 5000, DateTime.Now, DateTime.Now,
            Sha256.Parse("bec4d5e4f8d01f8741ccd268504d8b9a1086273ad2fca90eed55096da9bc910b"),
            Sha256.Parse("4d35ea70ce77c42fad7852045694c3c0ec30a7aa069e9cec769f344f14d3d9f9"),
            Sha256.Parse("ad0bea2255c37ec43daa5446f2599b9f23bd36ec79ab52c2323470d0e08b718d"), Enumerable.Empty<Sha256>());
        var tokenSummary = new StandardTokenContractSummary(blockReceipt.Height);
        tokenSummary.SetBaseProperties("Bitcoin (Wrapped)", "xBTC", 8);
        tokenSummary.SetTotalSupply(2100000000000000);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Token)null);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenSummary);

        // Act
        try
        {
            await _handler.Handle(new CreateAddTokenCommand(tokenAddress), CancellationToken.None);
        }
        catch (Exception)
        {
            // ignored
        }

        // Assert
        _mockMediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetInterfluxTokenContractSummaryQuery>(query =>
            query.Token == tokenAddress
            && query.BlockHeight == blockReceipt.Height), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_NonProvisional_AddAttribute()
    {
        // Arrange
        Address tokenAddress = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
        var blockReceipt = new BlockReceipt(Sha256.Parse("59218de9ed9bc1df4400fdf4b968ec6ca42baccbdb7f25c923345ba84d5eb5b3"), 5000, DateTime.Now, DateTime.Now,
            Sha256.Parse("bec4d5e4f8d01f8741ccd268504d8b9a1086273ad2fca90eed55096da9bc910b"),
            Sha256.Parse("4d35ea70ce77c42fad7852045694c3c0ec30a7aa069e9cec769f344f14d3d9f9"),
            Sha256.Parse("ad0bea2255c37ec43daa5446f2599b9f23bd36ec79ab52c2323470d0e08b718d"), Enumerable.Empty<Sha256>());
        var tokenSummary = new StandardTokenContractSummary(blockReceipt.Height);
        tokenSummary.SetBaseProperties("Native", "NTV", 8);
        tokenSummary.SetTotalSupply(UInt256.Parse("100000000000000000"));        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Token)null);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenSummary);
        ulong tokenId = 5;
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<MakeTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenId);

        // Act
        try
        {
            await _handler.Handle(new CreateAddTokenCommand(tokenAddress), CancellationToken.None);
        }
        catch (Exception)
        {
            // ignored
        }

        // Assert
        _mockMediator.Verify(callTo => callTo.Send(It.Is<MakeTokenAttributeCommand>(
            c => c.TokenAttribute.TokenId == tokenId
                 && c.TokenAttribute.AttributeType == TokenAttributeType.NonProvisional),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ForInterfluxToken_AddAttribute()
    {
        // Arrange
        Address tokenAddress = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
        var blockReceipt = new BlockReceipt(Sha256.Parse("59218de9ed9bc1df4400fdf4b968ec6ca42baccbdb7f25c923345ba84d5eb5b3"), 5000, DateTime.Now, DateTime.Now,
            Sha256.Parse("bec4d5e4f8d01f8741ccd268504d8b9a1086273ad2fca90eed55096da9bc910b"),
            Sha256.Parse("4d35ea70ce77c42fad7852045694c3c0ec30a7aa069e9cec769f344f14d3d9f9"),
            Sha256.Parse("ad0bea2255c37ec43daa5446f2599b9f23bd36ec79ab52c2323470d0e08b718d"), Enumerable.Empty<Sha256>());
        var tokenSummary = new StandardTokenContractSummary(blockReceipt.Height);
        tokenSummary.SetBaseProperties("ChainLink (InterFlux)", "iLINK", 18);
        tokenSummary.SetTotalSupply(UInt256.Parse("1000000000000000000000000000"));
        var interfluxSummary = new InterfluxTokenContractSummary(blockReceipt.Height);
        interfluxSummary.SetInterfluxDetails(_multiSigContractAddress, ExternalChainType.Ethereum, "0x514910771af9ca656af840dff83e8264ecf986ca");
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Token)null);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenSummary);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetInterfluxTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(interfluxSummary);
        ulong tokenId = 5;
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<MakeTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenId);

        // Act
        try
        {
            await _handler.Handle(new CreateAddTokenCommand(tokenAddress), CancellationToken.None);
        }
        catch (Exception)
        {
            // ignored
        }

        // Assert
        _mockMediator.Verify(callTo => callTo.Send(It.Is<MakeTokenAttributeCommand>(
            c => c.TokenAttribute.TokenId == tokenId
                 && c.TokenAttribute.AttributeType == TokenAttributeType.Interflux),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_ForInterfluxTokenNotOwnedByMultiSig_DoNotPersistTokenMapping()
    {
        // Arrange
        Address tokenAddress = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
        var blockReceipt = new BlockReceipt(Sha256.Parse("59218de9ed9bc1df4400fdf4b968ec6ca42baccbdb7f25c923345ba84d5eb5b3"), 5000, DateTime.Now, DateTime.Now,
            Sha256.Parse("bec4d5e4f8d01f8741ccd268504d8b9a1086273ad2fca90eed55096da9bc910b"),
            Sha256.Parse("4d35ea70ce77c42fad7852045694c3c0ec30a7aa069e9cec769f344f14d3d9f9"),
            Sha256.Parse("ad0bea2255c37ec43daa5446f2599b9f23bd36ec79ab52c2323470d0e08b718d"), Enumerable.Empty<Sha256>());
        var tokenSummary = new StandardTokenContractSummary(blockReceipt.Height);
        tokenSummary.SetBaseProperties("ChainLink (InterFlux)", "iLINK", 18);
        tokenSummary.SetTotalSupply(UInt256.Parse("1000000000000000000000000000"));
        var interfluxSummary = new InterfluxTokenContractSummary(blockReceipt.Height);
        var nativeChain = ExternalChainType.Ethereum;
        var nativeAddress = "0x514910771af9ca656af840dff83e8264ecf986ca";
        interfluxSummary.SetInterfluxDetails(new Address("NOT_THE_INTERFLUX_MULTISIG_ADDRESS"), nativeChain, nativeAddress);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Token)null);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenSummary);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetInterfluxTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(interfluxSummary);
        ulong tokenId = 5;
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<MakeTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenId);

        // Act
        try
        {
            await _handler.Handle(new CreateAddTokenCommand(tokenAddress), CancellationToken.None);
        }
        catch (Exception)
        {
            // ignored
        }

        // Assert
        _mockMediator.Verify(callTo => callTo.Send(It.IsAny<MakeTokenChainCommand>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ForInterfluxToken_PersistTokenMapping()
    {
        // Arrange
        Address tokenAddress = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
        var blockReceipt = new BlockReceipt(Sha256.Parse("59218de9ed9bc1df4400fdf4b968ec6ca42baccbdb7f25c923345ba84d5eb5b3"), 5000, DateTime.Now, DateTime.Now,
            Sha256.Parse("bec4d5e4f8d01f8741ccd268504d8b9a1086273ad2fca90eed55096da9bc910b"),
            Sha256.Parse("4d35ea70ce77c42fad7852045694c3c0ec30a7aa069e9cec769f344f14d3d9f9"),
            Sha256.Parse("ad0bea2255c37ec43daa5446f2599b9f23bd36ec79ab52c2323470d0e08b718d"), Enumerable.Empty<Sha256>());
        var tokenSummary = new StandardTokenContractSummary(blockReceipt.Height);
        tokenSummary.SetBaseProperties("ChainLink (InterFlux)", "iLINK", 18);
        tokenSummary.SetTotalSupply(UInt256.Parse("1000000000000000000000000000"));
        var interfluxSummary = new InterfluxTokenContractSummary(blockReceipt.Height);
        var nativeChain = ExternalChainType.Ethereum;
        var nativeAddress = "0x514910771af9ca656af840dff83e8264ecf986ca";
        interfluxSummary.SetInterfluxDetails(_multiSigContractAddress, nativeChain, nativeAddress);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Token)null);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenSummary);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetInterfluxTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(interfluxSummary);
        ulong tokenId = 5;
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<MakeTokenCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenId);

        // Act
        try
        {
            await _handler.Handle(new CreateAddTokenCommand(tokenAddress), CancellationToken.None);
        }
        catch (Exception)
        {
            // ignored
        }

        // Assert
        _mockMediator.Verify(callTo => callTo.Send(It.Is<MakeTokenChainCommand>(
            c => c.Chain.TokenId == tokenId
                 && c.Chain.NativeChain == nativeChain
                 && c.Chain.NativeAddress == nativeAddress),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_CallCirrusGetStandardTokenContractSummaryQueryFails_ThrowInvalidDataException()
    {
        // Arrange
        var blockReceipt = new BlockReceipt(Sha256.Parse("59218de9ed9bc1df4400fdf4b968ec6ca42baccbdb7f25c923345ba84d5eb5b3"), 5000, DateTime.Now, DateTime.Now,
                                            Sha256.Parse("bec4d5e4f8d01f8741ccd268504d8b9a1086273ad2fca90eed55096da9bc910b"),
                                            Sha256.Parse("4d35ea70ce77c42fad7852045694c3c0ec30a7aa069e9cec769f344f14d3d9f9"),
                                            Sha256.Parse("ad0bea2255c37ec43daa5446f2599b9f23bd36ec79ab52c2323470d0e08b718d"), Enumerable.Empty<Sha256>());
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Token)null);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Call to Cirrus failed"));

        // Act
        Task Act() => _handler.Handle(new CreateAddTokenCommand("PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV"), CancellationToken.None);

        // Assert
        var exception = await Assert.ThrowsAsync<InvalidDataException>(Act);
        exception.Message.Should().Be("Unable to validate SRC token.");
    }

    [Fact]
    public async Task Handle_MakeTokenCommand_Send()
    {
        // Arrange
        Address tokenAddress = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
        var blockReceipt = new BlockReceipt(Sha256.Parse("59218de9ed9bc1df4400fdf4b968ec6ca42baccbdb7f25c923345ba84d5eb5b3"), 5000, DateTime.Now, DateTime.Now,
                                            Sha256.Parse("bec4d5e4f8d01f8741ccd268504d8b9a1086273ad2fca90eed55096da9bc910b"),
                                            Sha256.Parse("4d35ea70ce77c42fad7852045694c3c0ec30a7aa069e9cec769f344f14d3d9f9"),
                                            Sha256.Parse("ad0bea2255c37ec43daa5446f2599b9f23bd36ec79ab52c2323470d0e08b718d"), Enumerable.Empty<Sha256>());
        var tokenSummary = new StandardTokenContractSummary(blockReceipt.Height);
        tokenSummary.SetBaseProperties("Bitcoin (Wrapped)", "xBTC", 8);
        tokenSummary.SetTotalSupply(2100000000000000);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Token)null);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenSummary);

        // Act
        try
        {
            await _handler.Handle(new CreateAddTokenCommand(tokenAddress), CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mockMediator.Verify(callTo => callTo.Send(It.Is<MakeTokenCommand>(query => query.Token.Address == tokenAddress && query.BlockHeight == blockReceipt.Height),
                                                   It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Token_MapResponse()
    {
        // Arrange
        Address tokenAddress = "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV";
        var blockReceipt = new BlockReceipt(Sha256.Parse("59218de9ed9bc1df4400fdf4b968ec6ca42baccbdb7f25c923345ba84d5eb5b3"), 5000, DateTime.Now, DateTime.Now,
                                            Sha256.Parse("bec4d5e4f8d01f8741ccd268504d8b9a1086273ad2fca90eed55096da9bc910b"),
                                            Sha256.Parse("4d35ea70ce77c42fad7852045694c3c0ec30a7aa069e9cec769f344f14d3d9f9"),
                                            Sha256.Parse("ad0bea2255c37ec43daa5446f2599b9f23bd36ec79ab52c2323470d0e08b718d"), Enumerable.Empty<Sha256>());
        var tokenSummary = new StandardTokenContractSummary(blockReceipt.Height);
        tokenSummary.SetBaseProperties("Bitcoin (Wrapped)", "xBTC", 8);
        tokenSummary.SetTotalSupply(2100000000000000);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync((Token)null);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<GetBestBlockReceiptQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockReceipt);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetStandardTokenContractSummaryQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenSummary);
        _mockMediator.Setup(callTo => callTo.Send(It.IsAny<MakeTokenCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(1UL);

        // Act
        try
        {
            await _handler.Handle(new CreateAddTokenCommand(tokenAddress), CancellationToken.None);
        }
        catch (Exception) { }

        // Assert
        _mockMapper.Verify(callTo => callTo.Map<TokenDto>(It.Is<Token>(token => token.Address == tokenAddress)));
    }
}
