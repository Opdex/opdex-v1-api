using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.TokensControllerTests;

public class GetTokenByAddressTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IApplicationContext> _contextMock;
    private readonly TokensController _controller;

    public GetTokenByAddressTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _contextMock = new Mock<IApplicationContext>();

        _controller = new TokensController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
    }

    [Fact]
    public async Task GetToken_GetTokenByAddressQuery_Send()
    {
        // Arrange
        Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";

        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.GetToken(token, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetTokenByAddressQuery>(query => query.Address == token),
                                                   cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetToken_TokenDto_Map()
    {
        // Arrange
        Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        var dto = new TokenDto();

        var cancellationToken = new CancellationTokenSource().Token;

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetTokenByAddressQuery>(), cancellationToken)).ReturnsAsync(dto);

        // Act
        await _controller.GetToken(token, cancellationToken);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<TokenResponseModel>(It.IsAny<TokenDto>()), Times.Once);
    }

    [Fact]
    public async Task GetToken_ReturnOk()
    {
        // Arrange
        Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u";
        var dto = new TokenDto();
        var tokenResponse = new TokenResponseModel();

        var cancellationToken = new CancellationTokenSource().Token;

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetTokenByAddressQuery>(), cancellationToken)).ReturnsAsync(dto);
        _mapperMock.Setup(callTo => callTo.Map<TokenResponseModel>(It.IsAny<TokenDto>())).Returns(tokenResponse);

        // Act
        var response = await _controller.GetToken(token, cancellationToken);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(tokenResponse);
    }
}