using AutoMapper;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Assemblers
{
    public class VaultDtoAssemblerTests
    {
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<IMediator> _mediatorMock;

        private readonly VaultDtoAssembler _assembler;

        public VaultDtoAssemblerTests()
        {
            _mapperMock = new Mock<IMapper>();
            _mediatorMock = new Mock<IMediator>();

            _assembler = new VaultDtoAssembler(_mapperMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Assemble_VaultDto_Map()
        {
            // Arrange
            var vault = new Vault(5, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", 15, "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", 500, UInt256.Parse("100000000"), 505, 510);

            // Act
            try
            {
                await _assembler.Assemble(vault);
            }
            catch (Exception) { }

            // Assert
            _mapperMock.Verify(callTo => callTo.Map<VaultDto>(vault), Times.Once);
        }

        [Fact]
        public async Task Assemble_RetrieveTokenByIdQuery_Send()
        {
            // Arrange
            var vault = new Vault(5, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", 15, "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", 500, UInt256.Parse("100000000"), 505, 510);

            // Act
            try
            {
                await _assembler.Assemble(vault);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveTokenByIdQuery>(query => query.TokenId == vault.TokenId && query.FindOrThrow),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Assemble_RetrieveAddressBalanceByTokenAddressAndOwnerQuery_Send()
        {
            // Arrange
            var vault = new Vault(5, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", 15, "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", 500, UInt256.Parse("100000000"), 505, 510);
            var token = new Token(15, "PHrN1DPvMcp17i5YL4yUzUCVcH2QimMvHi", false, "Wrapped Bitcoin", "WBTC", 8, 2100000000000000, UInt256.Parse("21000000"), 5, 15);

            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveTokenByIdQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(token);

            // Act
            try
            {
                await _assembler.Assemble(vault);
            }
            catch (Exception) { }

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveAddressBalanceByOwnerAndTokenQuery>(query => query.TokenId == token.Id
                                                                                                                  && query.Owner == vault.Address
                                                                                                                  && query.FindOrThrow),
                                                       It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
