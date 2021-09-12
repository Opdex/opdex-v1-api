using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;
using Opdex.Platform.Infrastructure.Data.Handlers.Tokens;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Tokens
{
    public class PersistTokenCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTokenCommandHandler _handler;

        public PersistTokenCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistTokenCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistTokenCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task PersistsToken_Success()
        {
            var token = new Token("PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM", true, "TokenName", "TKN", 8, 100_000_000, 500000000, 1);
            var command = new PersistTokenCommand(token);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1234L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(1234);
        }

        [Fact]
        public async Task PersistsToken_Fail()
        {
            var token = new Token("PNvzq4pxJ5v3pp9kDaZyifKNspGD79E4qM", true, "TokenName", "TKN", 8, 100_000_000, 500000000, 1);
            var command = new PersistTokenCommand(token);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(0L));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(0);
        }
    }
}
