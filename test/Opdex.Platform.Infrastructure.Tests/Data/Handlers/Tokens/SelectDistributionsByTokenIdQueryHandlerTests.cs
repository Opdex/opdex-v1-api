using AutoMapper;
using Moq;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens;

public class SelectDistributionsByTokenIdQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectDistributionsByTokenIdQueryHandler _handler;

    public SelectDistributionsByTokenIdQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectDistributionsByTokenIdQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public async Task Select_TokenDistributions_CorrectTable()
    {
        // Arrange
        var command = new SelectDistributionsByTokenIdQuery(5);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<TokenDistributionEntity>(
            It.Is<DatabaseQuery>(q => q.Sql.Contains("FROM token_distribution"))), Times.Once);
    }

    [Fact]
    public async Task Select_TokenDistributions_FilterByTokenId()
    {
        // Arrange
        var command = new SelectDistributionsByTokenIdQuery(5);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<TokenDistributionEntity>(
            It.Is<DatabaseQuery>(q => q.Sql.Contains($"TokenId = @TokenId"))), Times.Once);
    }

    [Fact]
    public async Task Select_TokenDistributions_OrderAsc()
    {
        // Arrange
        var command = new SelectDistributionsByTokenIdQuery(5);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync<TokenDistributionEntity>(
            It.Is<DatabaseQuery>(q => q.Sql.Contains($"ORDER BY NextDistributionBlock"))), Times.Once);
    }

}
