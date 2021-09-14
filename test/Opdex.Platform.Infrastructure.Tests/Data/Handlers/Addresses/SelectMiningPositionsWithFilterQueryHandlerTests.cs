using AutoMapper;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Mining;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Mining;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses
{
    public class SelectMiningPositionsWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMiningPositionsWithFilterQueryHandler _handler;

        public SelectMiningPositionsWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMiningPositionsWithFilterQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectMiningPositionsWithFilter_ByWallet()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new MiningPositionsCursor(Enumerable.Empty<Address>(), Enumerable.Empty<Address>(), false, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectMiningPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<AddressMiningEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("m.Owner = @Address"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningPositionsWithFilter_ByLiquidityPools()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var liquidityPools = new Address[] { "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj" };
            const uint limit = 10;

            var cursor = new MiningPositionsCursor(liquidityPools, Enumerable.Empty<Address>(), false, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectMiningPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressMiningEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN pool_liquidity pl") &&
                                                                q.Sql.Contains("pl.Address IN @LiquidityPools"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningPositionsWithFilter_ByMiningPools()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var miningPools = new Address[] { "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj" };
            const uint limit = 10;

            var cursor = new MiningPositionsCursor(Enumerable.Empty<Address>(), miningPools, false, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectMiningPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressMiningEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN pool_mining pm") &&
                                                                q.Sql.Contains("pm.Address IN @MiningPools"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningPositionsWithFilter_ExcludeZeroAmounts()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const bool includeZeroAmounts = false;
            const uint limit = 10;

            var cursor = new MiningPositionsCursor(Enumerable.Empty<Address>(), Enumerable.Empty<Address>(), includeZeroAmounts, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectMiningPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressMiningEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("m.Balance != '0'"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningPositionsWithFilter_ByCursor_NextASC()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new MiningPositionsCursor(Enumerable.Empty<Address>(), Enumerable.Empty<Address>(), false, direction, limit, PagingDirection.Forward, 50);

            var command = new SelectMiningPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressMiningEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("m.Id > @PositionId") &&
                                                                q.Sql.Contains($"ORDER BY m.Id {direction}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningPositionsWithFilter_ByCursor_NextDESC()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.DESC;
            const uint limit = 10;

            var cursor = new MiningPositionsCursor(Enumerable.Empty<Address>(), Enumerable.Empty<Address>(), false, direction, limit, PagingDirection.Forward, 50);

            var command = new SelectMiningPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressMiningEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("m.Id < @PositionId") &&
                                                                q.Sql.Contains($"ORDER BY m.Id {direction}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningPositionsWithFilter_ByCursor_PreviousDESC()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType requestDirection = SortDirectionType.DESC;
            const uint limit = 10;

            var cursor = new MiningPositionsCursor(Enumerable.Empty<Address>(), Enumerable.Empty<Address>(), false, requestDirection, limit, PagingDirection.Backward, 50);

            var command = new SelectMiningPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressMiningEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("m.Id > @PositionId") &&
                                                                q.Sql.Contains($"ORDER BY m.Id {SortDirectionType.ASC}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningPositionsWithFilter_ByCursor_PreviousASC()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType requestDirection = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new MiningPositionsCursor(Enumerable.Empty<Address>(), Enumerable.Empty<Address>(), false, requestDirection, limit, PagingDirection.Backward, 50);

            var command = new SelectMiningPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressMiningEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("m.Id < @PositionId") &&
                                                                q.Sql.Contains($"ORDER BY m.Id {SortDirectionType.DESC}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }
    }
}
