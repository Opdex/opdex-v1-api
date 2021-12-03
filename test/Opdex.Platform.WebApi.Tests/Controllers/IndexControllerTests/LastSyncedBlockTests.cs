using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.WebApi.Controllers;
using Opdex.Platform.WebApi.Models.Responses.Blocks;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Controllers.IndexControllerTests;

public class LastSyncedBlockTests
{
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly IndexController _controller;

    public LastSyncedBlockTests()
    {
        _mapperMock = new Mock<IMapper>();
        _mediatorMock = new Mock<IMediator>();

        var opdexConfiguration = new OpdexConfiguration { Network = NetworkType.DEVNET };

        _controller = new IndexController(_mapperMock.Object, _mediatorMock.Object, opdexConfiguration);
    }

    [Fact]
    public async Task GetLatestBlock_Send_GetLatestBlockQuery()
    {
        // Arrange
        var token = new CancellationTokenSource().Token;

        // Act
        await _controller.GetLastSyncedBlock(token);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<GetLatestBlockQuery>(), token), Times.Once);
    }

    [Fact]
    public async Task GetLatestBlock_Exists_Map()
    {
        // Arrange
        var blockDto = new BlockDto();
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetLatestBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(blockDto);

        // Act
        await _controller.GetLastSyncedBlock(CancellationToken.None);

        // Assert
        _mapperMock.Verify(callTo => callTo.Map<BlockResponseModel>(blockDto));
    }

    [Fact]
    public async Task GetLatestBlock_Exists_ReturnMapped()
    {
        // Arrange
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<GetLatestBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(new BlockDto());
        var blockResponse = new BlockResponseModel();
        _mapperMock.Setup(callTo => callTo.Map<BlockResponseModel>(It.IsAny<BlockDto>())).Returns(blockResponse);

        // Act
        var response = await _controller.GetLastSyncedBlock(CancellationToken.None);

        // Assert
        response.Result.Should().BeOfType<OkObjectResult>();
        ((OkObjectResult)response.Result).Value.Should().Be(blockResponse);
    }
}