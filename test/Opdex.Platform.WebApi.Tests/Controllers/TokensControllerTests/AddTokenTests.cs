using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Tokens;
using Opdex.Platform.WebApi.Models.Responses.Tokens;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.TokensControllerTests;

public class AddTokenTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IApplicationContext> _contextMock;
    private readonly TokensController _controller;

    public AddTokenTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _contextMock = new Mock<IApplicationContext>();

        _controller = new TokensController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
    }

    [Fact]
    public async Task AddToken_Succeess_ReturnCreated()
    {
        // Arrange
        Address token = "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3";

        var request = new AddTokenRequest { TokenAddress = token };
        var cancellationToken = new CancellationTokenSource().Token;

        var responseBody = new TokenResponseModel();

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<CreateAddTokenCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(new TokenDto());
        _mapperMock.Setup(callTo => callTo.Map<TokenResponseModel>(It.IsAny<TokenDto>())).Returns(responseBody);

        // Act
        var response = await _controller.AddToken(request, cancellationToken);

        // Act
        response.Should().BeOfType<CreatedResult>();
        ((CreatedResult)response).Value.Should().Be(responseBody);
        ((CreatedResult)response).Location.Should().Be("/tokens/PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDda3");
    }
}