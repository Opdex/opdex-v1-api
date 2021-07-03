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
            var model = new AddressAllowance(5L, 15L, "", "", "5000060000", 500, 1000);

            // Act
            var dto = _mapper.Map<AddressAllowanceDto>(model);

            // Assert
            dto.Amount.Should().Be(model.Allowance);
        }
    }
}
