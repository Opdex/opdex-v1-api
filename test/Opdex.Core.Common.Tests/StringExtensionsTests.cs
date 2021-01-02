using FluentAssertions;
using Opdex.Core.Common.Extensions;
using Xunit;

namespace Opdex.Core.Common.Tests
{
    public class StringExtensionsTests
    {
        [Theory]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData(null, false)]
        [InlineData("Test", true)]
        public void StringHasValue_Success(string value, bool expected)
        {
            value.HasValue().Should().Be(expected);
        }
    }
}