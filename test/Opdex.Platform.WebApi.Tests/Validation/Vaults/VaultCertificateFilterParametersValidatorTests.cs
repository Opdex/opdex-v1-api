using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults.Certificates;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Validation.Vaults;
using System.Collections.Generic;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Vaults;

public class VaultCertificateFilterParametersValidatorTests
{
    private readonly VaultCertificateFilterParametersValidator _validator;

    public VaultCertificateFilterParametersValidatorTests()
    {
        _validator = new VaultCertificateFilterParametersValidator();
    }

    [Theory]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Owner_Invalid(Address owner)
    {
        // Arrange
        var request = new VaultCertificateFilterParameters
        {
            Owner = owner
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Owner);
    }

    [Theory]
    [ClassData(typeof(NullAddressData))]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void Owner_Valid(Address owner)
    {
        // Arrange
        var request = new VaultCertificateFilterParameters
        {
            Owner = owner
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Owner);
    }

    [Fact]
    public void Status_Invalid()
    {
        // Arrange
        var request = new VaultCertificateFilterParameters
        {
            Status = new HashSet<VaultCertificateStatusFilter> { (VaultCertificateStatusFilter)255 }
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
        var request = new VaultCertificateFilterParameters
        {
            Status = new HashSet<VaultCertificateStatusFilter> { type }
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
        var request = new VaultCertificateFilterParameters
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
        var request = new VaultCertificateFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Limit);
    }
}
