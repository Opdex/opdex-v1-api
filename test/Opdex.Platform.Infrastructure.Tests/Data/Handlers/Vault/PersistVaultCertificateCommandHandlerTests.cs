using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vault;
using Opdex.Platform.Infrastructure.Data.Handlers.Vault;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data.Handlers.Vault
{
    public class PersistVaultCertificateCommandHandlerTests
    {
        private readonly Mock<IDbContext> _dbContext;
        private readonly PersistVaultCertificateCommandHandler _handler;
        
        public PersistVaultCertificateCommandHandlerTests()
        {
            var mapper = new MapperConfiguration(config => config.AddProfile(new PlatformInfrastructureMapperProfile())).CreateMapper();
            var logger = new NullLogger<PersistVaultCertificateCommandHandler>();

            _dbContext = new Mock<IDbContext>();
            _handler = new PersistVaultCertificateCommandHandler(_dbContext.Object, mapper, logger);
        }

        [Fact]
        public async Task Insert_VaultCertificate_Success()
        {
            var allowance = new VaultCertificate(1, "Owner", "1234", 10000, 123);
            var command = new PersistVaultCertificateCommand(allowance);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task Update_VaultCertificate_Success()
        {
            var allowance = new VaultCertificate(10, 1, "Owner", "1234", 10000, true,  123, 123);
            var command = new PersistVaultCertificateCommand(allowance);

            _dbContext.Setup(db => db.ExecuteCommandAsync(It.IsAny<DatabaseQuery>()))
                .Returns(() => Task.FromResult(1));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeTrue();
        }
        
        [Fact]
        public async Task PersistsVaultCertificate_Fail()
        {
            var allowance = new VaultCertificate(10, 1, "Owner", "1234", 10000, true,  123, 123);
            var command = new PersistVaultCertificateCommand(allowance);

            _dbContext.Setup(db => db.ExecuteScalarAsync<long>(It.IsAny<DatabaseQuery>()))
                .Throws(new Exception("Some SQL Exception"));
            
            var result = await _handler.Handle(command, CancellationToken.None);

            result.Should().BeFalse();
        }
    }
}