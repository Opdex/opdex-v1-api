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
        var smartContractCallTxs = receipt.SmartContractCallTxs;

        // Assert
        smartContractCallTxs.Count().Should().Be(0);
    }

    [Fact]
    public void SmartContractCreateTx_NonSmartContractTransfers_Exclude()
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
        var smartContractCreateTxs = receipt.SmartContractCreateTxs;

        // Assert
        smartContractCreateTxs.Count().Should().Be(0);
    }

    [Fact]
    public void SmartContractCallTx_InternalSmartContractTransfers_Exclude()
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
        var smartContractCallTxs = receipt.SmartContractCallTxs;

        // Assert
        smartContractCallTxs.Count().Should().Be(0);
    }

    [Fact]
    public void SmartContractCreateTx_InternalSmartContractTransfers_Exclude()
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
        var smartContractCreateTxs = receipt.SmartContractCreateTxs;

        // Assert
        smartContractCreateTxs.Count().Should().Be(0);
    }

    [Fact]
    public void SmartContractCallTx_External_Include()
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
                        new() { ScriptPubKey = new ScriptPubKeyDto { Hex = "c1a00fca3b3828579e89a98f98b" }}
                    }
                }
            }
        };

        // Act
        var smartContractCallTx = receipt.SmartContractCallTxs;
        var smartContractCreateTx = receipt.SmartContractCreateTxs;

        // Assert
        smartContractCallTx.Count(hash => hash == new Sha256(43859048509235)).Should().Be(1);
        smartContractCreateTx.Count().Should().Be(0);
    }

    [Fact]
    public void SmartContractCreateTx_External_Include()
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
        var smartContractCallTx = receipt.SmartContractCallTxs;
        var smartContractCreateTx = receipt.SmartContractCreateTxs;

        // Assert
        smartContractCreateTx.Count(hash => hash == new Sha256(43859048509235)).Should().Be(1);
        smartContractCallTx.Count().Should().Be(0);
    }
}
