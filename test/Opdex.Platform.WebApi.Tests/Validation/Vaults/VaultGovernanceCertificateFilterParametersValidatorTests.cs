using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Validation.Vaults;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Vaults;

public class VaultGovernanceCertificateFilterParametersValidatorTests
{
    private readonly VaultGovernanceCertificateFilterParametersValidator _validator;

    public VaultGovernanceCertificateFilterParametersValidatorTests()
    {
        _validator = new VaultGovernanceCertificateFilterParametersValidator();
    }

    [Theory]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Holder_Invalid(Address holder)
    {
        // Arrange
        var request = new VaultGovernanceCertificateFilterParameters
        {
            Holder = holder
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Holder);
    }

    [Theory]
    [ClassData(typeof(NullAddressData))]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void Holder_Valid(Address holder)
    {
        // Arrange
        var request = new VaultGovernanceCertificateFilterParameters
        {
            Holder = holder
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Holder);
    }

    [Fact]
    public void Status_Invalid()
    {
        // Arrange
        var request = new VaultGovernanceCertificateFilterParameters
        {
            Status = (VaultCertificateStatusFilter)255
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Status);
    }

    [Theory]
    [InlineData(default(VaultCertificateStatusFilter))]
    [InlineData(VaultCertificateStatusFilter.Vesting)]
    [InlineData(VaultCertificateStatusFilter.Redeemed)]
    [InlineData(VaultCertificateStatusFilter.Revoked)]
    public void Status_Valid(VaultCertificateStatusFilter type)
    {
        // Arrange
        var request = new VaultGovernanceCertificateFilterParameters
        {
            Status = type
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Status);
    }

    [Fact]
    public void Limit_Invalid()
    {
        // Arrange
        var request = new VaultGovernanceCertificateFilterParameters
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
        var request = new VaultGovernanceCertificateFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Limit);
    }
}
