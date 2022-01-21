using FluentValidation.TestHelper;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.WebApi.Models.Requests.Markets;
using Opdex.Platform.WebApi.Validation.Markets;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Markets;

public class SetMarketPermissionsQuoteRequestValidatorTests
{
    private readonly SetMarketPermissionsQuoteRequestValidator _validator;

    public SetMarketPermissionsQuoteRequestValidatorTests()
    {
        _validator = new SetMarketPermissionsQuoteRequestValidator();
    }

    [Fact]
    public void Permission_Invalid()
    {
        // Arrange
        var request = new SetMarketPermissionsQuoteRequest
        {
            Permission = (MarketPermissionType)255
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(r => r.Permission);
    }

    [Theory]
    [InlineData(MarketPermissionType.Provide)]
    [InlineData(MarketPermissionType.Trade)]
    [InlineData(MarketPermissionType.CreatePool)]
    [InlineData(MarketPermissionType.SetPermissions)]
    public void Permission_Valid(MarketPermissionType permissionType)
    {
        // Arrange
        var request = new SetMarketPermissionsQuoteRequest
        {
            Permission = permissionType
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(r => r.Permission);
    }
}
