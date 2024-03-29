using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.MiningGovernances;
using Opdex.Platform.WebApi.Validation.MiningGovernances;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.MiningGovernances;

public class MiningGovernanceFilterParametersValidatorTests
{
    private readonly MiningGovernanceFilterParametersValidator _validator;

    public MiningGovernanceFilterParametersValidatorTests()
    {
        _validator = new MiningGovernanceFilterParametersValidator();
    }

    [Theory]
    [ClassData(typeof(NonNetworkAddressData))]
    public void MinedToken_Invalid(Address minedToken)
    {
        // Arrange
        var request = new MiningGovernanceFilterParameters
        {
            MinedToken = minedToken
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.MinedToken);
    }

    [Theory]
    [ClassData(typeof(NullAddressData))]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void MinedToken_Valid(Address minedToken)
    {
        // Arrange
        var request = new MiningGovernanceFilterParameters
        {
            MinedToken = minedToken
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.MinedToken);
    }

    [Fact]
    public void Limit_Invalid()
    {
        // Arrange
        var request = new MiningGovernanceFilterParameters
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
        var request = new MiningGovernanceFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Limit);
    }
}