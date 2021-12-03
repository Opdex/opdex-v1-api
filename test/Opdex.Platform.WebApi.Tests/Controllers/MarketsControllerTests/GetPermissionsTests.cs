using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Markets;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MarketsControllerTests;

public class GetPermissionsTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly MarketsController _controller;

    public GetPermissionsTests()
    {
        _mediatorMock = new Mock<IMediator>();

        _controller = new MarketsController(_mediatorMock.Object, Mock.Of<IMapper>(), Mock.Of<IApplicationContext>());
    }

    [Fact]
    public async Task GetPermissions_GetMarketPermissionsForAddressQuery_Send()
    {
        // Arrange
        Address market = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
        Address wallet = "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy";
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.GetPermissions(market, wallet, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMarketPermissionsForAddressQuery>(query
                                                                                                  => query.Market == market
                                                                                                     && query.Wallet == wallet
                                                   ), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetPermissions_GetMarketPermissionsForAddressQuery_Return()
    {
        // Arrange
        Address market = "PR71udY85pAcNcitdDfzQevp6Zar9DizHM";
        Address wallet = "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy";
        var cancellationToken = new CancellationTokenSource().Token;

        var marketPermissions = new MarketPermissionType[]
        {
            MarketPermissionType.CreatePool, MarketPermissionType.Trade, MarketPermissionType.SetPermissions
        };

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMarketPermissionsForAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(marketPermissions);

        // Act
        var response = await _controller.GetPermissions(market, wallet, cancellationToken);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().BeEquivalentTo(marketPermissions);
    }
}