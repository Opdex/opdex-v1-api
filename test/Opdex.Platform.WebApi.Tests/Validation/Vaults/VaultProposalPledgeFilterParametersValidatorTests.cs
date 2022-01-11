using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Validation.Vaults;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Vaults;

public class VaultProposalPledgeFilterParametersValidatorTests
{
    private readonly VaultProposalPledgeFilterParametersValidator _validator;

    public VaultProposalPledgeFilterParametersValidatorTests()
    {
        _validator = new VaultProposalPledgeFilterParametersValidator();
    }

    [Theory]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Pledger_Invalid(Address pledger)
    {
        // Arrange
        var request = new VaultProposalPledgeFilterParameters
        {
            Pledger = pledger
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Pledger);
    }

    [Theory]
    [ClassData(typeof(NullAddressData))]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void Pledger_Valid(Address pledger)
    {
        // Arrange
        var request = new VaultProposalPledgeFilterParameters
        {
            Pledger = pledger
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Pledger);
    }

    [Fact]
    public void Limit_Invalid()
    {
        // Arrange
        var request = new VaultProposalPledgeFilterParameters
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
        var request = new VaultProposalPledgeFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Limit);
    }
}
