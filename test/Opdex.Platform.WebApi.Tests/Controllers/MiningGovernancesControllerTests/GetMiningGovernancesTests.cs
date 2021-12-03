using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningGovernances;
using Opdex.Platform.Application.Abstractions.Models.MiningGovernances;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models;
using Opdex.Platform.WebApi.Models.Requests.MiningGovernances;
using Opdex.Platform.WebApi.Models.Responses.MiningGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.MiningGovernancesControllerTests;

public class GetMiningGovernancesTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IApplicationContext> _applicationContextMock;

    private readonly MiningGovernancesController _controller;

    public GetMiningGovernancesTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();
        _applicationContextMock = new Mock<IApplicationContext>();

        _controller = new MiningGovernancesController(_mediatorMock.Object, _mapperMock.Object, _applicationContextMock.Object);
    }

    [Fact]
    public async Task GetGovernances_GetMiningGovernancesWithFilterQuery_Send()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.GetMiningGovernances(new MiningGovernanceFilterParameters(), cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetMiningGovernancesWithFilterQuery>(query => query.Cursor != null), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetGovernances_Result_ReturnOk()
    {
        // Arrange
        var miningGovernances = new MiningGovernancesResponseModel();

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetMiningGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new MiningGovernancesDto());
        _mapperMock.Setup(callTo => callTo.Map<MiningGovernancesResponseModel>(It.IsAny<MiningGovernancesDto>())).Returns(miningGovernances);

        // Act
        var response = await _controller.GetMiningGovernances(new MiningGovernanceFilterParameters(), CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(miningGovernances);
    }
}