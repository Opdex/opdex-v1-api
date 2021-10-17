using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.Wallets;
using Opdex.Platform.WebApi.Validation.Wallets;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Wallets
{
    public class AddressBalanceFilterParametersValidatorTests
    {
        private readonly AddressBalanceFilterParametersValidator _validator;

        public AddressBalanceFilterParametersValidatorTests()
        {
            _validator = new AddressBalanceFilterParametersValidator();
        }

        [Theory]
        [ClassData(typeof(NullAddressData))]
        [ClassData(typeof(EmptyAddressData))]
        [ClassData(typeof(NonNetworkAddressData))]
        public void Tokens_Items_Invalid(Address address)
        {
            // Arrange
            var request = new AddressBalanceFilterParameters
            {
                Tokens = new Address[] { address }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.Tokens);
        }

        [Fact]
        public void Tokens_Items_Valid()
        {
            // Arrange
            var request = new AddressBalanceFilterParameters
            {
                Tokens = new Address[] { new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh") }
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.Tokens);
        }

        [Fact]
        public void Limit_Invalid()
        {
            // Arrange
            var request = new AddressBalanceFilterParameters
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
            var request = new AddressBalanceFilterParameters
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
