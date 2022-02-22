using FluentAssertions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests.Clients.CirrusFullNodeApi.Models;

public class BlockReceiptDtoTests
{
    [Fact]
    public void SmartContractCallTx_NonSmartContractTransfers_Exclude()
    {
        // Arrange
        var receipt = new BlockReceiptDto
        {
            Transactions = new List<RawTransactionDto>
            {
                new()
                {
                    Hash = new Sha256(43859048509235),
                    Vin = new VInDto[]
                    {
                        new() { ScriptSig = new ScriptSigDto { Hex = "11832098b7482f75d13d8a284a7984" }}
                    },
                    Vout = new VOutDto[]
                    {
                        new() { ScriptPubKey = new ScriptPubKeyDto { Hex = "0ca3b3828579e89a98f98b" }}
                    }
                }
            }
        };

        // Act
        var smartContractTxs = receipt.SmartContractTxs;

        // Assert
        smartContractTxs.Count().Should().Be(0);
    }

    [Fact]
    public void SmartContractTxs_InternalSmartContractTransfers_Exclude()
    {
        // Arrange
        var receipt = new BlockReceiptDto
        {
            Transactions = new List<RawTransactionDto>
            {
                new()
                {
                    Hash = new Sha256(43859048509235),
                    Vin = new VInDto[]
                    {
                        new() { ScriptSig = new ScriptSigDto { Hex = "c3832098b7482f75d13d8a284a7984" }}
                    },
                    Vout = new VOutDto[]
                    {
                        new() { ScriptPubKey = new ScriptPubKeyDto { Hex = "c0a00fca3b3828579e89a98f98b" }}
                    }
                }
            }
        };

        // Act
        var smartContractTxs = receipt.SmartContractTxs;

        // Assert
        smartContractTxs.Count().Should().Be(0);
    }

    [Fact]
    public void SmartContractTxs_External_Include()
    {
        // Arrange
        var receipt = new BlockReceiptDto
        {
            Transactions = new List<RawTransactionDto>
            {
                new()
                {
                    Hash = new Sha256(43859048509235),
                    Vin = new VInDto[]
                    {
                        new() { ScriptSig = new ScriptSigDto { Hex = "01832098b7482f75d13d8a284a7984" }}
                    },
                    Vout = new VOutDto[]
                    {
                        new() { ScriptPubKey = new ScriptPubKeyDto { Hex = "c0a00fca3b3828579e89a98f98b" }}
                    }
                }
            }
        };

        // Act
        var smartContractTxs = receipt.SmartContractTxs;

        // Assert
        smartContractTxs.Count(hash => hash == new Sha256(43859048509235)).Should().Be(1);
    }
}
