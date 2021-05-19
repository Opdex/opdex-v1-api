using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using Opdex.Platform.Infrastructure.Data.Handlers.Addresses;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Addresses
{
    public class PersistAddressAllowanceCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistAddressAllowanceCommandHandler _handler;
        
        public PersistAddressAllowanceCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistAddressAllowanceCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistAddressAllowanceCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task Insert_AddressAllowance_Success()
        {
            const long expectedId = 10;
            var allowance = new AddressAllowance(1, 2, "Owner", "Spender", "100000000", 3, 4);
            var command = new PersistAddressAllowanceCommand(allowance);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedId));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }
        
        [Fact]
        public async Task Update_AddressAllowance_Success()
        {
            const long expectedId = 10;
            var allowance = new AddressAllowance(expectedId, 1, 2, "Owner", "Spender", "100000000", 3, 4);
            var command = new PersistAddressAllowanceCommand(allowance);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(expectedId));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }
        
        [Fact]
        public async Task PersistsAddressAllowance_Fail()
        {
            const long expectedId = 0;
            var allowance = new AddressAllowance(expectedId, 1, 2, "Owner", "Spender", "100000000", 3, 4);
            var command = new PersistAddressAllowanceCommand(allowance);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Throws(new Exception("Some SQL Exception"));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().Be(expectedId);
        }
    }
}