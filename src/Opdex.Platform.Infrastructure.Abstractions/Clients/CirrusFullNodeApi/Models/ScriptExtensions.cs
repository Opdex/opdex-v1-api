using Opdex.Platform.Common.Extensions;
using System.Globalization;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

public static class ScriptExtensions
{
    /// <summary>
    /// Attempts to parse a smart contract script type
    /// </summary>
    /// <param name="script">Transaction I/O script</param>
    /// <param name="type">Smart contract OP code</param>
    /// <returns>True if a smart contract script type could be parsed; otherwise false</returns>
    public static bool TryParseSmartContractScriptType(this ScriptDto script, out ScOpCodeType type)
    {
        type = default;

        if (script.Hex is null) return false;

        // we can get the SC script type by parsing the first byte
        var isValidScript = byte.TryParse(script.Hex[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var firstByte);

        if (!isValidScript) return false;

        var smartContractScriptType = ((ScOpCodeType)firstByte);
        var isValidSmartContractScript = smartContractScriptType.IsValid();

        if (isValidSmartContractScript) type = smartContractScriptType;

        return isValidSmartContractScript;
    }

    public static bool IsSmartContractScript(this ScriptDto script) => TryParseSmartContractScriptType(script, out _);
}
