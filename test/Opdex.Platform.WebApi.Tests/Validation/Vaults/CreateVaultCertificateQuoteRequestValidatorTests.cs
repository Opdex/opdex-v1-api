using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Validation.Vaults;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Vaults
{
    public class CreateVaultCertificateQuoteRequestValidatorTests
    {
        private readonly CreateVaultCertificateQuoteRequestValidator _validator;

        public CreateVaultCertificateQuoteRequestValidatorTests()
        {
            _validator = new CreateVaultCertificateQuoteRequestValidator();
        }

        [Fact]
        public void Amount_Zero_Invalid()
        {
            // Arrange
            var request = new CreateVaultCertificateQuoteRequest
            {
                Amount = FixedDecimal.Zero,
                Holder = Address.Empty
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.Amount);
        }

        [Fact]
        public void Amount_GreaterThanZero_Valid()
        {
            // Arrange
            var request = new CreateVaultCertificateQuoteRequest
            {
                Amount = FixedDecimal.Parse("0.00000001"),
                Holder = Address.Empty
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.Amount);
        }

        [Theory]
        [ClassData(typeof(EmptyAddressData))]
        [ClassData(typeof(NonNetworkAddressData))]
        public void Holder_Invalid(Address holder)
        {
            // Arrange
            var request = new CreateVaultCertificateQuoteRequest
            {
                Amount = FixedDecimal.Zero,
                Holder = holder
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.Holder);
        }

        [Fact]
        public void Holder_Valid()
        {
            // Arrange
            var request = new CreateVaultCertificateQuoteRequest
            {
                Amount = FixedDecimal.Zero,
                Holder = new Address("PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh")
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.Holder);
        }
    }
}
