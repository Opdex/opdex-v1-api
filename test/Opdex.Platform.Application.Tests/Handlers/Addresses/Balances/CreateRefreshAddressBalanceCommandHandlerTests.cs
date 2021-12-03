using FluentAssertions;
using MediatR;
using Moq;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Addresses.Balances;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Domain.Models.Blocks;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Application.Tests.Handlers.Addresses.Balances;

public class CreateRefreshAddressBalanceCommandHandlerTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IModelAssembler<AddressBalance, AddressBalanceDto>> _assemblerMock;
    private readonly CreateRefreshAddressBalanceCommandHandler _handler;

    public CreateRefreshAddressBalanceCommandHandlerTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _assemblerMock = new Mock<IModelAssembler<AddressBalance, AddressBalanceDto>>();
        _handler = new CreateRefreshAddressBalanceCommandHandler(_mediatorMock.Object, _assemblerMock.Object);
    }

    [Fact]
    public async Task Handle_RetrieveLatestBlock_Send()
    {
        // Arrange
        var command = new CreateRefreshAddressBalanceCommand(new Address("t8kAxvbaFzpPTWDE8f2bdgV7V1276xu2VH"), new Address("tCVeVYnfcTjtT6UbUFF9SsUYgfT5QUYtGr"));
        var cancellationToken = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(command, cancellationToken);
        }
        catch (Exception) { }

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_UpdateAddressBalance_Send()
    {
        // Arrange
        var command = new CreateRefreshAddressBalanceCommand(new Address("t8kAxvbaFzpPTWDE8f2bdgV7V1276xu2VH"), new Address("tCVeVYnfcTjtT6UbUFF9SsUYgfT5QUYtGr"));
        var cancellationToken = new CancellationTokenSource().Token;

        var block = new Block(500, Sha256.Parse("21aaa0f833c4a7f81bd9e8862388733a3a67a1b532c077f0d23503abe0b2f3d8"), DateTime.UtcNow, DateTime.UtcNow);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(block);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<CreateAddressBalanceCommand>(c => c.Token == command.Token
                                                                                           && c.Wallet == command.Wallet
                                                                                           && c.Block == block.Height), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_RetrieveUpdatedAddressBalance_Send()
    {
        // Arrange
        var command = new CreateRefreshAddressBalanceCommand(new Address("t8kAxvbaFzpPTWDE8f2bdgV7V1276xu2VH"), new Address("tCVeVYnfcTjtT6UbUFF9SsUYgfT5QUYtGr"));
        var cancellationToken = new CancellationTokenSource().Token;

        var block = new Block(500, Sha256.Parse("21aaa0f833c4a7f81bd9e8862388733a3a67a1b532c077f0d23503abe0b2f3d8"), DateTime.UtcNow, DateTime.UtcNow);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(block);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _mediatorMock.Verify(callTo => callTo.Send(It.Is<RetrieveAddressBalanceByOwnerAndTokenQuery>(c => c.TokenAddress == command.Token
                                                                                                          && c.Owner == command.Wallet), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task Handle_AssembleDto_Send()
    {
        // Arrange
        var command = new CreateRefreshAddressBalanceCommand(new Address("t8kAxvbaFzpPTWDE8f2bdgV7V1276xu2VH"), new Address("tCVeVYnfcTjtT6UbUFF9SsUYgfT5QUYtGr"));
        var cancellationToken = new CancellationTokenSource().Token;

        var block = new Block(500, Sha256.Parse("21aaa0f833c4a7f81bd9e8862388733a3a67a1b532c077f0d23503abe0b2f3d8"), DateTime.UtcNow, DateTime.UtcNow);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(block);
        var addressBalance = new AddressBalance(10, 10, "t8kAxvbaFzpPTWDE8f2bdgV7V1276xu2VH", 10000000000, 500, 505);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalanceByOwnerAndTokenQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(addressBalance);

        // Act
        await _handler.Handle(command, cancellationToken);

        // Assert
        _assemblerMock.Verify(callTo => callTo.Assemble(addressBalance), Times.Once);
    }

    [Fact]
    public async Task Handle_AssembleDto_Return()
    {
        // Arrange
        var command = new CreateRefreshAddressBalanceCommand(new Address("t8kAxvbaFzpPTWDE8f2bdgV7V1276xu2VH"), new Address("tCVeVYnfcTjtT6UbUFF9SsUYgfT5QUYtGr"));
        var cancellationToken = new CancellationTokenSource().Token;

        var block = new Block(500, Sha256.Parse("21aaa0f833c4a7f81bd9e8862388733a3a67a1b532c077f0d23503abe0b2f3d8"), DateTime.UtcNow, DateTime.UtcNow);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveLatestBlockQuery>(), It.IsAny<CancellationToken>())).ReturnsAsync(block);
        _mediatorMock.Setup(callTo => callTo.Send(It.IsAny<RetrieveAddressBalanceByOwnerAndTokenQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AddressBalance(10, 10, "t8kAxvbaFzpPTWDE8f2bdgV7V1276xu2VH", 10000000000, 500, 505));
        var addressBalanceDto = new AddressBalanceDto();
        _assemblerMock.Setup(callTo => callTo.Assemble(It.IsAny<AddressBalance>())).ReturnsAsync(addressBalanceDto);

        // Act
        var response = await _handler.Handle(command, cancellationToken);

        // Assert
        response.Should().Be(addressBalanceDto);
    }
}