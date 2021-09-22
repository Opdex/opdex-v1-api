using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.MiningPools;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Queries.MiningPools;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.MiningPools;
using Opdex.Platform.Application.EntryHandlers.MiningPools.Quotes;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.MiningPools
{
    public class GetMiningPoolsWithFilterQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<MiningPool, MiningPoolDto>> _assemblerMock;

        private readonly GetMiningPoolsWithFilterQueryHandler _handler;

        public GetMiningPoolsWithFilterQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<MiningPool, MiningPoolDto>>();

            _handler = new GetMiningPoolsWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
        }

        [Fact]
        public async Task Handle_RetrieveMiningPoolsWithFilterQuery_Send()
        {
            // Arrange
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Active, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetMiningPoolsWithFilterQuery(cursor);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMiningPoolsWithFilterQuery>(query => query.Cursor == cursor), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_MiningPoolsRetrieved_MapResults()
        {
            // Arrange
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Active, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetMiningPoolsWithFilterQuery(cursor);

            var miningPools = new MiningPool[]
            {
                new MiningPool(5, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(10, 5, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(15, 5, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPools);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(It.IsAny<MiningPool>()), Times.Exactly(miningPools.Length));
        }

        [Fact]
        public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
        {
            // Arrange
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Active, SortDirectionType.ASC, 3, PagingDirection.Backward, 55);
            var request = new GetMiningPoolsWithFilterQuery(cursor);

            var miningPools = new MiningPool[]
            {
                new MiningPool(5, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(10, 5, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(15, 5, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPools);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningPool>())).ReturnsAsync(new MiningPoolDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.MiningPools.Count().Should().Be(miningPools.Length);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
        {
            // Arrange
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Active, SortDirectionType.ASC, 2, PagingDirection.Backward, 55);
            var request = new GetMiningPoolsWithFilterQuery(cursor);

            var miningPools = new MiningPool[]
            {
                new MiningPool(5, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(10, 5, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(15, 5, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPools);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningPool>())).ReturnsAsync(new MiningPoolDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(miningPools[0]), Times.Never);
            dto.MiningPools.Count().Should().Be(miningPools.Length - 1);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
        {
            // Arrange
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Active, SortDirectionType.ASC, 2, PagingDirection.Forward, 55);
            var request = new GetMiningPoolsWithFilterQuery(cursor);

            var miningPools = new MiningPool[]
            {
                new MiningPool(5, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(10, 5, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(15, 5, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPools);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningPool>())).ReturnsAsync(new MiningPoolDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(miningPools[miningPools.Length - 1]), Times.Never);
            dto.MiningPools.Count().Should().Be(miningPools.Length - 1);
        }

        [Fact]
        public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
        {
            // Arrange
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Active, SortDirectionType.ASC, 2, PagingDirection.Forward, 0);
            var request = new GetMiningPoolsWithFilterQuery(cursor);

            var miningPools = new MiningPool[]
            {
                new MiningPool(5, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(10, 5, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(15, 5, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPools);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningPool>())).ReturnsAsync(new MiningPoolDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, miningPools[^2].Id);
            dto.Cursor.Previous.Should().Be(null);
        }

        [Fact]
        public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Active, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
            var request = new GetMiningPoolsWithFilterQuery(cursor);

            var miningPools = new MiningPool[]
            {
                new MiningPool(5, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(10, 5, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(15, 5, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPools);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningPool>())).ReturnsAsync(new MiningPoolDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, miningPools[^2].Id);
            AssertPrevious(dto.Cursor, miningPools[0].Id);
        }

        [Fact]
        public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Active, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
            var request = new GetMiningPoolsWithFilterQuery(cursor);

            var miningPools = new MiningPool[]
            {
                new MiningPool(5, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(10, 5, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(15, 5, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPools);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningPool>())).ReturnsAsync(new MiningPoolDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, miningPools[^1].Id);
            AssertPrevious(dto.Cursor, miningPools[1].Id);
        }

        [Fact]
        public async Task Handle_PagingForwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Active, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
            var request = new GetMiningPoolsWithFilterQuery(cursor);

            var miningPools = new MiningPool[]
            {
                new MiningPool(5, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(10, 5, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPools);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningPool>())).ReturnsAsync(new MiningPoolDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Cursor.Next.Should().Be(null);
            AssertPrevious(dto.Cursor, miningPools[0].Id);
        }

        [Fact]
        public async Task Handle_PagingBackwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Active, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
            var request = new GetMiningPoolsWithFilterQuery(cursor);

            var miningPools = new MiningPool[]
            {
                new MiningPool(5, 5, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505),
                new MiningPool(10, 5, "PKRQitdLu4FGWuJKbrCVLqnVPQtYyRwN76", UInt256.Parse("5000000"), UInt256.Parse("1000"), 500000, 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningPoolsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningPools);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningPool>())).ReturnsAsync(new MiningPoolDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, miningPools[^1].Id);
            dto.Cursor.Previous.Should().Be(null);
        }

        private void AssertNext(CursorDto dto, long pointer)
        {
            MiningPoolsCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Forward);
            next.Pointer.Should().Be(pointer);
        }

        private void AssertPrevious(CursorDto dto, long pointer)
        {
            MiningPoolsCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Backward);
            next.Pointer.Should().Be(pointer);
        }
    }
}
