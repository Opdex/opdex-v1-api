using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers.TransactionEvents.LiquidityPools;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.LiquidityPools;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Assemblers.TransactionEvents.LiquidityPools;

public class ReservesChangeEventDtoAssemblerTests
{
    private readonly ReservesChangeEventDtoAssembler _assembler;
    private readonly Mock<IMediator> _mediatorMock;

    public ReservesChangeEventDtoAssemblerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assembler = new ReservesChangeEventDtoAssembler(_mediatorMock.Object);
    }

    [Fact]
    public async Task Assemble_RetrieveLiquidityPoolByAddressQuery_SendForContract()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.reserveCrs = (ulong)100000000000;
        txLog.reserveSrc = "50000000000000000000";

        Address contract = new("PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5");
        var log = new ReservesLog(txLog, contract, 5);

        // Act
        try
        {
            await _assembler.Assemble(log);
        }
        catch (Exception)
        {
            // ignored
        }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
            It.Is<RetrieveLiquidityPoolByAddressQuery>(q => q.Address == contract && q.FindOrThrow), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Assemble_RetrieveTokenByIdQuery_SendForSrcTokenId()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.reserveCrs = (ulong)100000000000;
        txLog.reserveSrc = "50000000000000000000";

        Address contract = new("PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5");
        var log = new ReservesLog(txLog, contract, 5);

        var liquidityPool = new LiquidityPool(10, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", "BTC-CRS", 5, 15, 25, 500, 505);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(liquidityPool);

        // Act
        try
        {
            await _assembler.Assemble(log);
        }
        catch (Exception)
        {
            // ignored
        }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
            It.Is<RetrieveTokenByIdQuery>(q => q.TokenId == liquidityPool.SrcTokenId && q.FindOrThrow), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Assemble_MapValues()
    {
        // Arrange
        ulong crs = 100000000000;
        UInt256 src = UInt256.Parse("50000000000000000000");

        dynamic txLog = new ExpandoObject();
        txLog.reserveCrs = crs;
        txLog.reserveSrc = src.ToString();

        var log = new ReservesLog(txLog, new Address("PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5"), 5);

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLiquidityPoolByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LiquidityPool(10, "PX2J4s4UHLfwZbDRJSvPoskKD25xQBHWYi", "wBTC-CRS", 5, 15, 25, 500, 505));
        var tokenSrc = new Token(1, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", "Bitcoin (Wrapped)", "wBTC", 8, 100_000_000_000, new UInt256("10000000000000000000"), 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(tokenSrc);

        // Act
        var supplyChangeEvent = await _assembler.Assemble(log);

        // Assert
        supplyChangeEvent.Id.Should().Be(log.Id);
        supplyChangeEvent.TransactionId.Should().Be(log.TransactionId);
        supplyChangeEvent.Contract.Should().Be(log.Contract);
        supplyChangeEvent.SortOrder.Should().Be(log.SortOrder);
        supplyChangeEvent.Crs.Should().Be(log.ReserveCrs.ToDecimal(TokenConstants.Cirrus.Decimals));
        supplyChangeEvent.Src.Should().Be(log.ReserveSrc.ToDecimal(tokenSrc.Decimals));
    }
}
