using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Staking;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Staking;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Addresses.Staking;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Addresses.Staking
{
    public class GetStakingPositionsWithFilterQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<AddressStaking, StakingPositionDto>> _assemblerMock;

        private readonly GetStakingPositionsWithFilterQueryHandler _handler;

        public GetStakingPositionsWithFilterQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _assemblerMock = new Mock<IModelAssembler<AddressStaking, StakingPositionDto>>();

            _handler = new GetStakingPositionsWithFilterQueryHandler(_mediatorMock.Object, _assemblerMock.Object);
        }

        [Fact]
        public async Task Handle_GetStakingPositionsWithFilterQuery_Send()
        {
            // Arrange
            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetStakingPositionsWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            try
            {
                await _handler.Handle(request, cancellationToken);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveStakingPositionsWithFilterQuery>(query => query.Address == request.Address
                                                                                                            && query.Cursor == cursor), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_PositionsRetrieved_MapResults()
        {
            // Arrange
            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);
            var request = new GetStakingPositionsWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var position = new AddressStaking(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", UInt256.Parse("10000000000"), 500, 505);
            var positions = new AddressStaking[] { position };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveStakingPositionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(positions);

            // Act
            await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(position), Times.Once);
        }

        [Fact]
        public async Task Handle_LessThanLimitPlusOneResults_RemoveZero()
        {
            // Arrange
            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, SortDirectionType.ASC, 3, PagingDirection.Backward, 55);
            var request = new GetStakingPositionsWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var positions = new AddressStaking[]
            {
                new AddressStaking(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveStakingPositionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(positions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressStaking>())).ReturnsAsync(new StakingPositionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Positions.Count().Should().Be(positions.Length);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingBackward_RemoveFirst()
        {
            // Arrange
            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, SortDirectionType.ASC, 2, PagingDirection.Backward, 55);
            var request = new GetStakingPositionsWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var positions = new AddressStaking[]
            {
                new AddressStaking(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveStakingPositionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(positions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressStaking>())).ReturnsAsync(new StakingPositionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(positions[0]), Times.Never);
            dto.Positions.Count().Should().Be(positions.Length - 1);
        }

        [Fact]
        public async Task Handle_LimitPlusOneResultsPagingForward_RemoveLast()
        {
            // Arrange
            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, SortDirectionType.ASC, 2, PagingDirection.Forward, 55);
            var request = new GetStakingPositionsWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var positions = new AddressStaking[]
            {
                new AddressStaking(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveStakingPositionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(positions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressStaking>())).ReturnsAsync(new StakingPositionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            _assemblerMock.Verify(callTo => callTo.Assemble(positions[positions.Length - 1]), Times.Never);
            dto.Positions.Count().Should().Be(positions.Length - 1);
        }

        [Fact]
        public async Task Handle_FirstRequestInPagedResults_ReturnCursor()
        {
            // Arrange
            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, SortDirectionType.ASC, 2, PagingDirection.Forward, 0);
            var request = new GetStakingPositionsWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var positions = new AddressStaking[]
            {
                new AddressStaking(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveStakingPositionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(positions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressStaking>())).ReturnsAsync(new StakingPositionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, positions[^2].Id);
            dto.Cursor.Previous.Should().Be(null);
        }

        [Fact]
        public async Task Handle_PagingForwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
            var request = new GetStakingPositionsWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var positions = new AddressStaking[]
            {
                new AddressStaking(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveStakingPositionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(positions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressStaking>())).ReturnsAsync(new StakingPositionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, positions[^2].Id);
            AssertPrevious(dto.Cursor, positions[0].Id);
        }

        [Fact]
        public async Task Handle_PagingBackwardWithMoreResults_ReturnCursor()
        {
            // Arrange
            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
            var request = new GetStakingPositionsWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var positions = new AddressStaking[]
            {
                new AddressStaking(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(15, 10, "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveStakingPositionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(positions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressStaking>())).ReturnsAsync(new StakingPositionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, positions[^1].Id);
            AssertPrevious(dto.Cursor, positions[1].Id);
        }

        [Fact]
        public async Task Handle_PagingForwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, SortDirectionType.ASC, 2, PagingDirection.Forward, 50);
            var request = new GetStakingPositionsWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var positions = new AddressStaking[]
            {
                new AddressStaking(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveStakingPositionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(positions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressStaking>())).ReturnsAsync(new StakingPositionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            dto.Cursor.Next.Should().Be(null);
            AssertPrevious(dto.Cursor, positions[0].Id);
        }

        [Fact]
        public async Task Handle_PagingBackwardLastPage_ReturnCursor()
        {
            // Arrange
            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, SortDirectionType.ASC, 2, PagingDirection.Backward, 50);
            var request = new GetStakingPositionsWithFilterQuery("PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK", cursor);

            var positions = new AddressStaking[]
            {
                new AddressStaking(5, 10, "PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u", UInt256.Parse("10000000000"), 500, 505),
                new AddressStaking(10, 10, "PAVV2c9Muk9Eu4wi8Fqdmm55ffzhAFPffV", UInt256.Parse("10000000000"), 500, 505)
            };
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveStakingPositionsWithFilterQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(positions);
            _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressStaking>())).ReturnsAsync(new StakingPositionDto());

            // Act
            var dto = await _handler.Handle(request, CancellationToken.None);

            // Assert
            AssertNext(dto.Cursor, positions[^1].Id);
            dto.Cursor.Previous.Should().Be(null);
        }

        private void AssertNext(CursorDto dto, long pointer)
        {
            StakingPositionsCursor.TryParse(dto.Next.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Forward);
            next.Pointer.Should().Be(pointer);
        }

        private void AssertPrevious(CursorDto dto, long pointer)
        {
            StakingPositionsCursor.TryParse(dto.Previous.Base64Decode(), out var next).Should().Be(true);
            next.PagingDirection.Should().Be(PagingDirection.Backward);
            next.Pointer.Should().Be(pointer);
        }
    }
}
