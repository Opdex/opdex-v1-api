using AutoMapper;
using FluentAssertions;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.WebApi.Mappers;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools;
using Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.MiningPools;
using Opdex.Platform.WebApi.Models.Responses.Vaults;
using Opdex.Platform.WebApi.Models.Responses.Wallet;
using System.Linq;
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

        #region Transactions

        [Fact]
        public void From_StartMiningEventDto_To_StartMiningResponseModel()
        {
            // Arrange
            var dto = new StartMiningEventDto
            {
                Id = 1,
                TransactionId = 2,
                SortOrder = 1,
                Amount = "100.00000000",
                Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                Miner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                MinerBalance = "100.00000000",
                TotalSupply = "234.00000000"
            };

            // Act
            var response = _mapper.Map<StartMiningEventResponseModel>(dto);

            // Assert
            response.Contract.Should().Be(dto.Contract);
            response.SortOrder.Should().Be(dto.SortOrder);
            response.EventType.Should().Be(TransactionEventType.StartMiningEvent);
            response.Miner.Should().Be(dto.Miner);
            response.MinerBalance.Should().Be(dto.MinerBalance);
            response.TotalSupply.Should().Be(dto.TotalSupply);
            response.Amount.Should().Be(dto.Amount);
        }

        [Fact]
        public void From_StopMiningEventDto_To_StopMiningResponseModel()
        {
            // Arrange
            var dto = new StopMiningEventDto
            {
                Id = 1,
                TransactionId = 2,
                SortOrder = 1,
                Amount = "100.00000000",
                Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                Miner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                MinerBalance = "100.00000000",
                TotalSupply = "234.00000000"
            };

            // Act
            var response = _mapper.Map<StopMiningEventResponseModel>(dto);

            // Assert
            response.Contract.Should().Be(dto.Contract);
            response.SortOrder.Should().Be(dto.SortOrder);
            response.EventType.Should().Be(TransactionEventType.StopMiningEvent);
            response.Miner.Should().Be(dto.Miner);
            response.MinerBalance.Should().Be(dto.MinerBalance);
            response.TotalSupply.Should().Be(dto.TotalSupply);
            response.Amount.Should().Be(dto.Amount);
        }

        [Fact]
        public void From_StartStakingEventDto_To_StartStakingResponseModel()
        {
            // Arrange
            var dto = new StartStakingEventDto
            {
                Id = 1,
                TransactionId = 2,
                SortOrder = 1,
                Amount = "100.00000000",
                Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                Staker = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                StakerBalance = "100.00000000",
                TotalStaked = "234.00000000"
            };

            // Act
            var response = _mapper.Map<StartStakingEventResponseModel>(dto);

            // Assert
            response.Contract.Should().Be(dto.Contract);
            response.SortOrder.Should().Be(dto.SortOrder);
            response.EventType.Should().Be(TransactionEventType.StartStakingEvent);
            response.Staker.Should().Be(dto.Staker);
            response.StakerBalance.Should().Be(dto.StakerBalance);
            response.TotalStaked.Should().Be(dto.TotalStaked);
            response.Amount.Should().Be(dto.Amount);
        }

        [Fact]
        public void From_StopStakingEventDto_To_StopStakingResponseModel()
        {
            // Arrange
            var dto = new StopStakingEventDto
            {
                Id = 1,
                TransactionId = 2,
                SortOrder = 1,
                Amount = "100.00000000",
                Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                Staker = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                StakerBalance = "100.00000000",
                TotalStaked = "234.00000000"
            };

            // Act
            var response = _mapper.Map<StopStakingEventResponseModel>(dto);

            // Assert
            response.Contract.Should().Be(dto.Contract);
            response.SortOrder.Should().Be(dto.SortOrder);
            response.EventType.Should().Be(TransactionEventType.StopStakingEvent);
            response.Staker.Should().Be(dto.Staker);
            response.StakerBalance.Should().Be(dto.StakerBalance);
            response.TotalStaked.Should().Be(dto.TotalStaked);
            response.Amount.Should().Be(dto.Amount);
        }

        [Fact]
        public void From_AddLiquidityEventDto_To_AddLiquidityResponseModel()
        {
            // Arrange
            var dto = new AddLiquidityEventDto
            {
                Id = 1,
                TransactionId = 2,
                SortOrder = 1,
                Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                AmountCrs = "100.00000000",
                AmountSrc = "200.00000000",
                AmountLpt = "300.00000000",
                TokenSrc = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                TokenLp = "PVguEK5irNcbk7ZfvJtSJgyGvV5JqMgWVr",
                TokenLpTotalSupply = "400.00000000",
            };

            // Act
            var response = _mapper.Map<AddLiquidityEventResponseModel>(dto);

            // Assert
            response.Contract.Should().Be(dto.Contract);
            response.SortOrder.Should().Be(dto.SortOrder);
            response.EventType.Should().Be(TransactionEventType.AddLiquidityEvent);
            response.AmountCrs.Should().Be(dto.AmountCrs);
            response.AmountSrc.Should().Be(dto.AmountSrc);
            response.AmountLpt.Should().Be(dto.AmountLpt);
            response.TokenSrc.Should().Be(dto.TokenSrc);
            response.TokenLp.Should().Be(dto.TokenLp);
            response.TokenLpTotalSupply.Should().Be(dto.TokenLpTotalSupply);
        }

        [Fact]
        public void From_RemoveLiquidityEventDto_To_RemoveLiquidityResponseModel()
        {
            // Arrange
            var dto = new RemoveLiquidityEventDto
            {
                Id = 1,
                TransactionId = 2,
                SortOrder = 1,
                Contract = " PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXk",
                AmountCrs = "100.00000000",
                AmountSrc = "200.00000000",
                AmountLpt = "300.00000000",
                TokenSrc = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXl",
                TokenLp = "PVguEK5irNcbk7ZfvJtSJgyGvV5JqMgWVr",
                TokenLpTotalSupply = "400.00000000",
            };

            // Act
            var response = _mapper.Map<RemoveLiquidityEventResponseModel>(dto);

            // Assert
            response.Contract.Should().Be(dto.Contract);
            response.SortOrder.Should().Be(dto.SortOrder);
            response.EventType.Should().Be(TransactionEventType.RemoveLiquidityEvent);
            response.AmountCrs.Should().Be(dto.AmountCrs);
            response.AmountSrc.Should().Be(dto.AmountSrc);
            response.AmountLpt.Should().Be(dto.AmountLpt);
            response.TokenSrc.Should().Be(dto.TokenSrc);
            response.TokenLp.Should().Be(dto.TokenLp);
            response.TokenLpTotalSupply.Should().Be(dto.TokenLpTotalSupply);
        }

        #endregion

        [Fact]
        public void From_CertificateDto_To_CertificateResponseModel()
        {
            // Arrange
            var dto = new VaultCertificateDto
            {
                Owner = "PBJPuCXfcNKdN28FQf5uJYUcmAsqAEgUXM",
                Amount = "5021.24920000",
                VestingStartBlock = 10002,
                VestingEndBlock = 141842,
                Redeemed = false,
                Revoked = true
            };

            // Act
            var response = _mapper.Map<VaultCertificateResponseModel>(dto);

            // Assert
            response.Owner.Should().Be(dto.Owner);
            response.Amount.Should().Be(dto.Amount);
            response.VestingStartBlock.Should().Be(dto.VestingStartBlock);
            response.VestingEndBlock.Should().Be(dto.VestingEndBlock);
            response.Redeemed.Should().Be(dto.Redeemed);
            response.Revoked.Should().Be(dto.Revoked);
        }

        [Fact]
        public void From_CertificatesDto_To_CertificatesResponseModel()
        {
            // Arrange
            var dto = new VaultCertificatesDto
            {
                Certificates = new VaultCertificateDto[] { new VaultCertificateDto(), new VaultCertificateDto(), new VaultCertificateDto() },
                Cursor = new CursorDto { Next = "aG9sZGVyOjtkaXJlY3Rpb246QVNDO2xpbWl0OjI7cGFnaW5nOkZvcndhcmQ7cG9pbnRlcjpNZz09Ow", Previous = "aG9sZGVyOjtkaXJlY3Rpb246QVNDO2xpbWl0OjI7cGFnaW5nOkZvcndhcmQ7cG9pbnRlcjpNZz09Ow==" }
            };

            // Act
            var response = _mapper.Map<VaultCertificatesResponseModel>(dto);

            // Assert
            response.Results.Count().Should().Be(dto.Certificates.Count());
            response.Paging.Next.Should().Be(dto.Cursor.Next);
            response.Paging.Previous.Should().Be(dto.Cursor.Previous);
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
