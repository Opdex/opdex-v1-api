using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens;

public class SelectTokensWithFilterQueryHandlerTests
{
    private readonly Mock<IDbContext> _dbContext;
    private readonly SelectTokensWithFilterQueryHandler _handler;

    public SelectTokensWithFilterQueryHandlerTests()
    {
        var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

        _dbContext = new Mock<IDbContext>();
        _handler = new SelectTokensWithFilterQueryHandler(_dbContext.Object, mapper);
    }

    [Fact]
    public void SelectTokensWithFilter_InvalidCursor_ThrowArgumentNullException()
    {
        // Arrange

        // Act
        void Act() => new SelectTokensWithFilterQuery(0, null);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Tokens cursor must be provided.");
    }

    [Fact]
    public async Task Handle_Filter_MarketId()
    {
        // Arrange
        ulong marketId = 1;
        var cursor = new TokensCursor(null, Enumerable.Empty<Address>(),
                                      Enumerable.Empty<TokenAttributeFilter>(), Enumerable.Empty<ChainType>(),
                                      false, TokenOrderByType.Default, SortDirectionType.ASC,
                                      5, PagingDirection.Forward, default);
        string expected = "WHERE ts.MarketId = @MarketId";

        // Act
        await _handler.Handle(new SelectTokensWithFilterQuery(marketId, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync(
                It.Is<DatabaseQuery>(q => q.Sql.Contains(expected)),
                It.IsAny<Func<TokenEntity, TokenSummaryEntity, Token>>(),
                "Split"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Filter_Tokens()
    {
        // Arrange
        var tokens = new Address[] { "PKHsZBS9Mbt4fE4W2T1U8wgTKSazNjhvYs" };
        var cursor = new TokensCursor(null, tokens,
            Enumerable.Empty<TokenAttributeFilter>(), Enumerable.Empty<ChainType>(),
            false, TokenOrderByType.Default, SortDirectionType.ASC,
            5, PagingDirection.Forward, default);

        var expected = "t.Address IN @Tokens";

        // Act
        await _handler.Handle(new SelectTokensWithFilterQuery(0, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync(
                It.Is<DatabaseQuery>(q => q.Sql.Contains(expected)),
                It.IsAny<Func<TokenEntity, TokenSummaryEntity, Token>>(),
                "Split"),
            Times.Once);
    }

    [Theory]
    [InlineData(TokenAttributeFilter.NonProvisional, TokenAttributeFilter.NonProvisional)]
    [InlineData(TokenAttributeFilter.Provisional, TokenAttributeFilter.NonProvisional)]
    [InlineData(TokenAttributeFilter.Staking, TokenAttributeFilter.NonProvisional)]
    public async Task Handle_ShouldFilter_TokenAttribute(TokenAttributeFilter firstFilter, TokenAttributeFilter secondFilter)
    {
        // Arrange
        var tokenAttributes = new List<TokenAttributeFilter> { firstFilter, secondFilter };
        var cursor = new TokensCursor(null, Enumerable.Empty<Address>(),
            tokenAttributes, Enumerable.Empty<ChainType>(),
            false, TokenOrderByType.Default, SortDirectionType.ASC,
            5, PagingDirection.Forward, default);

        const string expectedJoin = "LEFT JOIN token_attribute ta ON ta.TokenId = t.Id";
        const string expectedWhere = "ta.AttributeTypeId IN @TokenAttributes";

        // Act
        await _handler.Handle(new SelectTokensWithFilterQuery(0, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync(
                It.Is<DatabaseQuery>(q => q.Sql.Contains(expectedJoin) && q.Sql.Contains(expectedWhere)),
                It.IsAny<Func<TokenEntity, TokenSummaryEntity, Token>>(),
                "Split"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFilter_Keyword()
    {
        // Arrange
        var cursor = new TokensCursor("BTC", Enumerable.Empty<Address>(),
            Enumerable.Empty<TokenAttributeFilter>(), Enumerable.Empty<ChainType>(),
            false, TokenOrderByType.Default, SortDirectionType.ASC,
            5, PagingDirection.Forward, default);

        const string name = "(t.Name LIKE CONCAT('%', @Keyword, '%') OR";
        const string symbol = "t.Symbol LIKE CONCAT('%', @Keyword, '%') OR";
        const string address = "t.Address LIKE CONCAT('%', @Keyword, '%'))";

        // Act
        await _handler.Handle(new SelectTokensWithFilterQuery(0, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync(
                It.Is<DatabaseQuery>(q => q.Sql.Contains(name) && q.Sql.Contains(symbol) &&  q.Sql.Contains(address)),
                It.IsAny<Func<TokenEntity, TokenSummaryEntity, Token>>(),
                "Split"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldNotFilter_NoLiquidity()
    {
        // Arrange
        var cursor = new TokensCursor("BTC", Enumerable.Empty<Address>(),
            Enumerable.Empty<TokenAttributeFilter>(), Enumerable.Empty<ChainType>(),
            true, TokenOrderByType.Default, SortDirectionType.ASC,
            5, PagingDirection.Forward, default);

        // Act
        await _handler.Handle(new SelectTokensWithFilterQuery(0, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync(
            It.Is<DatabaseQuery>( q => !q.Sql.Contains("ts.PriceUsd > 0")),
                It.IsAny<Func<TokenEntity, TokenSummaryEntity, Token>>(),
                "Split"),
            Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFilter_NoLiquidity()
    {
        // Arrange
        var cursor = new TokensCursor("BTC", Enumerable.Empty<Address>(),
            Enumerable.Empty<TokenAttributeFilter>(), Enumerable.Empty<ChainType>(),
            false, TokenOrderByType.Default, SortDirectionType.ASC,
            5, PagingDirection.Forward, default);

        // Act
        await _handler.Handle(new SelectTokensWithFilterQuery(0, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync(
            It.Is<DatabaseQuery>(q => q.Sql.Contains("ts.PriceUsd > 0")),
                It.IsAny<Func<TokenEntity, TokenSummaryEntity, Token>>(),
                "Split"),
            Times.Once);
    }

    [Fact]
    public async Task SelectTokensWithFilter_ByCursor_NextASC()
    {
        // Arrange
        const SortDirectionType orderBy = SortDirectionType.ASC;
        const uint limit = 25U;
        var cursor = new TokensCursor(null, Enumerable.Empty<Address>(),
            Enumerable.Empty<TokenAttributeFilter>(), Enumerable.Empty<ChainType>(),
            false, TokenOrderByType.Default, orderBy, limit, PagingDirection.Forward, (null, 10));
        // Act
        await _handler.Handle(new SelectTokensWithFilterQuery(0, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync(
                It.Is<DatabaseQuery>(q => q.Sql.Contains("t.Id > @TokenId") &&
                                          q.Sql.Contains($"ORDER BY t.Id {orderBy}") &&
                                          q.Sql.Contains($"LIMIT {limit + 1}")),
                It.IsAny<Func<TokenEntity, TokenSummaryEntity, Token>>(),
                "Split"),
            Times.Once);
    }

    [Fact]
    public async Task SelectTokensWithFilter_ByCursor_NextDESC()
    {
        // Arrange
        const SortDirectionType orderBy = SortDirectionType.DESC;
        const uint limit = 25U;
        var cursor = new TokensCursor(null, Enumerable.Empty<Address>(),
            Enumerable.Empty<TokenAttributeFilter>(), Enumerable.Empty<ChainType>(),
            false, TokenOrderByType.Name, orderBy, limit, PagingDirection.Forward, ("Bitcoin", 10));

        // Act
        await _handler.Handle(new SelectTokensWithFilterQuery(0, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync(
                It.Is<DatabaseQuery>(q => q.Sql.Contains("(t.Name, t.Id) < (@OrderByValue, @TokenId)") &&
                                          q.Sql.Contains($"ORDER BY t.Name {orderBy}, t.Id {orderBy}") &&
                                          q.Sql.Contains($"LIMIT {limit + 1}")),
                It.IsAny<Func<TokenEntity, TokenSummaryEntity, Token>>(),
                "Split"),
            Times.Once);
    }

    [Fact]
    public async Task SelectTokensWithFilter_ByCursor_PreviousDESC()
    {
        // Arrange
        const SortDirectionType orderBy = SortDirectionType.DESC;
        const uint limit = 25U;
        var cursor = new TokensCursor(null, Enumerable.Empty<Address>(),
            Enumerable.Empty<TokenAttributeFilter>(), Enumerable.Empty<ChainType>(),
            false, TokenOrderByType.Symbol, orderBy, limit, PagingDirection.Backward, ("BTC", 10));
        // Act
        await _handler.Handle(new SelectTokensWithFilterQuery(0, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync(
                It.Is<DatabaseQuery>(q => q.Sql.Contains("(t.Symbol, t.Id) > (@OrderByValue, @TokenId)") &&
                                          q.Sql.Contains($"ORDER BY t.Symbol {SortDirectionType.ASC}, t.Id {SortDirectionType.ASC}") &&
                                          q.Sql.Contains($"LIMIT {limit + 1}")),
                It.IsAny<Func<TokenEntity, TokenSummaryEntity, Token>>(),
                "Split"),
            Times.Once);
    }

    [Fact]
    public async Task SelectTokensWithFilter_ByCursor_PreviousASC()
    {
        // Arrange
        const SortDirectionType orderBy = SortDirectionType.ASC;
        const uint limit = 25U;
        var cursor = new TokensCursor(null, Enumerable.Empty<Address>(),
            Enumerable.Empty<TokenAttributeFilter>(), Enumerable.Empty<ChainType>(),
            false, TokenOrderByType.PriceUsd, orderBy, limit, PagingDirection.Backward, ("10.1", 10));
        // Act
        await _handler.Handle(new SelectTokensWithFilterQuery(0, cursor), CancellationToken.None);

        // Assert
        _dbContext.Verify(callTo => callTo.ExecuteQueryAsync(
                It.Is<DatabaseQuery>(q =>  q.Sql.Contains("(ts.PriceUsd, t.Id) < (@OrderByValue, @TokenId)") &&
                                               q.Sql.Contains($"ORDER BY AVG(ts.PriceUsd) {SortDirectionType.DESC}, t.Id {SortDirectionType.DESC}") &&
                                               q.Sql.Contains($"LIMIT {limit + 1}")),
                It.IsAny<Func<TokenEntity, TokenSummaryEntity, Token>>(),
                "Split"),
            Times.Once);
    }
}
