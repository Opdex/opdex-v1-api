using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.LiquidityPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using Opdex.Platform.Infrastructure.Data.Handlers.LiquidityPools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.LiquidityPools
{
    public class SelectLiquidityPoolsWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectLiquidityPoolsWithFilterQueryHandler _handler;

        public SelectLiquidityPoolsWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectLiquidityPoolsWithFilterQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public void SelectLiquidityPoolsWithFilterQuery_InvalidCursor_ThrowArgumentNullException()
        {
            // Arrange

            // Act
            void Act() => new SelectLiquidityPoolsWithFilterQuery(null);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Liquidity pools cursor must be set.");
        }

        [Fact]
        public async Task Handle_Filter_Markets()
        {
            // Arrange
            var cursor = new LiquidityPoolsCursor(string.Empty,
                                                  new List<Address> { "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L" },
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Any,
                                                  SortDirectionType.ASC,
                                                  25,
                                                  PagingDirection.Forward,
                                                  default);

            const string expected = "WHERE m.Address IN @Markets";

            // Act
            await _handler.Handle(new SelectLiquidityPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<LiquidityPoolEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains(expected))), Times.Once);
        }

        [Fact]
        public async Task Handle_Filter_LiquidityPools()
        {
            // Arrange
            var cursor = new LiquidityPoolsCursor(string.Empty,
                                                  Enumerable.Empty<Address>(),
                                                  new List<Address> { "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L" },
                                                  Enumerable.Empty<Address>(),
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Any,
                                                  SortDirectionType.ASC,
                                                  25,
                                                  PagingDirection.Forward,
                                                  default);

            const string expected = "WHERE pl.Address IN @LiquidityPools";

            // Act
            await _handler.Handle(new SelectLiquidityPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<LiquidityPoolEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains(expected))), Times.Once);
        }

        [Fact]
        public async Task Handle_Filter_Tokens()
        {
            // Arrange
            var cursor = new LiquidityPoolsCursor(string.Empty,
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  new List<Address> { "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L" },
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Any,
                                                  SortDirectionType.ASC,
                                                  25,
                                                  PagingDirection.Forward,
                                                  default);

            const string expected = "WHERE t.Address IN @Tokens";

            // Act
            await _handler.Handle(new SelectLiquidityPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<LiquidityPoolEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains(expected))), Times.Once);
        }

        [Fact]
        public async Task Handle_Filter_Keyword()
        {
            // Arrange
            var cursor = new LiquidityPoolsCursor("BTC",
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Any,
                                                  SortDirectionType.ASC,
                                                  25,
                                                  PagingDirection.Forward,
                                                  default);

            const string name = "WHERE (pl.Name LIKE CONCAT('%', @Keyword, '%') OR";
            const string address = "pl.Address LIKE CONCAT('%', @Keyword, '%'))";

            // Act
            await _handler.Handle(new SelectLiquidityPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<LiquidityPoolEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains(name) &&
                                                                                                                q.Sql.Contains(address))), Times.Once);
        }

        [Theory]
        [InlineData(LiquidityPoolStakingStatusFilter.Enabled)]
        [InlineData(LiquidityPoolStakingStatusFilter.Disabled)]
        public async Task Handle_Filter_StakingFilter(LiquidityPoolStakingStatusFilter filter)
        {
            // Arrange
            var cursor = new LiquidityPoolsCursor(string.Empty,
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  filter,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Any,
                                                  SortDirectionType.ASC,
                                                  25,
                                                  PagingDirection.Forward,
                                                  default);

            string expected = filter == LiquidityPoolStakingStatusFilter.Enabled
                ? "WHERE (m.StakingTokenId > 0 AND pl.SrcTokenId != m.StakingTokenId)"
                : "WHERE (m.StakingTokenId = 0 OR (m.StakingTokenId > 0 AND pl.SrcTokenId = m.StakingTokenId))";

            // Act
            await _handler.Handle(new SelectLiquidityPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<LiquidityPoolEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains(expected))), Times.Once);
        }

        [Theory]
        [InlineData(LiquidityPoolMiningStatusFilter.Enabled)]
        [InlineData(LiquidityPoolMiningStatusFilter.Disabled)]
        public async Task Handle_Filter_MiningFilter(LiquidityPoolMiningStatusFilter filter)
        {
            // Arrange
            var cursor = new LiquidityPoolsCursor(string.Empty,
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  filter,
                                                  LiquidityPoolOrderByType.Any,
                                                  SortDirectionType.ASC,
                                                  25,
                                                  PagingDirection.Forward,
                                                  default);

            const string expectedJoin = "JOIN pool_mining pm on pl.Id = pm.LiquidityPoolId";
            var conditional = filter == LiquidityPoolMiningStatusFilter.Enabled ? ">=" : "<";
            string expectedFilter = $"WHERE (pm.MiningPeriodEndBlock {conditional} (SELECT Height FROM block ORDER BY Height DESC LIMIT 1))";

            // Act
            await _handler.Handle(new SelectLiquidityPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<LiquidityPoolEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains(expectedJoin) &&
                                                                                                                q.Sql.Contains(expectedFilter))), Times.Once);
        }

        [Theory]
        [InlineData(LiquidityPoolNominationStatusFilter.Nominated)]
        [InlineData(LiquidityPoolNominationStatusFilter.NonNominated)]
        public async Task Handle_Filter_NominationFilter(LiquidityPoolNominationStatusFilter filter)
        {
            // Arrange
            var cursor = new LiquidityPoolsCursor(string.Empty,
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  filter,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Any,
                                                  SortDirectionType.ASC,
                                                  25,
                                                  PagingDirection.Forward,
                                                  default);

            const string expectedJoin = " JOIN governance_nomination gn ON gn.LiquidityPoolId = pl.Id";
            var conditional = filter == LiquidityPoolNominationStatusFilter.Nominated ? "true" : "false";
            string expectedFilter = $"WHERE gn.IsNominated = {conditional}";

            // Act
            await _handler.Handle(new SelectLiquidityPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<LiquidityPoolEntity>(It.Is<DatabaseQuery>(q => q.Sql.Contains(expectedJoin) &&
                                                                                                                q.Sql.Contains(expectedFilter))), Times.Once);
        }

        [Fact]
        public async Task SelectLiquidityPoolsWithFilter_ByCursor_NextASC()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 25U;
            var cursor = new LiquidityPoolsCursor(string.Empty,
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Any,
                                                  direction,
                                                  limit,
                                                  PagingDirection.Forward,
                                                  (string.Empty, 10));
            // Act
            await _handler.Handle(new SelectLiquidityPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<LiquidityPoolEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("pl.Id > @LiquidityPoolId") &&
                                                                q.Sql.Contains($"ORDER BY pl.Id {direction}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectLiquidityPoolsWithFilter_ByCursor_NextDESC()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.DESC;
            const uint limit = 25U;
            var cursor = new LiquidityPoolsCursor(string.Empty,
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Name,
                                                  direction,
                                                  limit,
                                                  PagingDirection.Forward,
                                                  ("Bitcoin", 10));
            // Act
            await _handler.Handle(new SelectLiquidityPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<LiquidityPoolEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("(pl.Name, pl.Id) < (@OrderByValue, @LiquidityPoolId)") &&
                                                                q.Sql.Contains($"ORDER BY pl.Name {direction}, pl.Id {direction}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectLiquidityPoolsWithFilter_ByCursor_PreviousDESC()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.DESC;
            const uint limit = 25U;
            var cursor = new LiquidityPoolsCursor(string.Empty,
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Liquidity,
                                                  direction,
                                                  limit,
                                                  PagingDirection.Backward,
                                                  ("23.23", 10));

            // Act
            await _handler.Handle(new SelectLiquidityPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<LiquidityPoolEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("(pls.LiquidityUsd, pl.Id) > (@OrderByValue, @LiquidityPoolId)") &&
                                                                q.Sql.Contains("ORDER BY pls.LiquidityUsd ASC, pl.Id ASC") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectLiquidityPoolsWithFilter_ByCursor_PreviousASC()
        {
            // Arrange
            const SortDirectionType direction = SortDirectionType.ASC;
            const uint limit = 25U;
            var cursor = new LiquidityPoolsCursor(string.Empty,
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  Enumerable.Empty<Address>(),
                                                  LiquidityPoolStakingStatusFilter.Any,
                                                  LiquidityPoolNominationStatusFilter.Any,
                                                  LiquidityPoolMiningStatusFilter.Any,
                                                  LiquidityPoolOrderByType.Volume,
                                                  direction,
                                                  limit,
                                                  PagingDirection.Backward,
                                                  ("23.23", 10));
            // Act
            await _handler.Handle(new SelectLiquidityPoolsWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<LiquidityPoolEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("(pls.VolumeUsd, pl.Id) < (@OrderByValue, @LiquidityPoolId)") &&
                                                                q.Sql.Contains("ORDER BY pls.VolumeUsd DESC, pl.Id DESC") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }
    }
}
