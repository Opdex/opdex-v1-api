using AutoMapper;
using FluentAssertions;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Domain.Models.Addresses;
using Xunit;

namespace Opdex.Platform.Application.Tests
{
    public class PlatformApplicationMapperProfileTests
    {
        private readonly IMapper _mapper;

        public PlatformApplicationMapperProfileTests()
        {
            _mapper = new MapperConfiguration(config => config.AddProfile(new PlatformApplicationMapperProfile())).CreateMapper();
        }

        [Fact]
        public void From_AddressAllowance_To_AddressAllowanceDto()
        {
            // Arrange
            var model = new AddressAllowance(5L, 15L, "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXj", "PQFv8x66vXEQEjw7ZBi8kCavrz15S1ShcG", "5000060000", 500, 1000);

            // Act
            var dto = _mapper.Map<AddressAllowanceDto>(model);

            // Assert
            dto.Owner.Should().Be(model.Owner);
            dto.Spender.Should().Be(model.Spender);
        }
    }
}
