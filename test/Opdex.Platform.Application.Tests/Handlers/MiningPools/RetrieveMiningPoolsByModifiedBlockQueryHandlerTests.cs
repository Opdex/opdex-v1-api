using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Handlers.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.MiningPools;

public class RetrieveMiningPoolsByModifiedBlockQueryHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly RetrieveMiningPoolsByModifiedBlockQueryHandler _handler;

    public RetrieveMiningPoolsByModifiedBlockQueryHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new RetrieveMiningPoolsByModifiedBlockQueryHandler(_mediator.Object);
    }

    [Fact]
    public void RetrieveMiningPoolsByModifiedBlockQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
    {
        // Arrange
        // Act
        void Act() => new RetrieveMiningPoolsByModifiedBlockQuery(0);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Contains("Block height must be greater than zero.");
    }

    [Fact]
    public async Task RetrieveMiningPoolsByModifiedBlockQuery_Sends_SelectMiningPoolByModifiedBlockQuery()
    {
        // Arrange
        const ulong blockHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new RetrieveMiningPoolsByModifiedBlockQuery(blockHeight), CancellationToken.None);
        } catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<SelectMiningPoolsByModifiedBlockQuery>(q => q.BlockHeight == blockHeight),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }
}