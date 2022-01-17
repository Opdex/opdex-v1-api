using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;
using Opdex.Platform.WebApi.Models.Requests.Tokens;
using Opdex.Platform.WebApi.Validation.Tokens;
using System.Collections.Generic;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Tokens;

public class TokenFilterParametersValidatorTests
{
    private readonly TokenFilterParametersValidator _validator;

    public TokenFilterParametersValidatorTests()
    {
        _validator = new TokenFilterParametersValidator();
    }

    [Theory]
    [InlineData("*")]
    [InlineData("?")]
    [InlineData(":")]
    [InlineData("asdf;")]
    public void Keyword_Invalid(string keyword)
    {
        // Arrange
        var request = new TokenFilterParameters
        {
            Keyword = keyword
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Keyword).WithErrorMessage("Keyword must consist of letters, numbers and spaces only.");
    }

    [Theory]
    [InlineData("asdf")]
    [InlineData("fda")]
    [InlineData("89df7g78eh5qehgn8943hg3")]
    [InlineData("tVfGTqrToiTU9bfnvD5UDC5ZQVY4oj4jrc")]
    [InlineData("Bitcoin Wrapped")]
    public void Keyword_Valid(string keyword)
    {
        // Arrange
        var request = new TokenFilterParameters
        {
            Keyword = keyword
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Keyword);
    }

    [Fact]
    public void OrderBy_Invalid()
    {
        // Arrange
        var request = new TokenFilterParameters
        {
            OrderBy = (TokenOrderByType)10000
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.OrderBy);
    }

    [Theory]
    [InlineData(TokenOrderByType.PriceUsd)]
    [InlineData(TokenOrderByType.DailyPriceChangePercent)]
    public void OrderBy_Valid(TokenOrderByType orderBy)
    {
        // Arrange
        var request = new TokenFilterParameters
        {
            OrderBy = orderBy
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.OrderBy);
    }

    [Theory]
    [ClassData(typeof(NullAddressData))]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Tokens_Items_Invalid(Address address)
    {
        // Arrange
        var request = new TokenFilterParameters
        {
            Tokens = new List<Address> { address }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Tokens);
    }

    [Fact]
    public void Tokens_Items_Valid()
    {
        // Arrange
        var request = new TokenFilterParameters
        {
            Tokens = new List<Address> { new Address("tVfGTqrToiTU9bfnvD5UDC5ZQVY4oj4jrc") }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Tokens);
    }

    [Theory]
    [InlineData((TokenAttributeFilter)1000)]
    public void AttributeFilter_Items_Invalid(TokenAttributeFilter filter)
    {
        // Arrange
        var request = new TokenFilterParameters
        {
            TokenAttributes = new List<TokenAttributeFilter> {filter}
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.TokenAttributes);
    }

    [Theory]
    [InlineData(TokenAttributeFilter.Provisional, TokenAttributeFilter.NonProvisional)]
    [InlineData(TokenAttributeFilter.NonProvisional, TokenAttributeFilter.NonProvisional)]
    [InlineData(TokenAttributeFilter.Staking, TokenAttributeFilter.NonProvisional)]
    public void AttributeFilter_Items_Valid(TokenAttributeFilter first, TokenAttributeFilter second)
    {
        // Arrange
        var request = new TokenFilterParameters
        {
            TokenAttributes = new List<TokenAttributeFilter> {first, second}
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.TokenAttributes);
    }

    [Fact]
    public void Limit_Invalid()
    {
        // Arrange
        var request = new TokenFilterParameters
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
        var request = new TokenFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Limit);
    }
}
