using FluentValidation.TestHelper;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using Opdex.Platform.WebApi.Models.Requests.Markets;
using Opdex.Platform.WebApi.Validation.Markets;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Markets;

public class MarketFilterParametersValidatorTests
{
    private readonly MarketFilterParametersValidator _validator;

    public MarketFilterParametersValidatorTests()
    {
        _validator = new MarketFilterParametersValidator();
    }

    [Fact]
    public void MarketType_Invalid()
    {
        // Arrange
        var request = new MarketFilterParameters
        {
            MarketType = (MarketType)100000000
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.MarketType);
    }

    [Theory]
    [InlineData(MarketType.All)]
    [InlineData(MarketType.Staking)]
    [InlineData(MarketType.Standard)]
    public void MarketType_Valid(MarketType marketType)
    {
        // Arrange
        var request = new MarketFilterParameters
        {
            MarketType = marketType
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.MarketType);
    }

    [Fact]
    public void OrderBy_Invalid()
    {
        // Arrange
        var request = new MarketFilterParameters
        {
            OrderBy = (MarketOrderByType)100000000
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.OrderBy);
    }

    [Theory]
    [InlineData(MarketOrderByType.Default)]
    [InlineData(MarketOrderByType.LiquidityUsd)]
    [InlineData(MarketOrderByType.StakingUsd)]
    [InlineData(MarketOrderByType.StakingWeight)]
    [InlineData(MarketOrderByType.VolumeUsd)]
    [InlineData(MarketOrderByType.MarketRewardsDailyUsd)]
    [InlineData(MarketOrderByType.ProviderRewardsDailyUsd)]
    [InlineData(MarketOrderByType.DailyLiquidityUsdChangePercent)]
    [InlineData(MarketOrderByType.DailyStakingUsdChangePercent)]
    [InlineData(MarketOrderByType.DailyStakingWeightChangePercent)]
    public void OrderBy_Valid(MarketOrderByType orderBy)
    {
        // Arrange
        var request = new MarketFilterParameters
        {
            OrderBy = orderBy
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.OrderBy);
    }

    [Fact]
    public void Limit_Invalid()
    {
        // Arrange
        var request = new MarketFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit + 1
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Limit);
    }

    [Fact]
    public void Limit_Valid()
    {
        // Arrange
        var request = new MarketFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Limit);
    }
}
