using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Validation.Vaults;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Vaults
{
    public class VaultCertificateFilterParametersValidatorTests
    {
        private readonly VaultCertificateFilterParametersValidator _validator;

        public VaultCertificateFilterParametersValidatorTests()
        {
            _validator = new VaultCertificateFilterParametersValidator();
        }

        [Theory]
        [ClassData(typeof(NonNetworkAddressData))]
        public void LockedToken_Invalid(Address lockedToken)
        {
            // Arrange
            var request = new VaultCertificateFilterParameters
            {
                Holder = lockedToken
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.Holder);
        }

        [Theory]
        [ClassData(typeof(EmptyAddressData))]
        [ClassData(typeof(ValidNetworkAddressData))]
        public void LockedToken_Valid(Address lockedToken)
        {
            // Arrange
            var request = new VaultCertificateFilterParameters
            {
                Holder = lockedToken
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.Holder);
        }

        [Fact]
        public void Limit_Invalid()
        {
            // Arrange
            var request = new VaultCertificateFilterParameters
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
            var request = new VaultCertificateFilterParameters
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
