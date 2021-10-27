using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.OHLC;
using Opdex.Platform.Domain.Models.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Tokens.Snapshots
{
    public class CreateCrsTokenSnapshotsCommandHandlerTests
    {
        private readonly Mock<IMediator> _mediator;
        private readonly Mock<ILogger<CreateCrsTokenSnapshotsCommandHandler>> _logger;
        private readonly CreateCrsTokenSnapshotsCommandHandler _handler;

        public CreateCrsTokenSnapshotsCommandHandlerTests()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<CreateCrsTokenSnapshotsCommandHandler>>();
            _handler = new CreateCrsTokenSnapshotsCommandHandler(_mediator.Object, _logger.Object);
        }

        [Fact]
        public void CreateCrsTokenSnapshotsCommand_InvalidBlockTime_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            const ulong blockHeight = 10;

            // Act
            void Act() => new CreateCrsTokenSnapshotsCommand(default, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block time must be set.");
        }

        [Fact]
        public void CreateCrsTokenSnapshotsCommand_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            DateTime blockTime = DateTime.UtcNow;
            const ulong blockHeight = 0;

            // Act
            void Act() => new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CreateCrsTokenSnapshotsCommand_Sends_GetTokenByAddressQuery()
        {
            // Arrange
            DateTime blockTime = DateTime.UtcNow;
            const ulong blockHeight = 10;

            // Act
            try
            {
                await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<GetTokenByAddressQuery>(q => q.Address == Address.Cirrus), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateCrsTokenSnapshotsCommand_Sends_RetrieveTokenSnapshotWithFilterQuery()
        {
            // Arrange
            DateTime blockTime = DateTime.UtcNow;
            const ulong blockHeight = 10;
            var tokenDto = new TokenDto { Id = 1, Name = "Cirrus", Symbol = "CRS" };

            _mediator.Setup(callTo => callTo.Send(It.IsAny<GetTokenByAddressQuery>(), CancellationToken.None))
                .ReturnsAsync(tokenDto);

            // Act
            try
            {
                await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenSnapshotWithFilterQuery>(q => q.TokenId == tokenDto.Id &&
                                                                                                    q.MarketId == 0 &&
                                                                                                    q.DateTime == blockTime &&
                                                                                                    q.SnapshotType == SnapshotType.Minute), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateCrsTokenSnapshotsCommand_ReturnsTrue_SnapshotExists()
        {
            // Arrange
            DateTime blockTime = DateTime.UtcNow;
            const ulong blockHeight = 10;
            var tokenDto = new TokenDto { Id = 1, Name = "Cirrus", Symbol = "CRS" };
            var latestSnapshot = new TokenSnapshot(1, 2, 3, new OhlcDecimalSnapshot(), SnapshotType.Daily, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<GetTokenByAddressQuery>(), CancellationToken.None))
                .ReturnsAsync(tokenDto);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotWithFilterQuery>(), CancellationToken.None))
                .ReturnsAsync(latestSnapshot);

            // Act
            var response = await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveCmcStraxPriceQuery>(), CancellationToken.None), Times.Never);
            response.Should().BeTrue();
        }

        [Fact]
        public async Task CreateCrsTokenSnapshotsCommand_Sends_RetrieveCmcStraxPriceQuery()
        {
            // Arrange
            DateTime blockTime = DateTime.UtcNow;
            DateTime latestSnapshotTime = blockTime.AddMinutes(-5);
            const ulong blockHeight = 10;
            var tokenDto = new TokenDto { Id = 1, Name = "Cirrus", Symbol = "CRS" };
            var latestSnapshot = new TokenSnapshot(1, 2, 3, new OhlcDecimalSnapshot(), SnapshotType.Daily, latestSnapshotTime, latestSnapshotTime, latestSnapshotTime);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<GetTokenByAddressQuery>(), CancellationToken.None))
                .ReturnsAsync(tokenDto);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotWithFilterQuery>(), CancellationToken.None))
                .ReturnsAsync(latestSnapshot);

            // Act
            try
            {
                await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveCmcStraxPriceQuery>(q => q.BlockTime == blockTime), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateCrsTokenSnapshotsCommand_ReturnsFalse_FailureGettingStraxPrice()
        {
            // Arrange
            DateTime blockTime = DateTime.UtcNow;
            DateTime latestSnapshotTime = blockTime.AddMinutes(-5);
            const ulong blockHeight = 10;
            var tokenDto = new TokenDto { Id = 1, Name = "Cirrus", Symbol = "CRS" };
            var latestSnapshot = new TokenSnapshot(1, 2, 3, new OhlcDecimalSnapshot(), SnapshotType.Daily, latestSnapshotTime, latestSnapshotTime, latestSnapshotTime);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<GetTokenByAddressQuery>(), CancellationToken.None))
                .ReturnsAsync(tokenDto);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotWithFilterQuery>(), CancellationToken.None))
                .ReturnsAsync(latestSnapshot);

            // Act
            var response = await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotWithFilterQuery>(), CancellationToken.None), Times.AtMostOnce);
            response.Should().BeFalse();
        }

        [Fact]
        public async Task CreateCrsTokenSnapshotsCommand_Sends_RetrieveTokenSnapshotWithFilterQuery_ForEachSnapshotType()
        {
            // Arrange
            DateTime blockTime = DateTime.UtcNow;
            DateTime latestSnapshotTime = blockTime.AddMinutes(-5);
            const ulong blockHeight = 10;
            var tokenDto = new TokenDto { Id = 1, Name = "Cirrus", Symbol = "CRS" };
            var latestSnapshot = new TokenSnapshot(1, 2, 3, new OhlcDecimalSnapshot(), SnapshotType.Daily, latestSnapshotTime, latestSnapshotTime, latestSnapshotTime);
            const decimal price = 1.1m;

            _mediator.Setup(callTo => callTo.Send(It.IsAny<GetTokenByAddressQuery>(), CancellationToken.None))
                .ReturnsAsync(tokenDto);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotWithFilterQuery>(), CancellationToken.None))
                .ReturnsAsync(latestSnapshot);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveCmcStraxPriceQuery>(), CancellationToken.None))
                .ReturnsAsync(price);

            // Act
            await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenSnapshotWithFilterQuery>(q => q.TokenId == tokenDto.Id &&
                                                                                                    q.MarketId == 0 &&
                                                                                                    q.DateTime == blockTime &&
                                                                                                    q.SnapshotType == SnapshotType.Minute), CancellationToken.None), Times.Exactly(2));

            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenSnapshotWithFilterQuery>(q => q.TokenId == tokenDto.Id &&
                                                                                                    q.MarketId == 0 &&
                                                                                                    q.DateTime == blockTime &&
                                                                                                    q.SnapshotType == SnapshotType.Hourly), CancellationToken.None), Times.Once);

            _mediator.Verify(callTo => callTo.Send(It.Is<RetrieveTokenSnapshotWithFilterQuery>(q => q.TokenId == tokenDto.Id &&
                                                                                                    q.MarketId == 0 &&
                                                                                                    q.DateTime == blockTime &&
                                                                                                    q.SnapshotType == SnapshotType.Daily), CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task CreateCrsTokenSnapshotsCommand_Sends_MakeTokenSnapshotCommand_ForEachSnapshotType()
        {
            // Arrange
            DateTime blockTime = DateTime.UtcNow;
            DateTime latestSnapshotTime = blockTime.AddMinutes(-5);
            const ulong blockHeight = 10;
            var tokenDto = new TokenDto { Id = 1, Name = "Cirrus", Symbol = "CRS" };
            var latestSnapshot = new TokenSnapshot(1, 2, 3, new OhlcDecimalSnapshot(), SnapshotType.Daily, latestSnapshotTime, latestSnapshotTime, latestSnapshotTime);
            const decimal price = 1.1m;

            _mediator.Setup(callTo => callTo.Send(It.IsAny<GetTokenByAddressQuery>(), CancellationToken.None))
                .ReturnsAsync(tokenDto);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotWithFilterQuery>(), CancellationToken.None))
                .ReturnsAsync(latestSnapshot);

            _mediator.Setup(callTo => callTo.Send(It.IsAny<RetrieveCmcStraxPriceQuery>(), CancellationToken.None))
                .ReturnsAsync(price);

            // Act
            await _handler.Handle(new CreateCrsTokenSnapshotsCommand(blockTime, blockHeight), CancellationToken.None);

            // Assert
            _mediator.Verify(callTo => callTo.Send(It.Is<MakeTokenSnapshotCommand>(q => q.BlockHeight == blockHeight), CancellationToken.None), Times.Exactly(3));
        }
    }
}
