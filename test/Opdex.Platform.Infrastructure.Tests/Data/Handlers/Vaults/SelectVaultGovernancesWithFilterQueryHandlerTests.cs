using AutoMapper;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Vaults;

public class SelectVaultGovernancesWithFilterQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultGovernancesWithFilterQueryHandler _handler;

    public SelectVaultGovernancesWithFilterQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultGovernancesWithFilterQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Filter_ByLockedToken()
    {
        // Arrange
        var lockedToken = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        var cursor = new VaultGovernancesCursor(lockedToken, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);

        // Act
        await _handler.Handle(new SelectVaultGovernancesWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<VaultGovernanceEntity>(
                              It.Is<DatabaseQuery>(q => q.Sql.Contains("JOIN token t ON t.Id = v.TokenId")
                                                        && q.Sql.Contains("t.Address = @LockedToken"))), Times.Once);
    }


    [Fact]
    public async Task SelectVaultsWithFilter_ByCursor_NextASC()
    {
        // Arrange
        var orderBy = SortDirectionType.ASC;
        var limit = 25U;
        var cursor = new VaultGovernancesCursor("", orderBy, limit, PagingDirection.Forward, 55);

        // Act
        await _handler.Handle(new SelectVaultGovernancesWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo =>
                              callTo.ExecuteQueryAsync<VaultGovernanceEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("v.Id > @Pointer") &&
                                                            q.Sql.Contains($"ORDER BY v.Id {orderBy}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
    }

    [Fact]
    public async Task SelectVaultsWithFilter_ByCursor_NextDESC()
    {
        // Arrange
        var orderBy = SortDirectionType.DESC;
        var limit = 25U;
        var cursor = new VaultGovernancesCursor("", orderBy, limit, PagingDirection.Forward, 55);

        // Act
        await _handler.Handle(new SelectVaultGovernancesWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo =>
                              callTo.ExecuteQueryAsync<VaultGovernanceEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("v.Id < @Pointer") &&
                                                            q.Sql.Contains($"ORDER BY v.Id {orderBy}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
    }

    [Fact]
    public async Task SelectVaultsWithFilter_ByCursor_PreviousDESC()
    {
        // Arrange
        var orderBy = SortDirectionType.DESC;
        var limit = 25U;
        var cursor = new VaultGovernancesCursor("", orderBy, limit, PagingDirection.Backward, 55);

        // Act
        await _handler.Handle(new SelectVaultGovernancesWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo =>
                              callTo.ExecuteQueryAsync<VaultGovernanceEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("v.Id > @Pointer") &&
                                                            q.Sql.Contains($"ORDER BY v.Id {SortDirectionType.ASC}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
    }

    [Fact]
    public async Task SelectVaultsWithFilter_ByCursor_PreviousASC()
    {
        // Arrange
        var orderBy = SortDirectionType.ASC;
        var limit = 25U;
        var cursor = new VaultGovernancesCursor("", orderBy, limit, PagingDirection.Backward, 55);

        // Act
        await _handler.Handle(new SelectVaultGovernancesWithFilterQuery(cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo =>
                              callTo.ExecuteQueryAsync<VaultGovernanceEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("v.Id < @Pointer") &&
                                                            q.Sql.Contains($"ORDER BY v.Id {SortDirectionType.DESC}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
    }
}
