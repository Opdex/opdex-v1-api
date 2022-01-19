using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Indexer;
using Opdex.Platform.Application.Abstractions.Models.Index;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models.Responses.Index;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.IndexControllerTests;

public class GetIndexerStatusTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly IndexerController _controller;

    public GetIndexerStatusTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();

        var opdexConfiguration = new OpdexConfiguration { Network = NetworkType.DEVNET };

        _controller = new IndexerController(_mapperMock.Object, _mediatorMock.Object, opdexConfiguration);
    }

    [Fact]
    public async Task GetIndexerStatus_Send_GetIndexerStatusQuery()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;

        // Act
        await _controller.GetIndexerStatus(token);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<GetIndexerStatusQuery>(), token), Times.Once);
    }

    [Fact]
    public async Task GetIndexerStatus_Exists_Map()
    {
        // Arrange
        var IndexerStatusDto = new IndexerStatusDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetIndexerStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(IndexerStatusDto);

        // Act
        await _controller.GetIndexerStatus(CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<IndexerStatusResponseModel>(IndexerStatusDto));
    }

    [Fact]
    public async Task GetIndexerStatus_Exists_ReturnMapped()
    {
        // Arrange
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetIndexerStatusQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new IndexerStatusDto());
        var blockResponse = new IndexerStatusResponseModel();
        _mapperMock.Setup(callTo => callTo.Map<IndexerStatusResponseModel>(It.IsAny<IndexerStatusDto>())).Returns(blockResponse);

        // Act
        var response = await _controller.GetIndexerStatus(CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        (((OkObjectResult)response.Result)!).Value.Should().Be(blockResponse);
    }
}
