using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens.Distribution;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens
{
    public class PersistTokenDistributionCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTokenDistributionCommandHandler _handler;
        
        public PersistTokenDistributionCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistTokenDistributionCommandHandler>();
            
            _dbContext = new Mock<IDbContext>();
            _handler = new PersistTokenDistributionCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task PersistsTokenDistribution_Success()
        {
            var tokenDistribution = new TokenDistribution(1, "100.11", "1000.11", 1, 2, 3);
            var command = new PersistTokenDistributionCommand(tokenDistribution);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task PersistsTokenDistribution_Fail()
        {
            var tokenDistribution = new TokenDistribution(1, "100.11", "1000.11", 1, 2, 3);
            var command = new PersistTokenDistributionCommand(tokenDistribution);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeFalse();
        }
    }
}