using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens.Snapshots;
using Opdex.Platform.Application.EntryHandlers.Tokens.Snapshots;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.OHLC;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Tokens.Snapshots
{
    public class GetTokenSnapshotsWithFilterQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IMapper> _mapperMock;

        private readonly GetTokenSnapshotsWithFilterQueryHandler _handler;

        public GetTokenSnapshotsWithFilterQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _mapperMock = new Mock<IMapper>();

            _handler = new GetTokenSnapshotsWithFilterQueryHandler(_mediatorMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task Handle_RetreiveTokenByAddressQuery_Send()
        {
            // Arrange
            var token = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
            var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);
            var request = new GetTokenSnapshotsWithFilterQuery(token, cursor);

            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByAddressQuery>(query => query.Address == token
                                                                                                && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_RetrieveTokenSnapshotsWithFilterQuery_Send()
        {
            // Arrange
            var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
            var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, default, default, PagingDirection.Forward, default);
            var request = new GetTokenSnapshotsWithFilterQuery(tokenAddress, cursor);

            var cancellationToken = new CancellationTokenSource().Token;

            var token = new Token(5, tokenAddress, false, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenSnapshotsWithFilterQuery>(query => query.TokenId == token.Id
                                                                                                          && query.MarketId == 0
                                                                                                          && query.Cursor == cursor), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_SnapshotsRetrieved_MapResults()
        {
            // Arrange
            var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
            var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 10, PagingDirection.Forward, default);
            var request = new GetTokenSnapshotsWithFilterQuery(tokenAddress, cursor);

            var token = new Token(5, tokenAddress, false, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            var snapshots = new TokenSnapshot[]
            {
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z"), DateTime.Parse("2021-11-06T10:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z"), DateTime.Parse("2021-11-06T09:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z"), DateTime.Parse("2021-11-06T08:59:59Z"), DateTime.Now),
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);

            // Act
            var results = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>()), Times.Exactly(snapshots.Length));
        }

        [Fact]
        public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
        {
            // Arrange
            var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
            var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 3, PagingDirection.Forward, (DateTime.UtcNow, 10));
            var request = new GetTokenSnapshotsWithFilterQuery(tokenAddress, cursor);

            var token = new Token(5, tokenAddress, false, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            var snapshots = new TokenSnapshot[]
            {
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z"), DateTime.Parse("2021-11-06T10:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z"), DateTime.Parse("2021-11-06T09:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z"), DateTime.Parse("2021-11-06T08:59:59Z"), DateTime.Now),
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
            _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Snapshots.Count().Should().Be(snapshots.Length);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
        {
            // Arrange
            var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
            var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Backward, (DateTime.UtcNow, 10));
            var request = new GetTokenSnapshotsWithFilterQuery(tokenAddress, cursor);

            var token = new Token(5, tokenAddress, false, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            var snapshots = new TokenSnapshot[]
            {
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z"), DateTime.Parse("2021-11-06T10:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z"), DateTime.Parse("2021-11-06T09:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z"), DateTime.Parse("2021-11-06T08:59:59Z"), DateTime.Now),
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
            _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<TokenSnapshotDto>(snapshots[0]), Times.Never);
            dto.Snapshots.Count().Should().Be(snapshots.Length - 1);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
        {
            // Arrange
            var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
            var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, (DateTime.UtcNow, 10));
            var request = new GetTokenSnapshotsWithFilterQuery(tokenAddress, cursor);

            var token = new Token(5, tokenAddress, false, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            var snapshots = new TokenSnapshot[]
            {
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z"), DateTime.Parse("2021-11-06T10:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z"), DateTime.Parse("2021-11-06T09:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z"), DateTime.Parse("2021-11-06T08:59:59Z"), DateTime.Now),
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
            _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<TokenSnapshotDto>(snapshots[snapshots.Length - 1]), Times.Never);
            dto.Snapshots.Count().Should().Be(snapshots.Length - 1);
        }

        [Fact]
        public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
        {
            // Arrange
            var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
            var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, default);
            var request = new GetTokenSnapshotsWithFilterQuery(tokenAddress, cursor);

            var token = new Token(5, tokenAddress, false, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            var snapshots = new TokenSnapshot[]
            {
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z"), DateTime.Parse("2021-11-06T10:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z"), DateTime.Parse("2021-11-06T09:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z"), DateTime.Parse("2021-11-06T08:59:59Z"), DateTime.Now),
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
            _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, (snapshots[^2].StartDate, snapshots[^2].Id));
            dto.Cursor.Previous.Should().Be(null);
        }

        [Fact]
        public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
            var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, (DateTime.UtcNow, 50));
            var request = new GetTokenSnapshotsWithFilterQuery(tokenAddress, cursor);

            var token = new Token(5, tokenAddress, false, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            var snapshots = new TokenSnapshot[]
            {
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z"), DateTime.Parse("2021-11-06T10:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z"), DateTime.Parse("2021-11-06T09:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z"), DateTime.Parse("2021-11-06T08:59:59Z"), DateTime.Now),
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
            _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, (snapshots[^2].StartDate, snapshots[^2].Id));
            AssertPrevious(dto.Cursor, (snapshots[0].StartDate, snapshots[0].Id));
        }

        [Fact]
        public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
            var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Backward, (DateTime.UtcNow, 50));
            var request = new GetTokenSnapshotsWithFilterQuery(tokenAddress, cursor);

            var token = new Token(5, tokenAddress, false, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            var snapshots = new TokenSnapshot[]
            {
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z"), DateTime.Parse("2021-11-06T10:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z"), DateTime.Parse("2021-11-06T09:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4.5m, 9.5m, 4.5m, 6m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T08:00:00Z"), DateTime.Parse("2021-11-06T08:59:59Z"), DateTime.Now),
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
            _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, (snapshots[^1].StartDate, snapshots[^1].Id));
            AssertPrevious(dto.Cursor, (snapshots[1].StartDate, snapshots[1].Id));
        }

        [Fact]
        public async Task Handle_PagingForwardLastPage_ReturnCursor()
        {
            // Arrange
            var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
            var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Forward, (DateTime.UtcNow, 50));
            var request = new GetTokenSnapshotsWithFilterQuery(tokenAddress, cursor);

            var token = new Token(5, tokenAddress, false, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            var snapshots = new TokenSnapshot[]
            {
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z"), DateTime.Parse("2021-11-06T10:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z"), DateTime.Parse("2021-11-06T09:59:59Z"), DateTime.Now),
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
            _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Cursor.Next.Should().Be(null);
            AssertPrevious(dto.Cursor, (snapshots[0].StartDate, snapshots[0].Id));
        }

        [Fact]
        public async Task Handle_PagingBackwardLastPage_ReturnCursor()
        {
            // Arrange
            var tokenAddress = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm");
            var cursor = new SnapshotCursor(Interval.OneHour, DateTime.UtcNow.AddDays(-5), DateTime.UtcNow, SortDirectionType.ASC, 2, PagingDirection.Backward, (DateTime.UtcNow, 50));
            var request = new GetTokenSnapshotsWithFilterQuery(tokenAddress, cursor);

            var token = new Token(5, tokenAddress, false, "Governance", "GOV", 8, 100000000, UInt256.Parse("500000000000000000000"), 20, 25);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            var snapshots = new TokenSnapshot[]
            {
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(5m, 6.5m, 2m, 4m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T10:00:00Z"), DateTime.Parse("2021-11-06T10:59:59Z"), DateTime.Now),
                new TokenSnapshot(3, 5, 0, new OhlcDecimalSnapshot(4m, 4.5m, 3.75m, 4.5m), SnapshotType.Hourly, DateTime.Parse("2021-11-06T09:00:00Z"), DateTime.Parse("2021-11-06T09:59:59Z"), DateTime.Now),
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenSnapshotsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(snapshots);
            _mapperMock.Setup(callTo => callTo.Map<TokenSnapshotDto>(It.IsAny<TokenSnapshot>())).Returns(new TokenSnapshotDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, (snapshots[^1].StartDate, snapshots[^1].Id));
            dto.Cursor.Previous.Should().Be(null);
        }

        private void AssertNext(CursorDto dto, (DateTime, ulong) pointer)
        {
            SnapshotCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Forward);
            next.Pointer.Should().Be(pointer);
        }

        private void AssertPrevious(CursorDto dto, (DateTime, ulong) pointer)
        {
            SnapshotCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Backward);
            next.Pointer.Should().Be(pointer);
        }
    }
}
