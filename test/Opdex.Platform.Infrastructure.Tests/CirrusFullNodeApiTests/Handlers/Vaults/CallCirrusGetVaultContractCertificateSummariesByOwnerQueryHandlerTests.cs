using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Vaults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Vaults
{
    public class CallCirrusGetVaultContractCertificateSummariesByOwnerQueryHandlerTests
    {
        private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
        private readonly CallCirrusGetVaultContractCertificateSummariesByOwnerQueryHandler _handler;

        public CallCirrusGetVaultContractCertificateSummariesByOwnerQueryHandlerTests()
        {
            _smartContractsModuleMock = new Mock<ISmartContractsModule>();
            _handler = new CallCirrusGetVaultContractCertificateSummariesByOwnerQueryHandler(_smartContractsModuleMock.Object);
        }

        [Fact] public void CallCirrusGetVaultContractCertificateSummariesByOwnerQuery_InvalidVault_ThrowsArgumentNullException()
        {
            // Arrange
            Address vault = Address.Empty;
            Address owner = "P1GLsMroh6zXXNMU9EjmivLgqqARwmH1iT";
            const ulong blockHeight = 10;

            // Act
            void Act() => new CallCirrusGetVaultContractCertificateSummariesByOwnerQuery(vault, owner, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Vault address must be provided.");
        }

        [Fact]
        public void CallCirrusGetVaultContractCertificateSummariesByOwnerQuery_InvalidOwner_ThrowsArgumentNullException()
        {
            // Arrange
            Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            Address owner = Address.Empty;
            const ulong blockHeight = 10;

            // Act
            void Act() => new CallCirrusGetVaultContractCertificateSummariesByOwnerQuery(vault, owner, blockHeight);

            // Assert
            Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Owner address must be provided.");
        }

        [Fact]
        public void CallCirrusGetVaultContractCertificateSummariesByOwnerQuery_InvalidBlockHeight_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            Address owner = "P1GLsMroh6zXXNMU9EjmivLgqqARwmH1iT";
            const ulong blockHeight = 0;

            // Act
            void Act() => new CallCirrusGetVaultContractCertificateSummariesByOwnerQuery(vault, owner, blockHeight);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
        }

        [Fact]
        public async Task CallCirrusGetVaultContractCertificateSummariesByOwnerQuery_Sends_LocalCallAsync()
        {
            // Arrange
            Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            Address owner = "P1GLsMroh6zXXNMU9EjmivLgqqARwmH1iT";
            const ulong blockHeight = 10;

            var parameters = new [] { new SmartContractMethodParameter(owner).Serialize() };

            // Act
            try
            {
                await _handler.Handle(new CallCirrusGetVaultContractCertificateSummariesByOwnerQuery(vault, owner, blockHeight), CancellationToken.None);
            }
            catch { }

            // Assert
            _smartContractsModuleMock.Verify(callTo => callTo.LocalCallAsync(It.Is<LocalCallRequestDto>(q => q.Amount == FixedDecimal.Zero &&
                                                                                                             q.MethodName == VaultConstants.Methods.GetCertificates &&
                                                                                                             q.Parameters.All(p => parameters.Contains(p)) &&
                                                                                                             q.ContractAddress == vault &&
                                                                                                             q.BlockHeight == blockHeight),
                                                                             It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task CallCirrusGetVaultContractCertificateSummariesByOwnerQuery_Returns_VaultContractCertificateSummaries()
        {
            // Arrange
            Address vault = "PU9EjmivLgqqARwmH1iT1GLsMroh6zXXNM";
            Address owner = "P1GLsMroh6zXXNMU9EjmivLgqqARwmH1iT";
            const ulong blockHeight = 10;

            var expectedCerts = new List<CertificateResponse>
            {
                new CertificateResponse{ Amount = 1, VestedBlock = 5, Revoked = false },
                new CertificateResponse{ Amount = 2, VestedBlock = 6, Revoked = true },
                new CertificateResponse{ Amount = 3, VestedBlock = 7, Revoked = false },
                new CertificateResponse{ Amount = 4, VestedBlock = 8, Revoked = true }
            };

            var expectedResponse = new LocalCallResponseDto { Return = expectedCerts };

            _smartContractsModuleMock.Setup(callTo => callTo.LocalCallAsync(It.IsAny<LocalCallRequestDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedResponse);

            // Act
            var response = await _handler.Handle(new CallCirrusGetVaultContractCertificateSummariesByOwnerQuery(vault, owner, blockHeight), CancellationToken.None);

            // Assert
            foreach (var cert in response)
            {
                var expected = expectedCerts.Single(nom => nom.VestedBlock == cert.VestedBlock);

                cert.Amount.Should().Be(expected.Amount);
                cert.VestedBlock.Should().Be(expected.VestedBlock);
                cert.Revoked.Should().Be(expected.Revoked);
            }
        }

        private sealed class CertificateResponse
        {
            public UInt256 Amount { get; set; }
            public ulong VestedBlock { get; set; }
            public bool Revoked { get; set; }
        }
    }
}
