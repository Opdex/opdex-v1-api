using AutoMapper;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using Opdex.Platform.Infrastructure.Data.Handlers.Vaults.Certificates;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Vaults.Certificates;

public class SelectVaultGovernanceCertificatesWithFilterQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectVaultGovernanceCertificatesWithFilterQueryHandler _handler;

    public SelectVaultGovernanceCertificatesWithFilterQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectVaultGovernanceCertificatesWithFilterQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Handle_Filter_ByHolder()
    {
        // Arrange
        var holder = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj";
        var cursor = new VaultGovernanceCertificatesCursor(holder, VaultCertificateStatusFilter.Redeemed, SortDirectionType.ASC, 25, PagingDirection.Backward, 55);

        // Act
        await _handler.Handle(new SelectVaultGovernanceCertificatesWithFilterQuery(5, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<VaultCertificateEntity>(
                              It.Is<DatabaseQuery>(q => q.Sql.Contains("c.Owner = @Holder"))), Times.Once);
    }


    [Fact]
    public async Task SelectVaultCertificatesWithFilter_ByCursor_NextASC()
    {
        // Arrange
        var orderBy = SortDirectionType.ASC;
        var limit = 25U;
        var cursor = new VaultGovernanceCertificatesCursor("", VaultCertificateStatusFilter.Revoked, orderBy, limit, PagingDirection.Forward, 55);

        // Act
        await _handler.Handle(new SelectVaultGovernanceCertificatesWithFilterQuery(5, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo =>
                              callTo.ExecuteQueryAsync<VaultCertificateEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("c.Id > @IdPointer") &&
                                                            q.Sql.Contains($"ORDER BY c.Id {orderBy}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
    }

    [Fact]
    public async Task SelectVaultCertificatesWithFilter_ByCursor_NextDESC()
    {
        // Arrange
        var orderBy = SortDirectionType.DESC;
        var limit = 25U;
        var cursor = new VaultGovernanceCertificatesCursor("", VaultCertificateStatusFilter.Redeemed, orderBy, limit, PagingDirection.Forward, 55);

        // Act
        await _handler.Handle(new SelectVaultGovernanceCertificatesWithFilterQuery(5, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo =>
                              callTo.ExecuteQueryAsync<VaultCertificateEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("c.Id < @IdPointer") &&
                                                            q.Sql.Contains($"ORDER BY c.Id {orderBy}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
    }

    [Fact]
    public async Task SelectVaultCertificatesWithFilter_ByCursor_PreviousDESC()
    {
        // Arrange
        var orderBy = SortDirectionType.DESC;
        var limit = 25U;
        var cursor = new VaultGovernanceCertificatesCursor("", VaultCertificateStatusFilter.Redeemed, orderBy, limit, PagingDirection.Backward, 55);

        // Act
        await _handler.Handle(new SelectVaultGovernanceCertificatesWithFilterQuery(5, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo =>
                              callTo.ExecuteQueryAsync<VaultCertificateEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("c.Id > @IdPointer") &&
                                                            q.Sql.Contains($"ORDER BY c.Id {SortDirectionType.ASC}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
    }

    [Fact]
    public async Task SelectVaultCertificatesWithFilter_ByCursor_PreviousASC()
    {
        // Arrange
        var orderBy = SortDirectionType.ASC;
        var limit = 25U;
        var cursor = new VaultGovernanceCertificatesCursor("", VaultCertificateStatusFilter.Redeemed, orderBy, limit, PagingDirection.Backward, 55);

        // Act
        await _handler.Handle(new SelectVaultGovernanceCertificatesWithFilterQuery(5, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo =>
                              callTo.ExecuteQueryAsync<VaultCertificateEntity>(
                                  It.Is<DatabaseQuery>(q => q.Sql.Contains("c.Id < @IdPointer") &&
                                                            q.Sql.Contains($"ORDER BY c.Id {SortDirectionType.DESC}") &&
                                                            q.Sql.Contains($"LIMIT {limit + 1}"))), Times.Once);
    }
}