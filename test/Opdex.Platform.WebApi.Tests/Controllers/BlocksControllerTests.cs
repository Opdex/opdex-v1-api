using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Models.Blocks;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models.Requests.Blocks;
using Opdex.Platform.WebApi.Models.Responses.Blocks;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers;

public class BlocksControllerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IMapper> _mapperMock;

    private readonly BlocksController _controller;

    public BlocksControllerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mapperMock = new Mock<IMapper>();

        _controller = new BlocksController(_mapperMock.Object, _mediatorMock.Object);
    }

    [Fact]
    public async Task GetBlocks_GetBlocksWithFilterQuery_Send()
    {
        // Arrange
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        await _controller.GetBlocks(new BlockFilterParameters(), cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<GetBlocksWithFilterQuery>(query => query.Cursor != null), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetBlocks_Result_ReturnOk()
    {
        // Arrange
        var vaults = new BlocksResponseModel();

        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetBlocksWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new BlocksDto());
        _mapperMock.Setup(callTo => callTo.Map<BlocksResponseModel>(It.IsAny<BlocksDto>())).Returns(vaults);

        // Act
        var response = await _controller.GetBlocks(new BlockFilterParameters(), CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result!).Value.Should().Be(vaults);
    }

    [Fact]
    public async Task GetBlock_GetBlockByHeightQuery_Send()
    {
        // Arrange
        ulong height = 10;

        using var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;

        // Act
        await _controller.GetBlock(height, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(
            It.Is<GetBlockByHeightQuery>(query => query.Height == height), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task GetBlock_BlockExists_MapResult()
    {
        // Arrange
        var dto = new BlockDto();
        _mediatorMock.Setup(callTo => callTo.Send(
            It.IsAny<GetBlockByHeightQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);

        // Act
        await _controller.GetBlock(5, CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<BlockResponseModel>(dto), Times.Once);
    }

    [Fact]
    public async Task GetBlock_BlockExists_Return200Ok()
    {
        // Arrange
        var expectedResponse = new BlockResponseModel();
        var dto = new BlockDto();
        _mediatorMock.Setup(callTo => callTo.Send(
            It.IsAny<GetBlockByHeightQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        _mapperMock.Setup(callTo => callTo.Map<BlockResponseModel>(dto)).Returns(expectedResponse);

        // Act
        var response = await _controller.GetBlock(5, CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        (((OkObjectResult)response.Result)!).Value.Should().Be(expectedResponse);
    }
}
