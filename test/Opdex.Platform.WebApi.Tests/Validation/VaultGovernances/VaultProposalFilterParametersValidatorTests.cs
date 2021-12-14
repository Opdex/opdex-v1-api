using FluentValidation.TestHelper;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.VaultGovernances;
using Opdex.Platform.WebApi.Validation.VaultGovernances;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.VaultGovernances;

public class VaultProposalFilterParametersValidatorTests
{
    private readonly VaultProposalFilterParametersValidator _validator;

    public VaultProposalFilterParametersValidatorTests()
    {
        _validator = new VaultProposalFilterParametersValidator();
    }

    [Fact]
    public void Status_Invalid()
    {
        // Arrange
        var request = new VaultProposalFilterParameters
        {
            Status = (VaultProposalStatus)255
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Status);
    }

    [Theory]
    [InlineData(default(VaultProposalStatus))]
    [InlineData(VaultProposalStatus.Pledge)]
    [InlineData(VaultProposalStatus.Vote)]
    [InlineData(VaultProposalStatus.Complete)]
    public void Status_Valid(VaultProposalStatus status)
    {
        // Arrange
        var request = new VaultProposalFilterParameters
        {
            Status = status
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Status);
    }

    [Fact]
    public void Type_Invalid()
    {
        // Arrange
        var request = new VaultProposalFilterParameters
        {
            Type = (VaultProposalType)255
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Type);
    }

    [Theory]
    [InlineData(default(VaultProposalType))]
    [InlineData(VaultProposalType.Create)]
    [InlineData(VaultProposalType.Revoke)]
    [InlineData(VaultProposalType.TotalPledgeMinimum)]
    [InlineData(VaultProposalType.TotalVoteMinimum)]
    public void Type_Valid(VaultProposalType type)
    {
        // Arrange
        var request = new VaultProposalFilterParameters
        {
            Type = type
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Type);
    }

    [Fact]
    public void Limit_Invalid()
    {
        // Arrange
        var request = new VaultProposalFilterParameters
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
        var request = new VaultProposalFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Limit);
    }
}
