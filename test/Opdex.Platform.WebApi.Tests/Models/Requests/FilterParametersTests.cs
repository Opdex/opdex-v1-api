using FluentAssertions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Models.Requests
{
    public class FilterParametersTests
    {
        [Fact]
        public void ValidateWellFormed_UnableToBuildCursor_False()
        {
            // Arrange
            var filters = new NullFilterParameters();

            // Act
            var wellFormed = filters.ValidateWellFormed();

            // Assert
            wellFormed.Should().Be(false);
        }

        [Fact]
        public void ValidateWellFormed_BuiltCursor_True()
        {
            // Arrange
            var filters = new WellFormedFilterParameters();

            // Act
            var wellFormed = filters.ValidateWellFormed();

            // Assert
            wellFormed.Should().Be(true);
        }
    }

    public class WellFormedFilterParameters : FilterParameters<StubCursor>
    {
        protected override StubCursor InternalBuildCursor()
        {
            return new StubCursor();
        }
    }

    public class NullFilterParameters : FilterParameters<StubCursor>
    {
        protected override StubCursor InternalBuildCursor()
        {
            return null;
        }
    }

    public class StubCursor : Cursor { }
}
