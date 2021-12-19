using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Transactions;
using Opdex.Platform.Application.Handlers.Transactions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Mempool;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Transactions;

public class RetrieveCirrusExistsInMempoolQueryHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly RetrieveCirrusExistsInMempoolQueryHandler _handler;

    public RetrieveCirrusExistsInMempoolQueryHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _handler = new RetrieveCirrusExistsInMempoolQueryHandler(_mediatorMock.Object);
    }

    [Fact]
    public async Task Handle_CallCirrusGetExistsInMempoolQuery_Send()
    {
        // Arrange
        var transactionHash = new Sha256(43594389025);
        var request = new RetrieveCirrusExistsInMempoolQuery(transactionHash);
        var cancellationToken = new CancellationTokenSource().Token;
        const int maxRetries = 3;

        // Suppress additional attempts after the first + retry to speed up tests
        int attempt = 0;
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetExistsInMempoolQuery>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(() => ++attempt > 1);

        // Act
        await _handler.Handle(request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CallCirrusGetExistsInMempoolQuery>(query => query.TransactionHash == transactionHash), cancellationToken),
            Times.AtMost(maxRetries));
    }

    [Fact]
    public async Task Handle_TransactionNotFoundInMempool_ReturnFalse()
    {
        // Arrange
        var request = new RetrieveCirrusExistsInMempoolQuery(new Sha256(43594389025));

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetExistsInMempoolQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(false);
    }

    [Fact]
    public async Task Handle_TransactionFoundInMempool_ReturnTrue()
    {
        // Arrange
        var request = new RetrieveCirrusExistsInMempoolQuery(new Sha256(43594389025));

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CallCirrusGetExistsInMempoolQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(true);

        // Act
        var response = await _handler.Handle(request, CancellationToken.None);

        // Assert
        response.Should().Be(true);
    }

}
