using FluentAssertions;
using Moq;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Balances;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Balances;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.Balances;

public class CallCirrusGetAddressCrsBalanceQueryHandlerTests
{
    private readonly Mock<IBlockStoreModule> _blockStoreModuleMock;
    private readonly CallCirrusGetAddressCrsBalanceQueryHandler _handler;

    public CallCirrusGetAddressCrsBalanceQueryHandlerTests()
    {
        _blockStoreModuleMock = new Mock<IBlockStoreModule>();
        _handler = new CallCirrusGetAddressCrsBalanceQueryHandler(_blockStoreModuleMock.Object);
    }

    [Fact]
    public void CallCirrusGetAddressBalance_ThrowsArgumentNullException_InvalidAddress()
    {
        // Arrange
        var address = Address.Empty;

        // Act
        void Act() => new CallCirrusGetAddressCrsBalanceQuery(address);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("A wallet address must be provided.");
    }

    [Fact]
    public async Task CallCirrusGetAddressBalance_Send_GetWalletAddressesBalances()
    {
        // Arrange
        Address wallet = "P9F65J5Nk6AxVkbJSw9iohk8zniD3waiRf";
        var token = new CancellationTokenSource().Token;

        // Act
        try
        {
            await _handler.Handle(new CallCirrusGetAddressCrsBalanceQuery(wallet), token);
        }
        catch { }

        // Assert
        _blockStoreModuleMock.Verify(callTo =>
                                         callTo.GetWalletAddressesBalances(It.Is<IEnumerable<Address>>(addresses => addresses.Count() == 1
                                                                                                                    && addresses.First() == wallet), token), Times.Once);
    }

    [Fact]
    public async Task CallCirrusGetAddressBalance_FirstAmountFromResponse_Return()
    {
        // Arrange
        Address wallet = "P9F65J5Nk6AxVkbJSw9iohk8zniD3waiRf";
        var token = new CancellationTokenSource().Token;
        const ulong balance = 10;
        var fullNodeResponse = new AddressesBalancesDto
        {
            Balances = new AddressBalanceItemDto[] { new AddressBalanceItemDto { Balance = balance } }
        };

        _blockStoreModuleMock.Setup(callTo => callTo.GetWalletAddressesBalances(It.IsAny<IEnumerable<Address>>(), token)).ReturnsAsync(fullNodeResponse);

        // Act
        var response = await _handler.Handle(new CallCirrusGetAddressCrsBalanceQuery(wallet), token);

        // Assert
        response.Should().Be(balance);
    }

    [Fact]
    public void CallCirrusGetAddressBalance_CaughtException_FindOrThrowTrue_Throws()
    {
        // Arrange
        Address wallet = "P9F65J5Nk6AxVkbJSw9iohk8zniD3waiRf";
        var token = new CancellationTokenSource().Token;
        _blockStoreModuleMock
            .Setup(callTo => callTo.GetWalletAddressesBalances(It.IsAny<IEnumerable<Address>>(), token))
            .Throws(new Exception());

        // Act
        // Assert
        _handler
            .Invoking(h => h.Handle(new CallCirrusGetAddressCrsBalanceQuery(wallet, findOrThrow: true), token))
            .Should()
            .ThrowAsync<Exception>();
    }

    [Fact]
    public async Task CallCirrusGetAddressBalance_CaughtException_FindOrThrowFalse_ReturnsZero()
    {
        // Arrange
        Address wallet = "P9F65J5Nk6AxVkbJSw9iohk8zniD3waiRf";
        var token = new CancellationTokenSource().Token;
        _blockStoreModuleMock
            .Setup(callTo => callTo.GetWalletAddressesBalances(It.IsAny<IEnumerable<Address>>(), token))
            .Throws(new Exception());

        // Act
        var response = await _handler.Handle(new CallCirrusGetAddressCrsBalanceQuery(wallet, findOrThrow: false), token);

        // Assert
        response.Should().Be(0);
    }
}