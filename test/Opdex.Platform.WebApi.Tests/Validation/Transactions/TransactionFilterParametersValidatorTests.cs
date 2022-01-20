using FluentValidation.TestHelper;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using Opdex.Platform.WebApi.Models.Requests.Transactions;
using Opdex.Platform.WebApi.Validation.Transactions;
using System.Collections.Generic;
using Xunit;

namespace Opdex.Platform.WebApi.Tests.Validation.Transactions;

public class TransactionFilterParametersValidatorTests
{
    private readonly TransactionFilterParametersValidator _validator;

    public TransactionFilterParametersValidatorTests()
    {
        _validator = new TransactionFilterParametersValidator();
    }

    [Theory]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Sender_Invalid(Address sender)
    {
        // Arrange
        var request = new TransactionFilterParameters
        {
            Sender = sender
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Sender);
    }

    [Theory]
    [ClassData(typeof(NullAddressData))]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(ValidNetworkAddressData))]
    public void Sender_Valid(Address sender)
    {
        // Arrange
        var request = new TransactionFilterParameters
        {
            Sender = sender
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Sender);
    }

    [Theory]
    [ClassData(typeof(NullAddressData))]
    [ClassData(typeof(EmptyAddressData))]
    [ClassData(typeof(NonNetworkAddressData))]
    public void Contracts_Items_Invalid(Address address)
    {
        // Arrange
        var request = new TransactionFilterParameters
        {
            Contracts = new List<Address>() { address }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Contracts);
    }

    [Fact]
    public void Contracts_Items_Valid()
    {
        // Arrange
        var request = new TransactionFilterParameters
        {
            Contracts = new List<Address>() { new Address("tVfGTqrToiTU9bfnvD5UDC5ZQVY4oj4jrc") }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Contracts);
    }

    [Theory]
    [InlineData(default(TransactionEventType))]
    [InlineData((TransactionEventType)1000)]
    public void EventTypes_Items_Invalid(TransactionEventType eventType)
    {
        // Arrange
        var request = new TransactionFilterParameters
        {
            EventTypes = new List<TransactionEventType>() { eventType }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.EventTypes);
    }

    [Fact]
    public void EventTypes_Items_Valid()
    {
        // Arrange
        var request = new TransactionFilterParameters
        {
            EventTypes = new List<TransactionEventType>() { TransactionEventType.ClaimPendingMarketOwnershipEvent }
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.EventTypes);
    }

    [Fact]
    public void Limit_Invalid()
    {
        // Arrange
        var request = new TransactionFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit + 1
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldHaveValidationErrorFor(request => request.Limit);
    }

    [Fact]
    public void Limit_Valid()
    {
        // Arrange
        var request = new TransactionFilterParameters
        {
            Limit = Cursor.DefaultMaxLimit
        };

        // Act
        var result = _validator.TestValidate(request);

        // Assert
        result.ShouldNotHaveValidationErrorFor(request => request.Limit);
    }
}