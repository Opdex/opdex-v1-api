using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.CirrusFullNodeApiTests.Handlers.SmartContracts;

public class CallCirrusGetSmartContractPropertyQueryHandlerTests
{
    private readonly Mock<ISmartContractsModule> _smartContractsModuleMock;
    private readonly CallCirrusGetSmartContractPropertyQueryHandler _handler;

    public CallCirrusGetSmartContractPropertyQueryHandlerTests()
    {
        _smartContractsModuleMock = new Mock<ISmartContractsModule>();
        _handler = new CallCirrusGetSmartContractPropertyQueryHandler(_smartContractsModuleMock.Object,
                                                                      NullLogger<CallCirrusGetSmartContractPropertyQueryHandler>.Instance);
    }

    [Fact]
    public void CallCirrusGetSmartContractPropertyQuery_InvalidContract_ThrowArgumentNullException()
    {
        // Arrange
        const string stateKey = MarketDeployerConstants.StateKeys.Owner;
        const SmartContractParameterType propertyType = SmartContractParameterType.Address;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CallCirrusGetSmartContractPropertyQuery(null, stateKey, propertyType, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Contract address must be provided.");
    }

    [Fact]
    public void CallCirrusGetSmartContractPropertyQuery_InvalidStateKey_ThrowArgumentNullException()
    {
        // Arrange
        Address contract = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
        const SmartContractParameterType propertyType = SmartContractParameterType.Address;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CallCirrusGetSmartContractPropertyQuery(contract, null, propertyType, blockHeight);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain("Property state key value must be provided.");
    }

    [Fact]
    public void CallCirrusGetSmartContractPropertyQuery_InvalidPropertyType_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        Address contract = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
        const string stateKey = MarketDeployerConstants.StateKeys.Owner;
        const SmartContractParameterType propertyType = 0;
        const ulong blockHeight = 10;

        // Act
        void Act() => new CallCirrusGetSmartContractPropertyQuery(contract, stateKey, propertyType, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Property type must be a valid value.");
    }

    [Fact]
    public void CallCirrusGetSmartContractPropertyQuery_InvalidBlockHeight_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        Address contract = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
        const string stateKey = MarketDeployerConstants.StateKeys.Owner;
        const SmartContractParameterType propertyType = SmartContractParameterType.Address;
        const ulong blockHeight = 0;

        // Act
        void Act() => new CallCirrusGetSmartContractPropertyQuery(contract, stateKey, propertyType, blockHeight);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain("Block height must be greater than zero.");
    }

    [Fact]
    public async Task CallCirrusGetSmartContractPropertyQuery_Sends_GetContractStorageAsync()
    {
        // Arrange
        Address contract = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
        const string stateKey = MarketDeployerConstants.StateKeys.Owner;
        const SmartContractParameterType propertyType = SmartContractParameterType.Address;
        const ulong blockHeight = 10;

        // Act
        try
        {
            await _handler.Handle(new CallCirrusGetSmartContractPropertyQuery(contract, stateKey, propertyType, blockHeight), CancellationToken.None);
        }
        catch { }

        // Assert
        _smartContractsModuleMock.Verify(callTo => callTo.GetContractStorageAsync(contract, stateKey, propertyType,
                                                                                  blockHeight, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CallCirrusGetSmartContractPropertyQuery_Returns()
    {
        // Arrange
        Address contract = "PH1iT1GLsMroh6zXXNMU9EjmivLgqqARwm";
        const string stateKey = MarketDeployerConstants.StateKeys.Owner;
        const SmartContractParameterType propertyType = SmartContractParameterType.Address;
        const ulong blockHeight = 10;

        Address expectedResponse = "PsMroh6zXXNMU9EjmivLgqqARwmH1iT1GL";

        _smartContractsModuleMock.Setup(callTo => callTo.GetContractStorageAsync(contract, stateKey, propertyType, blockHeight, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse.ToString());

        // Act
        var response = await _handler.Handle(new CallCirrusGetSmartContractPropertyQuery(contract, stateKey, propertyType, blockHeight), CancellationToken.None);

        // Assert
        response.Value.Should().Be(expectedResponse.ToString());
        response.Type.Should().Be(propertyType);
    }
}