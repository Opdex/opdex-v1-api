using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.EntryHandlers.Vaults
{
    public class GetVaultByAddressQueryHandlerTests
    {
        private readonly Mock<IMediator> _mediatorMock;
        private readonly Mock<IModelAssembler<Vault, VaultDto>> _vaultAssemblerMock;

        private readonly GetVaultByAddressQueryHandler _handler;

        public GetVaultByAddressQueryHandlerTests()
        {
            _mediatorMock = new Mock<IMediator>();
            _vaultAssemblerMock = new Mock<IModelAssembler<Vault, VaultDto>>();
            _handler = new GetVaultByAddressQueryHandler(_mediatorMock.Object, _vaultAssemblerMock.Object);
        }

        [Fact]
        public async Task Handler_RetrieveVaultByAddressQuery_Send()
        {
            // Arrange
            var address = "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm";
            var cancellationToken = new CancellationTokenSource().Token;

            // Act
            await _handler.Handle(new GetVaultByAddressQuery(address), cancellationToken);

            // Assert
            _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveVaultByAddressQuery>(query => query.Vault == address
                                                                                                && query.FindOrThrow), cancellationToken), Times.Once);
        }

        [Fact]
        public async Task Handle_VaultDto_Assemble()
        {
            // Arrange
            var vault = new Vault(5, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", 5, "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", 500, "100000000", 505, 510);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);

            // Act
            await _handler.Handle(new GetVaultByAddressQuery("PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm"), CancellationToken.None);

            // Assert
            _vaultAssemblerMock.Verify(callTo => callTo.Assemble(vault), Times.Once);
        }

        [Fact]
        public async Task Handle_VaultDto_Return()
        {
            // Arrange
            var vaultDto = new VaultDto();

            var vault = new Vault(5, "PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm", 5, "P8zHy2c8Nydkh2r6Wv6K6kacxkDcZyfaLy", 500, "100000000", 505, 510);
            _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveVaultByAddressQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(vault);
            _vaultAssemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<Vault>())).ReturnsAsync(vaultDto);

            // Act
            var response = await _handler.Handle(new GetVaultByAddressQuery("PBWQ38k7iYnkfGPPGgMkN2kwXwmu3wuFYm"), CancellationToken.None);

            // Assert
            response.Should().Be(vaultDto);
        }
    }
}
