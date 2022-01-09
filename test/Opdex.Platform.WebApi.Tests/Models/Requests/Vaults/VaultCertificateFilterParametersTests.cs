using FluentAssertions;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.Vaults;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Models.Requests.Vaults;

public class VaultCertificatesFilterParametersTests
{
    [Fact]
    public void DefaultPropertyValues()
    {
        // Arrange
        // Act
        var filters = new VaultGovernanceCertificateFilterParameters();

        // Assert
        filters.Holder.Should().Be(Address.Empty);
        filters.Direction.Should().Be(default);
        filters.Limit.Should().Be(default);
    }

    [Fact]
    public void BuildCursor_CursorStringNotProvided_ReturnFiltered()
    {
        // Arrange
        var filters = new VaultGovernanceCertificateFilterParameters
        {
            Holder = new Address("tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm"),
            Limit = 20,
            Direction = SortDirectionType.DESC
        };

        // Act
        var cursor = filters.BuildCursor();

        // Assert
        cursor.Holder.Should().Be(filters.Holder);
        cursor.SortDirection.Should().Be(filters.Direction);
        cursor.Limit.Should().Be(filters.Limit);
        cursor.PagingDirection.Should().Be(PagingDirection.Forward);
        cursor.Pointer.Should().Be(default);
        cursor.IsFirstRequest.Should().Be(true);
    }

    [Fact]
    public void BuildCursor_NotABase64CursorString_ReturnNull()
    {
        // Arrange
        var filters = new VaultGovernanceCertificateFilterParameters { EncodedCursor = "NOT_BASE_64_****" };

        // Act
        var cursor = filters.BuildCursor();

        // Assert
        cursor.Should().Be(null);
    }

    [Fact]
    public void BuildCursor_NotAValidCursorString_ReturnNull()
    {
        // Arrange
        var filters = new VaultGovernanceCertificateFilterParameters { EncodedCursor = "Tk9UX1ZBTElE" };

        // Act
        var cursor = filters.BuildCursor();

        // Assert
        cursor.Should().Be(null);
    }

    [Fact]
    public void BuildCursor_ValidCursorString_ReturnCursor()
    {
        // Arrange
        var filters = new VaultGovernanceCertificateFilterParameters { EncodedCursor = "aG9sZGVyOjtzdGF0dXM6QWxsO2RpcmVjdGlvbjpERVNDO2xpbWl0OjU7cGFnaW5nOkZvcndhcmQ7cG9pbnRlcjpNdz09Ow==" };

        // Act
        var cursor = filters.BuildCursor();

        // Assert
        cursor.Should().NotBe(null);
    }
}
