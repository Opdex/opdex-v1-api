using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Governances;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Governances;
using Opdex.Platform.Application.Abstractions.Queries.Governances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Governances;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Governances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Governances;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Governances
{
    public class GetMiningGovernancesWithFilterQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<MiningGovernance, MiningGovernanceDto>> _assemblerMock;

        private readonly GetMiningGovernancesWithFilterQueryHandler _handler;

        public GetMiningGovernancesWithFilterQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<MiningGovernance, MiningGovernanceDto>>();

            _handler = new GetMiningGovernancesWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
        }

        [Fact]
        public async Task Handle_RetrieveMiningGovernancesWithFilterQuery_Send()
        {
            // Arrange
            var cursor = new MiningGovernancesCursor(Address.Empty, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetMiningGovernancesWithFilterQuery(cursor);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveMiningGovernancesWithFilterQuery>(query => query.Cursor == cursor), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_MiningGovernancesRetrieved_MapResults()
        {
            // Arrange
            var cursor = new MiningGovernancesCursor(Address.Empty, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetMiningGovernancesWithFilterQuery(cursor);

            var miningGovernances = new MiningGovernance[]
            {
                new MiningGovernance(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningGovernances);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(It.IsAny<MiningGovernance>()), Times.Exactly(miningGovernances.Length));
        }

        [Fact]
        public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
        {
            // Arrange
            var cursor = new MiningGovernancesCursor(Address.Empty, SortDirectionType.ASC, 3, PagingDirection.Backward, 55);
            var request = new GetMiningGovernancesWithFilterQuery(cursor);

            var miningGovernances = new MiningGovernance[]
            {
                new MiningGovernance(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningGovernances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningGovernance>())).ReturnsAsync(new MiningGovernanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Governances.Count().Should().Be(miningGovernances.Length);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
        {
            // Arrange
            var cursor = new MiningGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Backward, 55);
            var request = new GetMiningGovernancesWithFilterQuery(cursor);

            var miningGovernances = new MiningGovernance[]
            {
                new MiningGovernance(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningGovernances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningGovernance>())).ReturnsAsync(new MiningGovernanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(miningGovernances[0]), Times.Never);
            dto.Governances.Count().Should().Be(miningGovernances.Length - 1);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
        {
            // Arrange
            var cursor = new MiningGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 55);
            var request = new GetMiningGovernancesWithFilterQuery(cursor);

            var miningGovernances = new MiningGovernance[]
            {
                new MiningGovernance(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningGovernances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningGovernance>())).ReturnsAsync(new MiningGovernanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(miningGovernances[miningGovernances.Length - 1]), Times.Never);
            dto.Governances.Count().Should().Be(miningGovernances.Length - 1);
        }

        [Fact]
        public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
        {
            // Arrange
            var cursor = new MiningGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 0);
            var request = new GetMiningGovernancesWithFilterQuery(cursor);

            var miningGovernances = new MiningGovernance[]
            {
                new MiningGovernance(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningGovernances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningGovernance>())).ReturnsAsync(new MiningGovernanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, miningGovernances[^2].Id);
            dto.Cursor.Previous.Should().Be(null);
        }

        [Fact]
        public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new MiningGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
            var request = new GetMiningGovernancesWithFilterQuery(cursor);

            var miningGovernances = new MiningGovernance[]
            {
                new MiningGovernance(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningGovernances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningGovernance>())).ReturnsAsync(new MiningGovernanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, miningGovernances[^2].Id);
            AssertPrevious(dto.Cursor, miningGovernances[0].Id);
        }

        [Fact]
        public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new MiningGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
            var request = new GetMiningGovernancesWithFilterQuery(cursor);

            var miningGovernances = new MiningGovernance[]
            {
                new MiningGovernance(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(15, "P9vqymoKE6JbeLmnbDS9HsZ68QZf15ed71", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningGovernances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningGovernance>())).ReturnsAsync(new MiningGovernanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, miningGovernances[^1].Id);
            AssertPrevious(dto.Cursor, miningGovernances[1].Id);
        }

        [Fact]
        public async Task Handle_PagingForwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new MiningGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
            var request = new GetMiningGovernancesWithFilterQuery(cursor);

            var miningGovernances = new MiningGovernance[]
            {
                new MiningGovernance(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningGovernances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningGovernance>())).ReturnsAsync(new MiningGovernanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Cursor.Next.Should().Be(null);
            AssertPrevious(dto.Cursor, miningGovernances[0].Id);
        }

        [Fact]
        public async Task Handle_PagingBackwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new MiningGovernancesCursor(Address.Empty, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
            var request = new GetMiningGovernancesWithFilterQuery(cursor);

            var miningGovernances = new MiningGovernance[]
            {
                new MiningGovernance(5, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505),
                new MiningGovernance(10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", 10, 500000, 10000, 15, UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveMiningGovernancesWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(miningGovernances);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<MiningGovernance>())).ReturnsAsync(new MiningGovernanceDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, miningGovernances[^1].Id);
            dto.Cursor.Previous.Should().Be(null);
        }

        private void AssertNext(CursorDto dto, ulong pointer)
        {
            MiningGovernancesCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Forward);
            next.Pointer.Should().Be(pointer);
        }

        private void AssertPrevious(CursorDto dto, ulong pointer)
        {
            MiningGovernancesCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Backward);
            next.Pointer.Should().Be(pointer);
        }
    }
}
