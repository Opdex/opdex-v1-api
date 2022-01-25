using FluentValidation.TestHelper;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Opdex.Platform.WebApi.Validation.Vaults;
using System.Collections.Generic;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Vaults;

public class VaultProposalFilterParametersValidatorTests
{
    private readonly VaultProposalFilterParametersValidator _validator;

    public VaultProposalFilterParametersValidatorTests()
    {
        _validator = new VaultProposalFilterParametersValidator();
    }

    [Theory]
    [InlineData(default(VaultProposalStatus))]
    [InlineData((VaultProposalStatus)255)]
    public void Status_Invalid(VaultProposalStatus value)
    {
        // Arrange
        var request = new VaultProposalFilterParameters
        {
            Status = new HashSet<VaultProposalStatus> { value }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Status);
    }

    [Theory]
    [InlineData(VaultProposalStatus.Pledge)]
    [InlineData(VaultProposalStatus.Vote)]
    [InlineData(VaultProposalStatus.Complete)]
    public void Status_Valid(VaultProposalStatus status)
    {
        // Arrange
        var request = new VaultProposalFilterParameters
        {
            Status = new HashSet<VaultProposalStatus> { status }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Status);
    }

    [Theory]
    [InlineData(default(VaultProposalType))]
    [InlineData((VaultProposalType)255)]
    public void Type_Invalid(VaultProposalType value)
    {
        // Arrange
        var request = new VaultProposalFilterParameters
        {
            Type = new HashSet<VaultProposalType> { value }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Type);
    }

    [Theory]
    [InlineData(VaultProposalType.Create)]
    [InlineData(VaultProposalType.Revoke)]
    [InlineData(VaultProposalType.TotalPledgeMinimum)]
    [InlineData(VaultProposalType.TotalVoteMinimum)]
    public void Type_Valid(VaultProposalType type)
    {
        // Arrange
        var request = new VaultProposalFilterParameters
        {
            Type = new HashSet<VaultProposalType> { type }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Type);
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
        result.ShouldHaveValidationErrorFor(r => r.Limit);
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
        result.ShouldNotHaveValidationErrorFor(r => r.Limit);
    }
}
