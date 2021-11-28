using AutoMapper;
using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.MiningGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningGovernances;
using Opdex.Platform.Infrastructure.Data.Handlers.MiningGovernances;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.MiningGovernances
{
    public class SelectMiningGovernanceByAddressQueryHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly SelectMiningGovernanceByAddressQueryHandler _handler;

        public SelectMiningGovernanceByAddressQueryHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();

            _dbContext = new Mock<IDbContext>();
            _handler = new SelectMiningGovernanceByAddressQueryHandler(_dbContext.Object, mapper);
        }

        [Fact]
        public async Task SelectMiningGovernanceByAddress_Success()
        {
            Address address = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";

            var expectedEntity = new MiningGovernanceEntity
            {
                Id = 123454,
                Address = address,
                TokenId = 10,
                NominationPeriodEnd = 999,
                MiningDuration = 1444,
                MiningPoolsFunded = 10,
                MiningPoolReward = 876543456789,
                CreatedBlock = 1
            };

            var command = new SelectMiningGovernanceByAddressQuery(address);

            _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedEntity));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Id.Should().Be(expectedEntity.Id);
            result.TokenId.Should().Be(expectedEntity.TokenId);
            result.Address.Should().Be(expectedEntity.Address);
            result.NominationPeriodEnd.Should().Be(expectedEntity.NominationPeriodEnd);
            result.MiningDuration.Should().Be(expectedEntity.MiningDuration);
            result.MiningPoolsFunded.Should().Be(expectedEntity.MiningPoolsFunded);
            result.MiningPoolReward.Should().Be(expectedEntity.MiningPoolReward);
        }

        [Fact]
        public void SelectMiningGovernanceByAddress_Throws_NotFoundException()
        {
            var command = new SelectMiningGovernanceByAddressQuery("PGZPZpB4iW4LHVEPMKehXfJ6u1yzNPDw7u");

            _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<MiningGovernanceEntity>(null));

            _handler.Invoking(h => h.Handle(command, CancellationToken.None))
                .Should()
                .Throw<NotFoundException>()
                .WithMessage($"{nameof(MiningGovernance)} not found.");
        }

        [Fact]
        public async Task SelectMiningGovernanceByAddress_ReturnsNull()
        {
            Address address = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";
            const bool findOrThrow = false;

            var command = new SelectMiningGovernanceByAddressQuery(address, findOrThrow);

            _dbContext.Setup(db => db.ExecuteFindAsync<MiningGovernanceEntity>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult<MiningGovernanceEntity>(null));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeNull();
        }
    }
}
