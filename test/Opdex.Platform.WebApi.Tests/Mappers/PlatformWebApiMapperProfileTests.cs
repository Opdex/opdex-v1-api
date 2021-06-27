using AutoMapper;
using FluentAssertions;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.WebApi.Mappers;
using Opdex.Platform.WebApi.Models.Responses.Wallet;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Mappers
{
    public class PlatformWebApiMapperProfileTests
    {
        private readonly IMapper _mapper;

        public PlatformWebApiMapperProfileTests()
        {
            _mapper = new MapperConfiguration(config => config.AddProfile(new PlatformWebApiMapperProfile())).CreateMapper();
        }

        [Fact]
        public void From_AddressAllowanceDto_To_ApprovedAllowanceResponseModel()
        {
            // Arrange
            var dto = new AddressAllowanceDto { Amount = "500000" };

            // Act
            var response = _mapper.Map<ApprovedAllowanceResponseModel>(dto);

            // Assert
            response.Amount.Should().Be(dto.Amount);
        }
    }
}
