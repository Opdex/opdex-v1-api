using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.BlockStore;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Blocks
{
    public class CallCirrusGetBlockReceiptByHashQueryHandlerTests
    {
        private readonly Mock<IBlockStoreModule> _blockStoreModule;
        private readonly CallCirrusGetBlockReceiptByHashQueryHandler _handler;

        public CallCirrusGetBlockReceiptByHashQueryHandlerTests()
        {
            _blockStoreModule = new Mock<IBlockStoreModule>();
            _handler = new CallCirrusGetBlockReceiptByHashQueryHandler(_blockStoreModule.Object, NullLogger<CallCirrusGetBlockReceiptByHashQueryHandler>.Instance);
        }

        [Fact]
        public void CallCirrusGetBlockReceiptByHashQuery_InvalidHash_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            void Act() => new CallCirrusGetBlockReceiptByHashQuery(null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Block has must be provided.");
        }

        [Fact]
        public async Task CallCirrusGetBlockReceiptByHashQuery_Sends_GetBlockAsync()
        {
            // Arrange
            const string hash = "aaaa9e7e17058f070ab5ae015dab05fc974193afb578e245b2494631a9b28e95";
            const bool findOrThrow = true;
            var command = new CallCirrusGetBlockReceiptByHashQuery(hash, findOrThrow);

            // Act
            try
            {
                await _handler.Handle(command, CancellationToken.None);
            }
            catch { }

            // Assert
            _blockStoreModule.Verify(callTo => callTo.GetBlockAsync(hash, CancellationToken.None), Times.Once);
        }

        [Fact]
        public void CallCirrusGetBlockReceiptByHashQuery_NullBlock_FindOrThrowTrue_ThrowsNotFoundException()
        {
            // Arrange
            const string hash = "aaaa9e7e17058f070ab5ae015dab05fc974193afb578e245b2494631a9b28e95";
            const bool findOrThrow = true;
            var command = new CallCirrusGetBlockReceiptByHashQuery(hash, findOrThrow);
            _blockStoreModule.Setup(callTo => callTo.GetBlockAsync(hash, CancellationToken.None)).ReturnsAsync(() => null);

            // Act
            // Assert
            _handler
                .Invoking(callTo => callTo.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Block by hash not found.");
        }

        [Fact]
        public async Task CallCirrusGetBlockReceiptByHashQuery_NullBlock_FindOrThrowFalse_ReturnsNull()
        {
            // Arrange
            const string hash = "aaaa9e7e17058f070ab5ae015dab05fc974193afb578e245b2494631a9b28e95";
            const bool findOrThrow = false;
            var command = new CallCirrusGetBlockReceiptByHashQuery(hash, findOrThrow);
            _blockStoreModule.Setup(callTo => callTo.GetBlockAsync(hash, CancellationToken.None)).ReturnsAsync(() => null);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public void CallCirrusGetBlockReceiptByHashQuery_BlockThrows_FindOrThrowTrue_ThrowsNotFoundException()
        {
            // Arrange
            const string hash = "aaaa9e7e17058f070ab5ae015dab05fc974193afb578e245b2494631a9b28e95";
            const bool findOrThrow = true;
            var command = new CallCirrusGetBlockReceiptByHashQuery(hash, findOrThrow);
            _blockStoreModule.Setup(callTo => callTo.GetBlockAsync(hash, CancellationToken.None)).Throws<Exception>();

            // Act
            // Assert
            _handler
                .Invoking(callTo => callTo.Handle(command, CancellationToken.None))
                .Should()
                .ThrowAsync<NotFoundException>()
                .WithMessage("Block by hash not found.");
        }

        [Fact]
        public async Task CallCirrusGetBlockReceiptByHashQuery_BlockThrows_FindOrThrowFalse_ReturnsNull()
        {
            // Arrange
            const string hash = "aaaa9e7e17058f070ab5ae015dab05fc974193afb578e245b2494631a9b28e95";
            const bool findOrThrow = false;
            var command = new CallCirrusGetBlockReceiptByHashQuery(hash, findOrThrow);
            _blockStoreModule.Setup(callTo => callTo.GetBlockAsync(hash, CancellationToken.None)).Throws<Exception>();

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Should().BeNull();
        }

        [Fact]
        public async Task CallCirrusGetBlockReceiptByHashQuery_FoundBlock_Returns()
        {
            // Arrange
            const string hash = "aaaa9e7e17058f070ab5ae015dab05fc974193afb578e245b2494631a9b28e95";
            const bool findOrThrow = true;
            var blockReceiptDto = new BlockReceiptDto
            {
                Hash = hash,
                Height = 10,
                Time = ((ulong)(DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds)).ToString(),
                MedianTime = ((ulong)(DateTime.UtcNow.Subtract(DateTime.MinValue).TotalSeconds)).ToString(),
                PreviousBlockHash = "c974193afb578e245b2494631a9b28e95aaa9e7e17058f070ab5ae015dab05f",
                NextBlockHash = "e245b2494631a9b28e95aaa9e7e5dab05f1705c974193afb5788f070ab5ae01",
                MerkleRoot = "9b28e95aaa9e7e5dab05f1705c974193afb5788f0e245b2494631a70ab5ae01",
            };

            var command = new CallCirrusGetBlockReceiptByHashQuery(hash, findOrThrow);
            _blockStoreModule.Setup(callTo => callTo.GetBlockAsync(hash, CancellationToken.None))
                .ReturnsAsync(blockReceiptDto);

            // Act
            var response = await _handler.Handle(command, CancellationToken.None);

            // Assert
            response.Hash.Should().Be(hash);
            response.Height.Should().Be(blockReceiptDto.Height);
            response.PreviousBlockHash.Should().Be(blockReceiptDto.PreviousBlockHash);
            response.NextBlockHash.Should().Be(blockReceiptDto.NextBlockHash);
            response.MerkleRoot.Should().Be(blockReceiptDto.MerkleRoot);
        }
    }
}
