using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.TransactionLogs;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using Opdex.Platform.Domain.Models.Transactions;
using System;
using System.Collections.Generic;
using System.Dynamic;
using Xunit;

namespace Opdex.Platform.Domain.Tests.Models.Transactions;

public class TransactionQuoteTests
{
    [Fact]
    public void CreateNew_TransactionQuote_InvalidGasUsed_ThrowArgumentOutOfRangeException()
    {
        // Arrange
        Address result = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        const uint gasUsed = 0;
        var request = GetTransactionQuoteRequest();

        // Act
        void Act() => new TransactionQuote(result, string.Empty, gasUsed, Array.Empty<TransactionLog>(), request);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act).Message.Should().Contain($"{nameof(gasUsed)} must be greater than 0.");
    }

    [Fact]
    public void CreateNew_TransactionQuote_InvalidRequest_ThrowArgumentNullException()
    {
        // Arrange
        Address result = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        const uint gasUsed = 10000;
        TransactionQuoteRequest request = null;


        // Act
        void Act() => new TransactionQuote(result, string.Empty, gasUsed, Array.Empty<TransactionLog>(), request);

        // Assert
        Assert.Throws<ArgumentNullException>(Act).Message.Should().Contain($"{nameof(request)} must not be null.");
    }

    [Fact]
    public void CreateNew_Successful_TransactionQuote()
    {
        // Arrange
        const string result = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        const uint gasUsed = 10000;
        var request = GetTransactionQuoteRequest();

        Address address = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        const int sortOrder = 1;
        dynamic txLog = new ExpandoObject();
        txLog.owner = "PMU9EjmivLgqqARwmH1iT1GLsMroh6zXXN";
        txLog.spender = "PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh";
        txLog.amount = "1";
        txLog.oldAmount = "0";

        var logs = new List<TransactionLog> { new ApprovalLog(txLog, address, sortOrder) };

        // Act
        var response = new TransactionQuote(result, string.Empty, gasUsed, logs, request);

        // Assert
        response.Result.Should().Be(result);
        response.GasUsed.Should().Be(gasUsed);
        response.Request.Should().BeEquivalentTo(request);
        response.Logs.Should().BeEquivalentTo(logs);
    }

    [Fact]
    public void CreateNew_TransactionQuote_WithErrors()
    {
        // Arrange
        const string error = "Error";
        const uint gasUsed = 10000;
        var request = GetTransactionQuoteRequest();

        // Act
        var response = new TransactionQuote(null, error, gasUsed, Array.Empty<TransactionLog>(), request);

        // Assert
        response.Result.Should().BeNull();
        response.Error.Should().Be(error);
        response.GasUsed.Should().Be(gasUsed);
        response.Request.Should().BeEquivalentTo(request);
    }

    private static TransactionQuoteRequest GetTransactionQuoteRequest()
    {
        Address sender = "PWcdTKU64jVFCDoHJgUKz633jsy1XTenAy";
        Address to = "PBSH3FTVne6gKiSgVBL4NRTJ31QmGShjMy";
        FixedDecimal amount = FixedDecimal.Zero;
        const string method = "Swap";
        const string callback = "https://dev-api.opdex.com/transactions";

        return new TransactionQuoteRequest(sender, to, amount, method, callback);
    }
}