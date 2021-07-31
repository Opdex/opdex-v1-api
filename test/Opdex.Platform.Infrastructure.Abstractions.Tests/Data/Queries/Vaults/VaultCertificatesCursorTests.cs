using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using Xunit;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests.Data.Queries.Vaults
{
    public class VaultCertificatesCursorTests
    {
        [Fact]
        public void Create_LimitExceedsMaximum_ThrowArgumentOutOfRangeException()
        {
            // Arrange
            // Act
            static void Act() => new VaultCertificatesCursor("PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", SortDirectionType.ASC, VaultCertificatesCursor.MaxLimit + 1, PagingDirection.Forward, 0);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>("limit", Act);
        }

        [Fact]
        public void Create_InvalidPointer_ThrowArgumentException()
        {
            // Arrange
            // Act
            static void Act() => new VaultCertificatesCursor("PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", SortDirectionType.ASC, 25, PagingDirection.Forward, -1);

            // Assert
            Assert.Throws<ArgumentException>("pointer", Act);
        }

        [Fact]
        public void ToString_StringifiesCursor_FormatCorrectly()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor("PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Act
            var result = cursor.ToString();

            // Assert
            result.Should().Contain("holder:PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX;");
            result.Should().Contain("direction:ASC;");
            result.Should().Contain("limit:25;");
            result.Should().Contain("paging:Forward;");
            result.Should().Contain("pointer:NTAw;");
        }

        [Fact]
        public void Turn_NonIdenticalPointer_ReturnAnotherCursor()
        {
            // Arrange
            var cursor = new VaultCertificatesCursor("PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX", SortDirectionType.ASC, 25, PagingDirection.Forward, 500);

            // Act
            var result = cursor.Turn(PagingDirection.Backward, 567);

            // Assert
            result.Should().BeOfType<VaultCertificatesCursor>();
            var adjacentCursor = (VaultCertificatesCursor)result;
            adjacentCursor.Holder.Should().Be(cursor.Holder);
            adjacentCursor.OrderBy.Should().Be(cursor.OrderBy);
            adjacentCursor.Limit.Should().Be(cursor.Limit);
            adjacentCursor.PagingDirection.Should().Be(PagingDirection.Backward);
            adjacentCursor.Pointer.Should().Be(567);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(";:;;;;;::;;;:::;;;:::;;:::;;")]
        [InlineData("direction:Invalid;limit:50;paging:Forward;pointer:NTAw;")] // invalid orderBy
        [InlineData("limit:50;paging:Forward;pointer:NTAw;")] // missing orderBy
        [InlineData("direction:ASC;limit:51;paging:Forward;pointer:NTAw;")] // over max limit
        [InlineData("direction:ASC;paging:Forward;pointer:NTAw;")] // missing limit
        [InlineData("direction:ASC;limit:50;paging:Invalid;pointer:NTAw;")] // invalid paging direction
        [InlineData("direction:ASC;limit:50;pointer:NTAw;")] // missing paging direction
        [InlineData("direction:ASC;limit:50;paging:Forward;pointer:LTE=;")] // pointer: -1;
        [InlineData("direction:ASC;limit:50;paging:Forward;pointer:YWJj")] // pointer: abc;
        [InlineData("direction:ASC;limit:50;paging:Forward;pointer:10")] // pointer not valid base64
        [InlineData("direction:ASC;limit:50;paging:Forward;")] // pointer missing
        public void TryParse_InvalidCursor_ReturnFalse(string stringified)
        {
            // Arrange
            // Act
            var canParse = VaultCertificatesCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(false);
            cursor.Should().Be(null);
        }

        [Fact]
        public void TryParse_ValidCursor_ReturnTrue()
        {
            // Arrange
            var stringified = "holder:;direction:ASC;limit:50;paging:Forward;pointer:MTA=;"; // pointer: 10;

            // Act
            var canParse = VaultCertificatesCursor.TryParse(stringified, out var cursor);

            // Assert
            canParse.Should().Be(true);
            cursor.Holder.Should().Be("");
            cursor.OrderBy.Should().Be(SortDirectionType.ASC);
            cursor.Limit.Should().Be(50);
            cursor.PagingDirection.Should().Be(PagingDirection.Forward);
            cursor.Pointer.Should().Be(10);
        }
    }
}
