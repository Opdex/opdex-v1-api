using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.EntryHandlers.Blocks;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Blocks;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Blocks
{
    public class GetLatestBlockQueryHandlerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly GetLatestBlockQueryHandler _handler;

        public GetLatestBlockQueryHandlerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();

            _handler = new GetLatestBlockQueryHandler(_mapperMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Handle_RetrieveLatestBlockQuery_Send()
        {
            // Arrange
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(new GetLatestBlockQuery(), cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveLatestBlockQuery>(query => !query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_NoBlockFound_ThrowNotFound()
        {
            // Arrange
            // Act
            Task Act() => _handler.Handle(new GetLatestBlockQuery(), CancellationToken.None);

            // Assert
            await Assert.ThrowsAsync<NotFoundException>(Act);
        }

        [Fact]
        public async Task Handle_Found_Map()
        {
            // Arrange
            var block = new Block(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(block);

            // Act
            await _handler.Handle(new GetLatestBlockQuery(), CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<BlockDto>(block), Times.Once);
        }

        [Fact]
        public async Task Handle_Found_ReturnMapped()
        {
            // Arrange
            var blockDto = new BlockDto();
            var block = new Block(10, Sha256.Parse("18236e42c337ee0b8a23df39523a904853ac9a1e42120a5086420ecf9c79b147"), DateTime.UtcNow, DateTime.UtcNow);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(block);
            _mapperMock.Setup(callTo => callTo.Map<BlockDto>(block)).Returns(blockDto);

            // Act
            var response = await _handler.Handle(new GetLatestBlockQuery(), CancellationToken.None);

            // Assert
            response.Should().Be(blockDto);
        }
    }
}