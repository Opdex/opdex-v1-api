using FluentValidation.TestHelper;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.Tokens;
using Opdex.Platform.WebApi.Validation.Tokens;
using System.Collections.Generic;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Tokens
{
    public class TokenFilterParametersValidatorTests
    {
        private readonly TokenFilterParametersValidator _validator;

        public TokenFilterParametersValidatorTests()
        {
            _validator = new TokenFilterParametersValidator();
        }

        [Fact]
        public void OrderBy_Invalid()
        {
            // Arrange
            var request = new TokenFilterParameters
            {
                OrderBy = 0
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
        [InlineData(default(TokenAttributeType))]
        [InlineData((TokenAttributeType)1000)]
        public void Attributes_Items_Invalid(TokenAttributeType eventType)
        {
            // Arrange
            var request = new TokenFilterParameters
            {
                Attributes = new List<TokenAttributeType> { eventType }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Attributes);
        }

        [Fact]
        public void Attributes_Items_Valid()
        {
            // Arrange
            var request = new TokenFilterParameters
            {
                Attributes = new List<TokenAttributeType> { TokenAttributeType.SRC20 }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.Attributes);
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
}
