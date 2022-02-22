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
     * Smart contracts can be thought of as having external and internal transfers/calls.
     * - External calls are done by a user to a smart contract method.
     * - Internal calls are created as a result of the contract execution.
     *
     * For example, in the example block: https://chainz.cryptoid.info/cirrus-test/block.dws?3434decc0e31d8e0a5cccb662b73ac225beaf259c7e8eceec69f83c71c69f7e2.htm
     *  - The 1st transaction is the coinbase
     *  - The 2nd is an external smart contract transactions
     *  - The 3rd an internal transaction.
     *
     * Internal transactions are created by the contract executor, see https://github.com/stratisproject/StratisFullNode/blob/master/src/Stratis.SmartContracts.CLR/ContractExecutor.cs#L90
     *
     */
    public bool IsExternalSmartContractTransfer => Vin.All(v => v.ScriptSig is not null && !v.ScriptSig.IsSmartContractScript());

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
    public bool IsSmartContractCall => Vout.Any(v => v.ScriptPubKey.TryParseSmartContractScriptType(out var scriptType) && scriptType == ScOpCodeType.OP_CALLCONTRACT);
    public bool IsSmartContractCreate => Vout.Any(v => v.ScriptPubKey.TryParseSmartContractScriptType(out var scriptType) && scriptType == ScOpCodeType.OP_CREATECONTRACT);
}

public class VInDto
{
    public ScriptSigDto ScriptSig { get; set; }
}

public class VOutDto
{
    public ScriptPubKeyDto ScriptPubKey { get; set; }
}

public abstract class ScriptDto
{
    public string Hex { get; set; }
}

public class ScriptSigDto : ScriptDto
{
}

public class ScriptPubKeyDto : ScriptDto
{
    public ScriptPubKeyDto()
    {
        Addresses = Array.Empty<Address>();
    }

    public Address[] Addresses { get; set; }
}
