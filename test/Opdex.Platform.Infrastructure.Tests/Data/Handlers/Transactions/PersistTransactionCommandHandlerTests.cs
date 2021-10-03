using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Transactions;
using Opdex.Platform.Infrastructure.Data.Handlers.Transactions;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Transactions
{
    public class PersistTransactionCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistTransactionCommandHandler _handler;

        public PersistTransactionCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistTransactionCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistTransactionCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task PersistsTransaction_Success()
        {
            const ulong id = 1234ul;
            var transaction = new Transaction("txHash", ulong.MaxValue, 1, "PFrSHgtz2khDuciJdLAZtR2uKwgyXryMjM", "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", true, null, null);
            var command = new PersistTransactionCommand(transaction);

            _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(id));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(id);
        }

        [Fact]
        public async Task PersistsTransaction_Fail()
        {
            const ulong id = 0ul;
            var transaction = new Transaction("txHash", ulong.MaxValue, 1, "PFrSHgtz2khDuciJdLAZtR2uKwgyXryMjM", "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh", true, null, null);
            var command = new PersistTransactionCommand(transaction);

            _dbContext.Setup(db => db.ExecuteScalarAsync<ulong>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(id));

            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(0);
        }
    }
}
