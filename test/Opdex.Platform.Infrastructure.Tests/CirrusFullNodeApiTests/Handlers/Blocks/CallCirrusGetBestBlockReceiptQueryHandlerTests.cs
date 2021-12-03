using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Blocks;

public class CallCirrusGetBestBlockReceiptQueryHandlerTests
{
    private readonly Mock<IBlockStoreModule> _blockStoreModule;
    private readonly CallCirrusGetBestBlockReceiptQueryHandler _handler;

    public CallCirrusGetBestBlockReceiptQueryHandlerTests()
    {
        _blockStoreModule = new Mock<IBlockStoreModule>();
        _handler = new CallCirrusGetBestBlockReceiptQueryHandler(_blockStoreModule.Object);
    }

    [Fact]
    public async Task CallCirrusGetBestBlockReceiptQuery_Sends_GetBestBlockAsync()
    {
        // Arrange
        var command = new CallCirrusGetBestBlockReceiptQuery();

        // Act
        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch { }

        // Assert
        _blockStoreModule.Verify(callTo => callTo.GetBestBlockAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CallCirrusGetBestBlockReceiptQuery_Sends_GetBlockAsync()
    {
        // Arrange
        Sha256 bestBlock = Sha256.Parse("aaaa9e7e17058f070ab5ae015dab05fc974193afb578e245b2494631a9b28e95");
        var command = new CallCirrusGetBestBlockReceiptQuery();
        _blockStoreModule.Setup(callTo => callTo.GetBestBlockAsync(CancellationToken.None)).ReturnsAsync(bestBlock);

        // Act
        try
        {
            await _handler.Handle(command, CancellationToken.None);
        }
        catch { }

        // Assert
        _blockStoreModule.Verify(callTo => callTo.GetBlockAsync(bestBlock, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CallCirrusGetBestBlockReceiptQuery_FoundBlock_Returns()
    {
        // Arrange
        Sha256 bestBlock = Sha256.Parse("aaaa9e7e17058f070ab5ae015dab05fc974193afb578e245b2494631a9b28e95");
        var blockReceiptDto = new BlockReceiptDto
        {
            Hash = bestBlock,
            Height = 10,
            Time = ((ulong)DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds).ToString(),
            MedianTime = ((ulong)DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds).ToString(),
            PreviousBlockHash = Sha256.Parse("c974193afb578e245b2494631a9b28e95aaa9e7e17058f070ab5ae015dab05f2"),
            NextBlockHash = null,
            MerkleRoot = Sha256.Parse("9b28e95aaa9e7e5dab05f1705c974193afb5788f0e245b2494631a70ab5ae01d"),
        };

        var command = new CallCirrusGetBestBlockReceiptQuery();
        _blockStoreModule.Setup(callTo => callTo.GetBestBlockAsync(CancellationToken.None)).ReturnsAsync(bestBlock);
        _blockStoreModule.Setup(callTo => callTo.GetBlockAsync(bestBlock, CancellationToken.None)).ReturnsAsync(blockReceiptDto);


        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Hash.Should().Be(bestBlock);
        response.Height.Should().Be(blockReceiptDto.Height);
        response.PreviousBlockHash.Should().Be(blockReceiptDto.PreviousBlockHash);
        response.NextBlockHash.Should().Be(blockReceiptDto.NextBlockHash);
        response.MerkleRoot.Should().Be(blockReceiptDto.MerkleRoot);
    }
}