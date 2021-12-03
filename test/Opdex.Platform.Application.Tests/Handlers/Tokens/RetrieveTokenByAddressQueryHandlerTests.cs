using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Handlers.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Tokens;

public class RetrieveTokenByAddressQueryHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly RetrieveTokenByAddressQueryHandler _handler;

    public RetrieveTokenByAddressQueryHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _handler = new RetrieveTokenByAddressQueryHandler(_mediator.Object);
    }

    [Fact]
    public void RetrieveTokenByAddressQuery_InvalidAddress_ThrowsArgumentNullException()
    {
        // Arrange
        Address tokenAddress = Address.Empty;

        // Act
        void Act() => new RetrieveTokenByAddressQuery(tokenAddress);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Token address must be provided.");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task RetrieveTokenByAddressQuery_Sends_SelectTokenByAddressQuery(bool findOrThrow)
    {
        // Arrange
        Address tokenAddress = "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm";

        // Act
        await _handler.Handle(new RetrieveTokenByAddressQuery(tokenAddress, findOrThrow), CancellationToken.None);

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<SelectTokenByAddressQuery>(query => query.Address == tokenAddress &&
                                                                                         query.FindOrThrow == findOrThrow),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }
}