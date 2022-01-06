using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.Markets;
using Opdex.Platform.WebApi.Models.Responses.Markets;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MarketsControllerTests;

public class GetMarketsTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly MarketsController _controller;

    public GetMarketsTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();
        _controller = new MarketsController(_mediatorMock.Object, _mapperMock.Object, Mock.Of<IApplicationContext>());
    }

    [Fact]
    public async Task GetMarkets_GetMarketsWithFilterQuery_Send()
    {
        // Arrange
        var filters = new MarketFilterParameters
        {
            MarketType = MarketType.Standard,
            OrderBy = MarketOrderByType.VolumeUsd
        };

        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.GetMarkets(filters, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMarketsWithFilterQuery>(
                query => query.Cursor.Type == filters.MarketType
                            && query.Cursor.OrderBy == filters.OrderBy
                            && query.Cursor.IsFirstRequest),
            cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetMarkets_MarketDto_Map()
    {
        // Arrange
        var filters = new MarketFilterParameters();

        var dto = new MarketsDto();

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMarketsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        // Act
        await _controller.GetMarkets(filters, CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<MarketsResponseModel>(It.IsAny<MarketsDto>()), Times.Once);
    }

    [Fact]
    public async Task GetMarkets_ReturnOk()
    {
        // Arrange
        var filters = new MarketFilterParameters();

        var dto = new MarketsDto();
        var marketResponse = new MarketsResponseModel();

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMarketsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        _mapperMock.Setup(callTo => callTo.Map<MarketsResponseModel>(It.IsAny<MarketsDto>())).Returns(marketResponse);

        // Act
        var response = await _controller.GetMarkets(filters, CancellationToken.None);

        // Act
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result!).Value.Should().Be(marketResponse);
    }
}
