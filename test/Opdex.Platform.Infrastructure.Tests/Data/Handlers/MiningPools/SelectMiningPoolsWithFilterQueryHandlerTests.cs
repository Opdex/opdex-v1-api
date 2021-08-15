using AutoMapper;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningPools;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningPools
{
    public class SelectMiningPoolsWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMiningPoolsWithFilterQueryHandler _handler;

        public SelectMiningPoolsWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMiningPoolsWithFilterQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task Handle_DoNotFilter_ByLiquidityPools()
        {
            // Arrange
            var liquidityPools = Enumerable.Empty<Address>();
            var cursor = new MiningPoolsCursor(liquidityPools, MiningStatusFilter.Any, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);

            // Act
            await _handler.Handle(new SelectMiningPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<MiningPoolEntity>(
                                  It.Is<DatabaseQuery>(q => !q.Sql.Contains("JOIN pool_liquidity pl ON pl.Id = pm.LiquidityPoolId")
                                                         && !q.Sql.Contains("pl.Address IN @LiquidityPools"))), Times.Once);
        }

        [Fact]
        public async Task Handle_Filter_ByLiquidityPools()
        {
            // Arrange
            var liquidityPools = new Address[] { "PKHsZBS9Mbt4fE4W2T1U8wgTKSazNjhvYs" };
            var cursor = new MiningPoolsCursor(liquidityPools, MiningStatusFilter.Any, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);

            // Act
            await _handler.Handle(new SelectMiningPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<MiningPoolEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN pool_liquidity pl ON pl.Id = pm.LiquidityPoolId")
                                                         && q.Sql.Contains("pl.Address IN @LiquidityPools"))), Times.Once);
        }

        [Fact]
        public async Task Handle_Filter_MiningStatusAny()
        {
            // Arrange
            var miningStatus = MiningStatusFilter.Any;
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), miningStatus, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);

            // Act
            await _handler.Handle(new SelectMiningPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<MiningPoolEntity>(
                                  It.Is<DatabaseQuery>(q => !q.Sql.Contains("SELECT MAX(b.Height) FROM block b"))), Times.Once);
        }

        [Fact]
        public async Task Handle_Filter_MiningStatusActive()
        {
            // Arrange
            var miningStatus = MiningStatusFilter.Active;
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), miningStatus, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);

            // Act
            await _handler.Handle(new SelectMiningPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<MiningPoolEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("(SELECT MAX(b.Height) FROM block b) < pm.MiningPeriodEndBlock"))), Times.Once);
        }

        [Fact]
        public async Task Handle_Filter_MiningStatusInactive()
        {
            // Arrange
            var miningStatus = MiningStatusFilter.Inactive;
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), miningStatus, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);

            // Act
            await _handler.Handle(new SelectMiningPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<MiningPoolEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("(SELECT MAX(b.Height) FROM block b) >= pm.MiningPeriodEndBlock"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningPoolsWithFilter_ByCursor_NextASC()
        {
            // Arrange
            var orderBy = SortDirectionType.ASC;
            var limit = 25U;
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Any, orderBy, limit, PagingDirection.Forward, 55);

            // Act
            await _handler.Handle(new SelectMiningPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<MiningPoolEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("pm.Id > @MiningPoolId") &&
                                                                q.Sql.Contains($"ORDER BY pm.Id {orderBy}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningPoolsWithFilter_ByCursor_NextDESC()
        {
            // Arrange
            var orderBy = SortDirectionType.DESC;
            var limit = 25U;
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Any, orderBy, limit, PagingDirection.Forward, 55);

            // Act
            await _handler.Handle(new SelectMiningPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<MiningPoolEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("pm.Id < @MiningPoolId") &&
                                                                q.Sql.Contains($"ORDER BY pm.Id {orderBy}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningPoolsWithFilter_ByCursor_PreviousDESC()
        {
            // Arrange
            var orderBy = SortDirectionType.DESC;
            var limit = 25U;
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Any, orderBy, limit, PagingDirection.Backward, 55);

            // Act
            await _handler.Handle(new SelectMiningPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<MiningPoolEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("pm.Id > @MiningPoolId") &&
                                                                q.Sql.Contains($"ORDER BY pm.Id {SortDirectionType.ASC}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningPoolsWithFilter_ByCursor_PreviousASC()
        {
            // Arrange
            var orderBy = SortDirectionType.ASC;
            var limit = 25U;
            var cursor = new MiningPoolsCursor(Enumerable.Empty<Address>(), MiningStatusFilter.Any, orderBy, limit, PagingDirection.Backward, 55);

            // Act
            await _handler.Handle(new SelectMiningPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<MiningPoolEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("pm.Id < @MiningPoolId") &&
                                                                q.Sql.Contains($"ORDER BY pm.Id {SortDirectionType.DESC}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }
    }
}
