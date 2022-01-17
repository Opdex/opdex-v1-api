using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Transactions;
using Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.LiquidityPools.Quotes;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.LiquidityPools;

public class CreateAddLiquidityTransactionQuoteCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>> _assemblerMock;
    private readonly CreateAddLiquidityTransactionQuoteCommandHandler _handler;
    private readonly OpdexConfiguration _config;
    const string MethodName = RouterConstants.Methods.AddLiquidity;

    public CreateAddLiquidityTransactionQuoteCommandHandlerTests()
    {
        _config = new OpdexConfiguration { ApiUrl = "https://dev-api.opdex.com", WalletTransactionCallback = "/transactions" };
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<TransactionQuote, TransactionQuoteDto>>();
        _handler = new CreateAddLiquidityTransactionQuoteCommandHandler(_assemblerMock.Object, _mediatorMock.Object, _config);
    }

    [Fact]
    public void CreateAddLiquidityTransactionQuoteCommand_InvalidLiquidityPool_ThrowArgumentNullException()
    {
        // Arrange
        Address liquidityPool = Address.Empty;
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        FixedDecimal amountCrs = FixedDecimal.Parse("1.00");
        FixedDecimal amountSrc = FixedDecimal.Parse("2.00");
        FixedDecimal amountCrsMin = FixedDecimal.Parse("0.9");
        FixedDecimal amountSrcMin = FixedDecimal.Parse("1.8");

        // Act
        void Act() => new CreateAddLiquidityTransactionQuoteCommand(liquidityPool, walletAddress, amountCrs, amountSrc, amountCrsMin,
                                                                    amountSrcMin, walletAddress, 0ul);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Liquidity pool must be provided.");
    }

    [Fact]
    public void CreateAddLiquidityTransactionQuoteCommand_InvalidRecipient_ThrowArgumentNullException()
    {
        // Arrange
        Address recipient = Address.Empty;
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        FixedDecimal amountCrs = FixedDecimal.Parse("1.00");
        FixedDecimal amountSrc = FixedDecimal.Parse("2.00");
        FixedDecimal amountCrsMin = FixedDecimal.Parse("0.9");
        FixedDecimal amountSrcMin = FixedDecimal.Parse("1.8");

        // Act
        void Act() => new CreateAddLiquidityTransactionQuoteCommand(liquidityPool, walletAddress, amountCrs, amountSrc, amountCrsMin,
                                                                    amountSrcMin, recipient, 0ul);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Recipient must be provided.");
    }

    [Theory]
    [InlineData("-1.01")]
    [InlineData("0")]
    public void CreateAddLiquidityTransactionQuoteCommand_InvalidAmountCrs_ThrowArgumentOutOfRangeException(string amountCrs)
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        var amountSrc = FixedDecimal.Parse("2.00");
        var amountCrsMin = FixedDecimal.Parse("0.9");
        var amountSrcMin = FixedDecimal.Parse("1.8");

        // Act
        void Act() => new CreateAddLiquidityTransactionQuoteCommand(liquidityPool, walletAddress, FixedDecimal.Parse(amountCrs), amountSrc, amountCrsMin,
                                                                    amountSrcMin, walletAddress, 0ul);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Amount CRS must be greater than 0.");
    }

    [Theory]
    [InlineData("-1.01")]
    [InlineData("0")]
    public void CreateAddLiquidityTransactionQuoteCommand_InvalidAmountSrc_ThrowArgumentOutOfRangeException(string amountSrc)
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        var amountCrs = FixedDecimal.Parse("1.00");
        var amountCrsMin = FixedDecimal.Parse("0.9");
        var amountSrcMin = FixedDecimal.Parse("1.8");

        // Act
        void Act() => new CreateAddLiquidityTransactionQuoteCommand(liquidityPool, walletAddress, amountCrs, FixedDecimal.Parse(amountSrc), amountCrsMin,
                                                                    amountSrcMin, walletAddress, 0ul);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Amount SRC must be greater than 0.");
    }

    [Theory]
    [InlineData("-1.01")]
    [InlineData("0")]
    public void CreateAddLiquidityTransactionQuoteCommand_InvalidAmountCrsMin_ThrowArgumentOutOfRangeException(string amountCrsMin)
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        var amountCrs = FixedDecimal.Parse("1.00");
        var amountSrc = FixedDecimal.Parse("2.00");
        var amountSrcMin = FixedDecimal.Parse("1.8");

        // Act
        void Act() => new CreateAddLiquidityTransactionQuoteCommand(liquidityPool, walletAddress, amountCrs, amountSrc, FixedDecimal.Parse(amountCrsMin),
                                                                    amountSrcMin, walletAddress, 0ul);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Amount CRS minimum must be greater than 0.");
    }

    [Theory]
    [InlineData("-1.01")]
    [InlineData("0")]
    public void CreateAddLiquidityTransactionQuoteCommand_InvalidAmountSrcMin_ThrowArgumentOutOfRangeException(string amountSrcMin)
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        var amountCrs = FixedDecimal.Parse("1.00");
        var amountSrc = FixedDecimal.Parse("2.00");
        var amountCrsMin = FixedDecimal.Parse("0.9");

        // Act
        void Act() => new CreateAddLiquidityTransactionQuoteCommand(liquidityPool, walletAddress, amountCrs, amountSrc, amountCrsMin,
                                                                    FixedDecimal.Parse(amountSrcMin), walletAddress, 0ul);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Amount SRC minimum must be greater than 0.");
    }

    [Fact]
    public async Task CreateAddLiquidityTransactionQuoteCommand_Sends_MakeTransactionQuoteCommand()
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        FixedDecimal amountCrs = FixedDecimal.Parse("1.00");
        FixedDecimal amountSrc = FixedDecimal.Parse("2.00");
        FixedDecimal amountCrsMin = FixedDecimal.Parse("0.9");
        FixedDecimal amountSrcMin = FixedDecimal.Parse("1.8");
        const ulong deadline = 1ul;

        var pool = new LiquidityPool(1, liquidityPool, "BTC-CRS", 2, 3, 4, 6, 7);
        var token = new Token(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "Bitcoin", "BTC", 8, 100_000_000, 10000000, 2, 3);
        var marketRouter = new MarketRouter(1, "PMsinMXrr2uNEL5AQD1LpiYTRFiRTA8uZU", 2, true, 3, 4);
        var cancellationToken = new CancellationTokenSource().Token;

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), cancellationToken))
            .ReturnsAsync(pool);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), cancellationToken))
            .ReturnsAsync(token);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveMarketRouterByMarketIdQuery>(), cancellationToken))
            .ReturnsAsync(marketRouter);

        var command = new CreateAddLiquidityTransactionQuoteCommand(liquidityPool, walletAddress, amountCrs, amountSrc, amountCrsMin,
                                                                    amountSrcMin, walletAddress, deadline);

        var expectedParameters = new List<TransactionQuoteRequestParameter>
        {
            new TransactionQuoteRequestParameter("Token", token.Address),
            new TransactionQuoteRequestParameter("SRC Amount", UInt256.Parse("200000000")),
            new TransactionQuoteRequestParameter("Minimum CRS Amount", 90000000ul),
            new TransactionQuoteRequestParameter("Minimum SRC Amount", UInt256.Parse("180000000")),
            new TransactionQuoteRequestParameter("Recipient", walletAddress),
            new TransactionQuoteRequestParameter("Deadline", deadline)
        };

        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<MakeTransactionQuoteCommand>(c => c.QuoteRequest.Sender == walletAddress
                                                                                           && c.QuoteRequest.To == marketRouter.Address
                                                                                           && c.QuoteRequest.Amount == amountCrs
                                                                                           && c.QuoteRequest.Method == MethodName
                                                                                           && c.QuoteRequest.Callback != null
                                                                                           && c.QuoteRequest.Parameters
                                                                                               .All(p => expectedParameters
                                                                                                        .Select(e => e.Value)
                                                                                                        .Contains(p.Value))),
                                                   It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateAddLiquidityTransactionQuoteCommand_Assembles_TransactionQuoteDto()
    {
        // Arrange
        Address walletAddress = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address liquidityPool = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        FixedDecimal amountCrs = FixedDecimal.Parse("1.00");
        FixedDecimal amountSrc = FixedDecimal.Parse("2.00");
        FixedDecimal amountCrsMin = FixedDecimal.Parse("0.9");
        FixedDecimal amountSrcMin = FixedDecimal.Parse("1.8");
        const ulong deadline = 5400;

        var pool = new LiquidityPool(1, liquidityPool, "BTC-CRS", 2, 3, 4, 6, 7);
        var token = new Token(1, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", "Bitcoin", "BTC", 8, 100_000_000, 10000000, 2, 3);
        var marketRouter = new MarketRouter(1, "PMsinMXrr2uNEL5AQD1LpiYTRFiRTA8uZU", 2, true, 3, 4);
        var cancellationToken = new CancellationTokenSource().Token;

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), cancellationToken))
            .ReturnsAsync(pool);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), cancellationToken))
            .ReturnsAsync(token);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveActiveMarketRouterByMarketIdQuery>(), cancellationToken))
            .ReturnsAsync(marketRouter);

        var command = new CreateAddLiquidityTransactionQuoteCommand(liquidityPool, walletAddress, amountCrs, amountSrc, amountCrsMin,
                                                                    amountSrcMin, walletAddress, deadline);

        var expectedParameters = new List<TransactionQuoteRequestParameter>
        {
            new TransactionQuoteRequestParameter("Token", token.Address),
            new TransactionQuoteRequestParameter("SRC Amount", UInt256.Parse("200000000")),
            new TransactionQuoteRequestParameter("Minimum CRS Amount", 90000000ul),
            new TransactionQuoteRequestParameter("Minimum SRC Amount", UInt256.Parse("180000000")),
            new TransactionQuoteRequestParameter("Recipient", walletAddress),
            new TransactionQuoteRequestParameter("Deadline", deadline)
        };

        var expectedRequest = new TransactionQuoteRequest(walletAddress, marketRouter.Address, amountCrs, MethodName, _config.WalletTransactionCallback, expectedParameters);

        var expectedQuote = new TransactionQuote("1000", null, 23800, null, expectedRequest);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<MakeTransactionQuoteCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedQuote);

        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch { }

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(expectedQuote), Times.Once);
    }
}
