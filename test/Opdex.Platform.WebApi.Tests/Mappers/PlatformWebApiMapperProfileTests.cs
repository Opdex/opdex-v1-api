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

        [Fact]
        public void From_MiningPositionDto_To_MiningPositionResponseModel()
        {
            // Arrange
            var dto = new MiningPositionDto
            {
                Address = "PHUzrtkLfffDZMd2v8QULRZvBCY5RwrrQK",
                Amount = "200.00000000",
                MiningPool = "PXResSytiRhJwNiD1DS9aZinPjEUvk8BuX",
                MiningToken = "PSqkCUMpPykkfL3XhYPefjjc9U4kqdrc4L"
            };

            // Act
            var response = _mapper.Map<MiningPositionResponseModel>(dto);

            // Assert
            response.Address.Should().Be(dto.Address);
            response.Amount.Should().Be(dto.Amount);
            response.MiningPool.Should().Be(dto.MiningPool);
            response.MiningToken.Should().Be(dto.MiningToken);
        }
    }
}
