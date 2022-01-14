using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Routers;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.MarketTokens;
using Opdex.Platform.WebApi.Models.Responses.MarketTokens;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MarketTokensControllerTests;

public class SwapAmountOutTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IApplicationContext> _contextMock;
    private readonly MarketTokensController _controller;

    public SwapAmountOutTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _contextMock = new Mock<IApplicationContext>();

        _controller = new MarketTokensController(_mediatorMock.Object, _mapperMock.Object, _contextMock.Object);
    }

    [Fact]
    public async Task SwapAmountOut_GetLiquidityPoolSwapAmountOutQuery_Send()
    {
        // Arrange
        Address market = new Address("t7RorA7xQCMVYKPM1ibPE1NSswaLbpqLQb");
        Address tokenOut = new Address("tNgQhNxvachxKGvRonk2S8nrpYi44carYv");
        var request = new SwapAmountOutQuoteRequestModel
        {
            TokenIn = new Address("tUHwBBmhHbaBA49hVhuVNUDmreGjSFceuD"),
            TokenInAmount = FixedDecimal.Parse("10.50000000")
        };

        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.SwapAmountOut(market, tokenOut, request, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetSwapAmountOutQuery>(query => query.Market == market
                                                                                         && query.TokenIn == request.TokenIn
                                                                                         && query.TokenInAmount == request.TokenInAmount
                                                                                         && query.TokenOut == tokenOut), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task SwapAmountOut_Response_ReturnOk()
    {
        // Arrange
        Address market = new Address("t7RorA7xQCMVYKPM1ibPE1NSswaLbpqLQb");
        Address tokenOut = new Address("tNgQhNxvachxKGvRonk2S8nrpYi44carYv");
        var request = new SwapAmountOutQuoteRequestModel
        {
            TokenIn = new Address("tUHwBBmhHbaBA49hVhuVNUDmreGjSFceuD"),
            TokenInAmount = FixedDecimal.Parse("10.50000000")
        };

        FixedDecimal amountOut = FixedDecimal.Parse("2.55558888");

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetSwapAmountOutQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(amountOut);

        // Act
        var response = await _controller.SwapAmountOut(market, tokenOut, request, CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        ((SwapAmountOutQuoteResponseModel)((OkObjectResult)response.Result).Value).AmountOut.Should().Be(amountOut);
    }
}