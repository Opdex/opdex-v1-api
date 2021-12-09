using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;
using Opdex.Platform.WebApi.Validation.LiquidityPools;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.LiquidityPools;

public class CalculateAddLiquidityAmountsRequestModelValidatorTests
{
    private readonly CalculateAddLiquidityAmountsRequestModelValidator _validator;

    public CalculateAddLiquidityAmountsRequestModelValidatorTests()
    {
        _validator = new CalculateAddLiquidityAmountsRequestModelValidator();
    }

    [Fact]
    public void TokenIn_Invalid()
    {
        // Arrange
        var request = new CalculateAddLiquidityAmountsRequestModel
        {
            TokenIn = Address.Empty
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.TokenIn);
    }

    [Theory]
    [ClassData(typeof(NonNetworkAddressData))]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void TokenIn_Valid(Address token)
    {
        // Arrange
        var request = new CalculateAddLiquidityAmountsRequestModel
        {
            TokenIn = token
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.TokenIn);
    }


    [Theory]
    [ClassData(typeof(InvalidSRCAmountData))]
    [ClassData(typeof(ZeroSRCAmountData))]
    public void AmountIn_Invalid(FixedDecimal amount)
    {
        // Arrange
        var request = new CalculateAddLiquidityAmountsRequestModel
        {
            AmountIn = amount
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.AmountIn);
    }

    [Fact]
    public void AmountIn_Valid()
    {
        // Arrange
        var request = new CalculateAddLiquidityAmountsRequestModel
        {
            AmountIn = FixedDecimal.Parse("0.000000000000000001")
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.AmountIn);
    }
}
