using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Handlers.Blocks;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Blocks;

public class RetrieveCirrusBlockReceiptByHashQueryHandlerTests
{
    private readonly RetrieveCirrusBlockReceiptByHashQueryHandler _handler;
    private readonly Mock<IMediator> _mediator;

    public RetrieveCirrusBlockReceiptByHashQueryHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new RetrieveCirrusBlockReceiptByHashQueryHandler(_mediator.Object);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task RetrieveCirrusBlockReceiptByHashQuery_Sends_CallCirrusGetBlockReceiptByHashQuery(bool findOrThrow)
    {
        // Arrange
        Sha256 hash = Sha256.Parse("aaaa9e7e17058f070ab5ae015dab05fc974193afb578e245b2494631a9b28e95");

        // Act
        await _handler.Handle(new RetrieveCirrusBlockReceiptByHashQuery(hash, findOrThrow), CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<CallCirrusGetBlockReceiptByHashQuery>(q => q.Hash == hash &&
                                                                                                q.FindOrThrow == findOrThrow),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }
}