using FluentAssertions;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Addresses;
using Xunit;

namespace Opdex.Platform.Application.Tests.Mappers;

public class AddressesPlatformApplicationMapperProfileTests : PlatformApplicationMapperProfileTests
{
    [Fact]
    public void From_AddressAllowance_To_AddressAllowanceDto()
    {
        // Arrange
        var model = new AddressAllowance(5L, 15L, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcG", new UInt256("5000060000"), 500, 1000);

        // Act
        var dto = _mapper.Map<AddressAllowanceDto>(model);

        // Assert
        dto.Owner.Should().Be(model.Owner);
        dto.Spender.Should().Be(model.Spender);
    }
}