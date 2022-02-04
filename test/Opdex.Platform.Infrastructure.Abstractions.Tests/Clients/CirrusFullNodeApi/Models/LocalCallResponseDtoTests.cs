using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Xunit;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests.Clients.CirrusFullNodeApi.Models;

public class LocalCallResponseDtoTests
{
    [Fact]
    public void TryDeserializeValue_RevertTrue_ReturnFalseAndDefault()
    {
        // Arrange
        var localCallResponseDto = new LocalCallResponseDto()
        {
            Revert = true
        };

        // Act
        var success = localCallResponseDto.TryDeserializeValue(out Address owner);

        // Assert
        success.Should().Be(false);
        owner.Should().Be(default(Address));
    }

    [Fact]
    public void TryDeserializeValue_ValueTypeNull_ReturnFalseAndDefault()
    {
        // Arrange
        var localCallResponseDto = new LocalCallResponseDto()
        {
            Revert = false,
            Return = null
        };

        // Act
        var success = localCallResponseDto.TryDeserializeValue(out int count);

        // Assert
        success.Should().Be(false);
        count.Should().Be(default);
    }

    [Fact]
    public void TryDeserializeValue_ReferenceTypeNull_ReturnTrueAndNull()
    {
        // Arrange
        var localCallResponseDto = new LocalCallResponseDto()
        {
            Revert = false,
            Return = null
        };

        // Act
        var success = localCallResponseDto.TryDeserializeValue(out string name);

        // Assert
        success.Should().Be(true);
        name.Should().Be(null);
    }

    [Fact]
    public void TryDeserializeValue_UnableToDeserialize_ReturnFalseAndDefault()
    {
        // Arrange
        var localCallResponseDto = new LocalCallResponseDto()
        {
            Revert = false,
            Return = "Unable to deserialize to an int"
        };

        // Act
        var success = localCallResponseDto.TryDeserializeValue(out int count);

        // Assert
        success.Should().Be(false);
        count.Should().Be(default);
    }

    [Fact]
    public void TryDeserializeValue_ReturnInvalidEnumValue_ReturnFalseAndValue()
    {
        // Arrange
        var localCallResponseDto = new LocalCallResponseDto()
        {
            Revert = false,
            Return = 5000
        };

        // Act
        var success = localCallResponseDto.TryDeserializeValue(out ExternalChainType chainType);

        // Assert
        success.Should().Be(false);
        chainType.Should().Be((ExternalChainType)5000);
    }

    [Fact]
    public void TryDeserializeValue_ReturnValidValue_ReturnTrueAndValue()
    {
        // Arrange
        var localCallResponseDto = new LocalCallResponseDto()
        {
            Revert = false,
            Return = 5
        };

        // Act
        var success = localCallResponseDto.TryDeserializeValue(out int count);

        // Assert
        success.Should().Be(true);
        count.Should().Be(5);
    }
}
