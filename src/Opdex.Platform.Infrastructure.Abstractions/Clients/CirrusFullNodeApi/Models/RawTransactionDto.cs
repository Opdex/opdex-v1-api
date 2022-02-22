using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;
using System.Globalization;
using System.Linq;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;

public class RawTransactionDto
{
    public RawTransactionDto()
    {
        Vout = Array.Empty<VOutDto>();
    }

    public Sha256 Hash { get; set; }

    public VInDto[] Vin { get; set; }

    public VOutDto[] Vout { get; set; }

    /*
     *
     * External transfers scriptPubKey include OP_CREATECONTRACT or OP_CALLCONTRACT (nonstandard)
     * Internal transfers scriptSig include OP_SPEND (nonstandard)
     * Internal transfers scriptPubKey include OP_INTERNALCONTRACTTRANSFER (nonstandard)
     * Refund TXs are P2PKH
     *
     * Coinbase and SegWit transactions do not contain a script sig in the input. Therefore, we filter these out.
     * We filter out transactions where the script sig uses nonstandard smart contract OP codes (internal transfers).
     * Finally, we return transactions where the script pub key uses nonstandard smart contract OP codes.
     *
     */
    public bool IsSmartContractCall => Vout.Any(v => v.ScriptPubKey.SmartContractScriptType == ScOpcodeType.OP_CALLCONTRACT);
    public bool IsSmartContractCreate => Vout.Any(v => v.ScriptPubKey.SmartContractScriptType == ScOpcodeType.OP_CREATECONTRACT);

    public bool IsExternalSmartContractTransfer => Vin.All(v => v.ScriptSig is not null && !v.ScriptSig.IsSmartContractScript);
}

public class VInDto
{
    public ScriptSigDto ScriptSig { get; set; }
}

public class VOutDto
{
    public ScriptPubKeyDto ScriptPubKey { get; set; }
}

public class ScriptSigDto
{
    public string Hex { get; set; }

    public bool IsSmartContractScript => SmartContractScriptType.IsValid();

    // For a smart contract script, we can get the type from the first byte
    public ScOpcodeType SmartContractScriptType
    {
        get
        {
            if (Hex is null) return default;
            var isValidScript = byte.TryParse(Hex[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var firstByte);
            if (!isValidScript) return default;
            var smartContractScriptType = ((ScOpcodeType)firstByte);
            var isValidSmartContractScript = smartContractScriptType.IsValid();
            return isValidSmartContractScript ? smartContractScriptType : default;
        }
    }
}

public class ScriptPubKeyDto
{
    public ScriptPubKeyDto()
    {
        Addresses = Array.Empty<Address>();
    }

    public Address[] Addresses { get; set; }

    public string Hex { get; set; }

    public bool IsSmartContractScript => SmartContractScriptType.IsValid();

    // For a smart contract script, we can get the type from the first byte
    public ScOpcodeType SmartContractScriptType
    {
        get
        {
            if (Hex is null) return default;
            var isValidScript = byte.TryParse(Hex[..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var firstByte);
            if (!isValidScript) return default;
            var smartContractScriptType = ((ScOpcodeType)firstByte);
            var isValidSmartContractScript = smartContractScriptType.IsValid();
            return isValidSmartContractScript ? smartContractScriptType : default;
        }
    }
}
