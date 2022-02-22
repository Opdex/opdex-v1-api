using FluentAssertions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Xunit;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests.Clients.CirrusFullNodeApi.Models;

public class RawTransactionDtoTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("bFc0")]
    [InlineData("c4c3")]
    public void ScriptSig_IsSmartContractScript_False(string hex)
    {
        var scriptSig = new ScriptSigDto { Hex = hex };
        scriptSig.IsSmartContractScript.Should().Be(false);
        scriptSig.SmartContractScriptType.Should().Be(default);
    }

    [Theory]
    [InlineData("c011", ScOpcodeType.OP_CREATECONTRACT)]
    [InlineData("c111", ScOpcodeType.OP_CALLCONTRACT)]
    [InlineData("c211", ScOpcodeType.OP_SPEND)]
    [InlineData("c311", ScOpcodeType.OP_INTERNALCONTRACTTRANSFER)]
    public void ScriptSig_IsSmartContractScript_True(string hex, ScOpcodeType scriptType)
    {
        var scriptSig = new ScriptSigDto { Hex = hex };
        scriptSig.IsSmartContractScript.Should().Be(true);
        scriptSig.SmartContractScriptType.Should().Be(scriptType);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("bFc0")]
    [InlineData("c4c3")]
    public void ScriptPubKey_IsSmartContractScript_False(string hex)
    {
        var scriptPubKey = new ScriptPubKeyDto { Hex = hex };
        scriptPubKey.IsSmartContractScript.Should().Be(false);
        scriptPubKey.SmartContractScriptType.Should().Be(default);
    }

    [Theory]
    [InlineData("c011", ScOpcodeType.OP_CREATECONTRACT)]
    [InlineData("c111", ScOpcodeType.OP_CALLCONTRACT)]
    [InlineData("c211", ScOpcodeType.OP_SPEND)]
    [InlineData("c311", ScOpcodeType.OP_INTERNALCONTRACTTRANSFER)]
    public void ScriptPubKey_IsSmartContractScript_True(string hex, ScOpcodeType scriptType)
    {
        var scriptPubKey = new ScriptPubKeyDto { Hex = hex };
        scriptPubKey.IsSmartContractScript.Should().Be(true);
        scriptPubKey.SmartContractScriptType.Should().Be(scriptType);
    }
}
