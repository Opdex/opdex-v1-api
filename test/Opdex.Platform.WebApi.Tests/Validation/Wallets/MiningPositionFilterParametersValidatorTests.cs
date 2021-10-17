using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.Wallets;
using Opdex.Platform.WebApi.Validation.Wallets;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Wallets
{
    public class MiningPositionFilterParametersValidatorTests
    {
        private readonly MiningPositionFilterParametersValidator _validator;

        public MiningPositionFilterParametersValidatorTests()
        {
            _validator = new MiningPositionFilterParametersValidator();
        }

        [Theory]
        [ClassData(typeof(NullAddressData))]
        [ClassData(typeof(EmptyAddressData))]
        [ClassData(typeof(NonNetworkAddressData))]
        public void MiningPools_Items_Invalid(Address address)
        {
            // Arrange
            var request = new MiningPositionFilterParameters
            {
                MiningPools = new Address[] { address }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.MiningPools);
        }

        [Fact]
        public void MiningPools_Items_Valid()
        {
            // Arrange
            var request = new MiningPositionFilterParameters
            {
                MiningPools = new Address[] { new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh") }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.MiningPools);
        }

        [Theory]
        [ClassData(typeof(NullAddressData))]
        [ClassData(typeof(EmptyAddressData))]
        [ClassData(typeof(NonNetworkAddressData))]
        public void LiquidityPools_Items_Invalid(Address address)
        {
            // Arrange
            var request = new MiningPositionFilterParameters
            {
                LiquidityPools = new Address[] { address }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.LiquidityPools);
        }

        [Fact]
        public void LiquidityPools_Items_Valid()
        {
            // Arrange
            var request = new MiningPositionFilterParameters
            {
                LiquidityPools = new Address[] { new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh") }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.LiquidityPools);
        }

        [Fact]
        public void Limit_Invalid()
        {
            // Arrange
            var request = new MiningPositionFilterParameters
            {
                Limit = Cursor.DefaultMaxLimit + 1
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.Limit);
        }

        [Fact]
        public void Limit_Valid()
        {
            // Arrange
            var request = new MiningPositionFilterParameters
            {
                Limit = Cursor.DefaultMaxLimit
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.Limit);
        }
    }
}
