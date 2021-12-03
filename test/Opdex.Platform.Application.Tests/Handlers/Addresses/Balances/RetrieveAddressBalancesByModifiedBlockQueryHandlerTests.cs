using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Handlers.Addresses.Balances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Balances;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Addresses.Balances;

public class RetrieveAddressBalancesByModifiedBlockQueryHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly RetrieveAddressBalancesByModifiedBlockQueryHandler _handler;

    public RetrieveAddressBalancesByModifiedBlockQueryHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new RetrieveAddressBalancesByModifiedBlockQueryHandler(_mediator.Object);
    }

    [Fact]
    public void RetrieveAddressBalancesByModifiedBlockQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        void Act() => new RetrieveAddressBalancesByModifiedBlockQuery(0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
    }

    [Fact]
    public async Task RetrieveAddressBalancesByModifiedBlockQuery_Sends_SelectAddressBalancesByModifiedBlockQuery()
    {
        // Arrange
        const ulong blockHeight = 10;
        var command = new RetrieveAddressBalancesByModifiedBlockQuery(blockHeight);

        // Act
        try
        {
            await _handler.Handle(command, CancellationToken.None);
        } catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<SelectAddressBalancesByModifiedBlockQuery>(q => q.BlockHeight == blockHeight),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }
}