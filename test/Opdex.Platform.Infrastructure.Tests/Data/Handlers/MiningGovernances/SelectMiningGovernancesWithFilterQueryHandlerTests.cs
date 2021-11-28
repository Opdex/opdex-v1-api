using AutoMapper;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningGovernances
{
    public class SelectMiningGovernancesWithFilterQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMiningGovernancesWithFilterQueryHandler _handler;

        public SelectMiningGovernancesWithFilterQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMiningGovernancesWithFilterQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task Handle_Filter_ByMinedToken()
        {
            // Arrange
            var minedToken = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
            var cursor = new MiningGovernancesCursor(minedToken, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);

            // Act
            await _handler.Handle(new SelectMiningGovernancesWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<MiningGovernanceEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN token t ON t.Id = g.TokenId")
                                                         && q.Sql.Contains("t.Address = @MinedToken"))), Times.Once);
        }


        [Fact]
        public async Task SelectMiningGovernancesWithFilter_ByCursor_NextASC()
        {
            // Arrange
            var orderBy = SortDirectionType.ASC;
            var limit = 25U;
            var cursor = new MiningGovernancesCursor("", orderBy, limit, PagingDirection.Forward, 55);

            // Act
            await _handler.Handle(new SelectMiningGovernancesWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<MiningGovernanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("g.Id > @MiningGovernanceId") &&
                                                                q.Sql.Contains($"ORDER BY g.Id {orderBy}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningGovernancesWithFilter_ByCursor_NextDESC()
        {
            // Arrange
            var orderBy = SortDirectionType.DESC;
            var limit = 25U;
            var cursor = new MiningGovernancesCursor("", orderBy, limit, PagingDirection.Forward, 55);

            // Act
            await _handler.Handle(new SelectMiningGovernancesWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<MiningGovernanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("g.Id < @MiningGovernanceId") &&
                                                                q.Sql.Contains($"ORDER BY g.Id {orderBy}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningGovernancesWithFilter_ByCursor_PreviousDESC()
        {
            // Arrange
            var orderBy = SortDirectionType.DESC;
            var limit = 25U;
            var cursor = new MiningGovernancesCursor("", orderBy, limit, PagingDirection.Backward, 55);

            // Act
            await _handler.Handle(new SelectMiningGovernancesWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<MiningGovernanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("g.Id > @MiningGovernanceId") &&
                                                                q.Sql.Contains($"ORDER BY g.Id {SortDirectionType.ASC}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }

        [Fact]
        public async Task SelectMiningGovernancesWithFilter_ByCursor_PreviousASC()
        {
            // Arrange
            var orderBy = SortDirectionType.ASC;
            var limit = 25U;
            var cursor = new MiningGovernancesCursor("", orderBy, limit, PagingDirection.Backward, 55);

            // Act
            await _handler.Handle(new SelectMiningGovernancesWithFilterQuery(cursor), CancellationToken.None);

            // Assert
            _dbContext.Verify(callTo =>
                                  callTo.ExecuteQueryAsync<MiningGovernanceEntity>(
                                      It.Is<DatabaseQuery>(q => q.Sql.Contains("g.Id < @MiningGovernanceId") &&
                                                                q.Sql.Contains($"ORDER BY g.Id {SortDirectionType.DESC}") &&
                                                                q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
        }
    }
}
