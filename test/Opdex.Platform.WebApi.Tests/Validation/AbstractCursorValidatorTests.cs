using FluentValidation.TestHelper;
using Opdex.Platform.WebApi.Models.Requests;
using Opdex.Platform.WebApi.Tests.Models.Requests;
using Opdex.Platform.WebApi.Validation;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation
{
    public class AbstractCursorValidatorTests
    {
        private readonly FakeAbstractCursorValidator _validator;

        public AbstractCursorValidatorTests()
        {
            _validator = new FakeAbstractCursorValidator();
        }

        [Fact]
        public void Cursor_Invalid()
        {
            // Arrange
            var request = new NullFilterParameters
            {
                Cursor = "INVALID_CURSOR"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldHaveValidationErrorFor(request => request.Cursor);
        }

        [Fact]
        public void Cursor_Valid()
        {
            // Arrange
            var request = new WellFormedFilterParameters
            {
                Cursor = "VALID_CURSOR"
            };

            // Act
            var result = _validator.TestValidate(request);

            // Assert
            result.ShouldNotHaveValidationErrorFor(request => request.Cursor);
        }
    }

    public class FakeAbstractCursorValidator : AbstractCursorValidator<FilterParameters<StubCursor>, StubCursor> { }
}
