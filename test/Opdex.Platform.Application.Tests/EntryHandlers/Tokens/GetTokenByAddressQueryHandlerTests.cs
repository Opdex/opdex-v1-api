using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Tokens;

public class GetTokenByAddressQueryHandlerTests
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<IModelAssembler<Token, TokenDto>> _assembler;
    private readonly GetTokenByAddressQueryHandler _handler;

    public GetTokenByAddressQueryHandlerTests()
    {
        _mediator = new Mock<IMediator>();
        _assembler = new Mock<IModelAssembler<Token, TokenDto>>();
        _handler = new GetTokenByAddressQueryHandler(_mediator.Object, _assembler.Object);
    }

    [Fact]
    public void GetTokenByAddressQuery_InvalidAddress_ThrowsArgumentNullException()
    {
        // Arrange
        Address tokenAddress = Address.Empty;

        // Act
        void Act() => new GetTokenByAddressQuery(tokenAddress);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Contains("Token address must be provided.");
    }

    [Fact]
    public async Task GetTokenByAddressQuery_Sends_RetrieveByTokenAddressQuery()
    {
        // Arrange
        Address tokenAddress = "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm";

        // Act
        try
        {
            await _handler.Handle(new GetTokenByAddressQuery(tokenAddress), CancellationToken.None);
        }
        catch { }

        // Assert
        _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == tokenAddress &&
                                                                                           query.FindOrThrow == true),
                                               It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetTokenByAddressQuery_Sends_AssembleTokenDto()
    {
        // Arrange
        Address tokenAddress = "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm";
        var token = new Token(1, tokenAddress, false, "STRAX", "STRAX", 8, 100_000_000, new UInt256("10000000000000000"), 9, 10);

        _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

        // Act
        await _handler.Handle(new GetTokenByAddressQuery(tokenAddress), CancellationToken.None);


        // Assert
        _assembler.Verify(callTo => callTo.Assemble(token), Times.Once);
    }
}