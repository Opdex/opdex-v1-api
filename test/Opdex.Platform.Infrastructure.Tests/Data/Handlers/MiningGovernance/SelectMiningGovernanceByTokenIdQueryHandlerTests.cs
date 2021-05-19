using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernance;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernance;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningGovernance
{
    public class SelectMiningGovernanceByTokenIdQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMiningGovernanceByTokenIdQueryHandler _handler;
        
        public SelectMiningGovernanceByTokenIdQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMiningGovernanceByTokenIdQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectMiningGovernanceByTokenId_Success()
        {
            const long tokenId = 10;
            
            var expectedEntity = new MiningGovernanceEntity
            {
                Id = 123454,
                Address = "Address",
                TokenId = tokenId,
                NominationPeriodEnd = 999,
                Balance = "8765789",
                MiningPoolsFunded = 10,
                MiningPoolReward = "876543456789"
            };
                
            var command = new SelectMiningGovernanceByTokenIdQuery(tokenId);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedEntity));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedEntity.Id);
            result.TokenId.Should().Be(expectedEntity.TokenId);
            result.Address.Should().Be(expectedEntity.Address);
            result.NominationPeriodEnd.Should().Be(expectedEntity.NominationPeriodEnd);
            result.Balance.Should().Be(expectedEntity.Balance);
            result.MiningPoolsFunded.Should().Be(expectedEntity.MiningPoolsFunded);
            result.MiningPoolReward.Should().Be(expectedEntity.MiningPoolReward);
        }
        
        [Fact]
        public void SelectMiningGovernanceByTokenId_Throws_NotFoundException()
        {
            const long tokenId = 10;
            
            var command = new SelectMiningGovernanceByTokenIdQuery(tokenId);
        
            _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<MiningGovernanceEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(MiningGovernance)} not found.");
        }
    }
}