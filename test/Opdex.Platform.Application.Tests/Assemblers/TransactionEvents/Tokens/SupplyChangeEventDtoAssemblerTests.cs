using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers.TransactionEvents.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Assemblers.TransactionEvents.Tokens;

public class SupplyChangeEventDtoAssemblerTests
{
    private readonly SupplyChangeEventDtoAssembler _assembler;
    private readonly Mock<IMediator> _mediatorMock;

    public SupplyChangeEventDtoAssemblerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assembler = new SupplyChangeEventDtoAssembler(_mediatorMock.Object);
    }

    [Fact]
    public async Task Assemble_RetrieveTokenByAddressQuery_SendForContract()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.previousSupply = "0";
        txLog.totalSupply = "100000";

        Address contract = new Address("PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5");
        var log = new SupplyChangeLog(txLog, contract, 5);

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
            It.Is<RetrieveTokenByAddressQuery>(q => q.Address == contract && q.FindOrThrow), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Assemble_MapValues()
    {
        // Arrange
        dynamic txLog = new ExpandoObject();
        txLog.previousSupply = "0";
        txLog.totalSupply = "100000000000000";

        var log = new SupplyChangeLog(txLog, new Address("PM2p2uVqojah5kcXzHiBtV8LVDVGVAgvj5"), 5);

        var token = new Token(1, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", "Copper", "CPR", 11, 100_000_000_000, new UInt256("10000000000000000000"), 9, 10);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(token);

        // Act
        var supplyChangeEvent = await _assembler.Assemble(log);

        // Assert
        supplyChangeEvent.Id.Should().Be(log.Id);
        supplyChangeEvent.TransactionId.Should().Be(log.TransactionId);
        supplyChangeEvent.Contract.Should().Be(log.Contract);
        supplyChangeEvent.SortOrder.Should().Be(log.SortOrder);
        supplyChangeEvent.TotalSupply.Should().Be(log.TotalSupply.ToDecimal(token.Decimals));
    }
}
