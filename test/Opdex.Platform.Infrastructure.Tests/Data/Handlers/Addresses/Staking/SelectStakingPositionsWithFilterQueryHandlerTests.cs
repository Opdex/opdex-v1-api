using AutoMapper;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Addresses.Staking;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Staking;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses.Staking
{
    public class SelectStakingPositionsWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectStakingPositionsWithFilterQueryHandler _handler;

        public SelectStakingPositionsWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectStakingPositionsWithFilterQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectStakingPositionsWithFilter_ByWallet()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectStakingPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<AddressStakingEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("s.Owner = @Address"))), Times.Once);
        }

        [Fact]
        public async Task SelectStakingPositionsWithFilter_ByLiquidityPools()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var liquidityPools = new Address[] { "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj" };
            const uint limit = 10;

            var cursor = new StakingPositionsCursor(liquidityPools, false, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectStakingPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressStakingEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN pool_liquidity pl") &&
                                                                q.Sql.Contains("pl.Address IN @LiquidityPools"))), Times.Once);
        }

        [Fact]
        public async Task SelectStakingPositionsWithFilter_ExcludeZeroAmounts()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const bool includeZeroAmounts = false;
            const uint limit = 10;

            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), includeZeroAmounts, direction, limit, PagingDirection.Backward, 50);

            var command = new SelectStakingPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressStakingEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("s.Weight != '0'"))), Times.Once);
        }

        [Fact]
        public async Task SelectStakingPositionsWithFilter_ByCursor_NextASC()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, direction, limit, PagingDirection.Forward, 50);

            var command = new SelectStakingPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressStakingEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("s.Id > @PositionId") &&
                                                                q.Sql.Contains($"ORDER BY s.Id {direction}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectStakingPositionsWithFilter_ByCursor_NextDESC()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType direction = SortDirectionType.DESC;
            const uint limit = 10;

            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, direction, limit, PagingDirection.Forward, 50);

            var command = new SelectStakingPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressStakingEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("s.Id < @PositionId") &&
                                                                q.Sql.Contains($"ORDER BY s.Id {direction}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectStakingPositionsWithFilter_ByCursor_PreviousDESC()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType requestDirection = SortDirectionType.DESC;
            const uint limit = 10;

            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, requestDirection, limit, PagingDirection.Backward, 50);

            var command = new SelectStakingPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressStakingEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("s.Id > @PositionId") &&
                                                                q.Sql.Contains($"ORDER BY s.Id {SortDirectionType.ASC}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectStakingPositionsWithFilter_ByCursor_PreviousASC()
        {
            // Arrange
            Address wallet = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            const SortDirectionType requestDirection = SortDirectionType.ASC;
            const uint limit = 10;

            var cursor = new StakingPositionsCursor(Enumerable.Empty<Address>(), false, requestDirection, limit, PagingDirection.Backward, 50);

            var command = new SelectStakingPositionsWithFilterQuery(wallet, cursor);

            // Act
            await _handler.Handle(command, CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<AddressStakingEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("s.Id < @PositionId") &&
                                                                q.Sql.Contains($"ORDER BY s.Id {SortDirectionType.DESC}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }
    }
}
