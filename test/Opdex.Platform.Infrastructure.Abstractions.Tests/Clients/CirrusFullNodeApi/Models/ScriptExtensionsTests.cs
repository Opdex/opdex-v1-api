using FluentAssertions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Xunit;

namespace Opdex.Platform.Infrastructure.Abstractions.Tests.Clients.CirrusFullNodeApi.Models;

public class ScriptExtensionsTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("bFc0")]
    [InlineData("c4c3")]
    public void Script_IsSmartContractScript_False(string hex)
    {
        var scriptSig = new ScriptSigDto { Hex = hex };
        scriptSig.IsSmartContractScript().Should().Be(false);
    }

    [Theory]
    [InlineData("c011", ScOpCodeType.OP_CREATECONTRACT)]
    [InlineData("c111", ScOpCodeType.OP_CALLCONTRACT)]
    [InlineData("c211", ScOpCodeType.OP_SPEND)]
    [InlineData("c311", ScOpCodeType.OP_INTERNALCONTRACTTRANSFER)]
    public void Script_IsSmartContractScript_True(string hex, ScOpCodeType expectedScriptType)
    {
        var scriptSig = new ScriptSigDto { Hex = hex };
        scriptSig.IsSmartContractScript().Should().Be(true);
        scriptSig.TryParseSmartContractScriptType(out var scriptType).Should().Be(true);
        scriptType.Should().Be(expectedScriptType);
    }
}
