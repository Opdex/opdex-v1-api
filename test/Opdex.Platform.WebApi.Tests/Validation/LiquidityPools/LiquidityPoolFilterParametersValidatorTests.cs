using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.LiquidityPools;
using Opdex.Platform.WebApi.Models.Requests.LiquidityPools;
using Opdex.Platform.WebApi.Validation.LiquidityPools;
using System.Collections.Generic;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.LiquidityPools
{
    public class LiquidityPoolFilterParametersValidatorTests
    {
        private readonly LiquidityPoolFilterParametersValidator _validator;

        public LiquidityPoolFilterParametersValidatorTests()
        {
            _validator = new LiquidityPoolFilterParametersValidator();
        }

        [Theory]
        [InlineData("*")]
        [InlineData("?")]
        [InlineData(":")]
        [InlineData("asdf;")]
        public void Keyword_Invalid(string keyword)
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
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
            var request = new LiquidityPoolFilterParameters
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
            var request = new LiquidityPoolFilterParameters
            {
                OrderBy = (LiquidityPoolOrderByType)10000
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.OrderBy);
        }

        [Theory]
        [InlineData(LiquidityPoolOrderByType.Liquidity)]
        [InlineData(LiquidityPoolOrderByType.Volume)]
        [InlineData(LiquidityPoolOrderByType.StakingWeight)]
        [InlineData(LiquidityPoolOrderByType.Name)]
        public void OrderBy_Valid(LiquidityPoolOrderByType orderBy)
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
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
        public void LiquidityPools_LiquidityPools_Invalid(Address address)
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
            {
                LiquidityPools = new List<Address> { address }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.LiquidityPools);
        }

        [Fact]
        public void LiquidityPools_LiquidityPools_Valid()
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
            {
                LiquidityPools = new List<Address> { new Address("tVfGTqrToiTU9bfnvD5UDC5ZQVY4oj4jrc") }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.LiquidityPools);
        }

        [Theory]
        [ClassData(typeof(NullAddressData))]
        [ClassData(typeof(EmptyAddressData))]
        [ClassData(typeof(NonNetworkAddressData))]
        public void LiquidityPools_Tokens_Invalid(Address address)
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
            {
                Tokens = new List<Address> { address }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Tokens);
        }

        [Fact]
        public void LiquidityPools_Tokens_Valid()
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
            {
                Tokens = new List<Address> { new Address("tVfGTqrToiTU9bfnvD5UDC5ZQVY4oj4jrc") }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.Tokens);
        }

        [Theory]
        [ClassData(typeof(NullAddressData))]
        [ClassData(typeof(EmptyAddressData))]
        [ClassData(typeof(NonNetworkAddressData))]
        public void LiquidityPools_Markets_Invalid(Address address)
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
            {
                Markets = new List<Address> { address }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.Markets);
        }

        [Fact]
        public void LiquidityPools_Markets_Valid()
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
            {
                Markets = new List<Address> { new Address("tVfGTqrToiTU9bfnvD5UDC5ZQVY4oj4jrc") }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.Markets);
        }

        [Theory]
        [InlineData((LiquidityPoolStakingStatusFilter)1000)]
        public void ProvisionalFilter_StakingFilter_Invalid(LiquidityPoolStakingStatusFilter filter)
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
            {
                StakingFilter = filter
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.StakingFilter);
        }

        [Theory]
        [InlineData(LiquidityPoolStakingStatusFilter.Any)]
        [InlineData(LiquidityPoolStakingStatusFilter.Enabled)]
        [InlineData(LiquidityPoolStakingStatusFilter.Disabled)]
        public void ProvisionalFilter_StakingFilterValid(LiquidityPoolStakingStatusFilter filter)
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
            {
                StakingFilter = filter
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.StakingFilter);
        }

        [Theory]
        [InlineData((LiquidityPoolNominationStatusFilter)1000)]
        public void ProvisionalFilter_NominationFilter_Invalid(LiquidityPoolNominationStatusFilter filter)
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
            {
                NominationFilter = filter
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.NominationFilter);
        }

        [Theory]
        [InlineData(LiquidityPoolNominationStatusFilter.Any)]
        [InlineData(LiquidityPoolNominationStatusFilter.Nominated)]
        [InlineData(LiquidityPoolNominationStatusFilter.NonNominated)]
        public void ProvisionalFilter_NominationFilterValid(LiquidityPoolNominationStatusFilter filter)
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
            {
                NominationFilter = filter
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.NominationFilter);
        }

        [Theory]
        [InlineData((LiquidityPoolMiningStatusFilter)1000)]
        public void ProvisionalFilter_MiningFilter_Invalid(LiquidityPoolMiningStatusFilter filter)
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
            {
                MiningFilter = filter
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(r => r.MiningFilter);
        }

        [Theory]
        [InlineData(LiquidityPoolMiningStatusFilter.Any)]
        [InlineData(LiquidityPoolMiningStatusFilter.Enabled)]
        [InlineData(LiquidityPoolMiningStatusFilter.Disabled)]
        public void ProvisionalFilter_MiningFilterValid(LiquidityPoolMiningStatusFilter filter)
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
            {
                MiningFilter = filter
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(r => r.MiningFilter);
        }

        [Fact]
        public void Limit_Invalid()
        {
            // Arrange
            var request = new LiquidityPoolFilterParameters
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
            var request = new LiquidityPoolFilterParameters
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
