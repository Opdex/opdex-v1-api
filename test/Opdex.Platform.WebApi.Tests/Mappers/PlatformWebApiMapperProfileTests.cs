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
            var dto = new AddressAllowanceDto
            {
                Allowance = "500000",
                Owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                Spender = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                Token = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXM",
            };

            // Act
            var response = _mapper.Map<ApprovedAllowanceResponseModel>(dto);

            // Assert
            response.Allowance.Should().Be(dto.Allowance);
            response.Owner.Should().Be(dto.Owner);
            response.Spender.Should().Be(dto.Spender);
            response.Token.Should().Be(dto.Token);
        }
    }
}
