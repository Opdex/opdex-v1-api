using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    /// <summary>
    /// A class containing the necessary parameters to perform a local smart contract method call request.
    /// </summary>
    public class LocalCallRequestDto
    {
        public LocalCallRequestDto(Address address, string methodName, ulong? blockHeight = null)
            : this(address, methodName, Array.Empty<string>(), blockHeight) { }

        public LocalCallRequestDto(Address address, string methodName, string[] parameters, ulong? blockHeight = null)
            : this(address, address, methodName, parameters, blockHeight) { }

        public LocalCallRequestDto(Address address, Address sender, string methodName, ulong? blockHeight = null)
            : this(address, sender, methodName, Array.Empty<string>(), blockHeight) { }

        public LocalCallRequestDto(Address address, Address sender, string methodName, string[] parameters, ulong? blockHeight = null, FixedDecimal? amount = null)
        {
            Amount = amount ?? FixedDecimal.Zero;
            GasPrice = 100;
            GasLimit = 250_000;
            ContractAddress = address;
            Sender = sender;
            MethodName = methodName;
            Parameters = parameters;
            BlockHeight = blockHeight;
        }

        /// <summary>
        /// The address of the smart contract containing the method.
        /// </summary>
        public Address ContractAddress { get; set; }

        /// <summary>
        /// The name of the method to call.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// The amount of STRAX (or sidechain coin) to send to the smart contract address.
        /// No funds are actually sent, but the Amount field allows
        /// certain scenarios, where the funds sent dictates the result, to be checked.
        /// </summary>
        public FixedDecimal Amount { get; set; }

        /// <summary>
        /// The gas price to use. This is used to calculate the expected expenditure
        /// if the method is run by a miner mining a call transaction rather than
        /// locally.
        /// </summary>
        public ulong GasPrice { get; set; }

        /// <summary>
        /// The maximum amount of gas that can be spent executing this transaction.
        /// Although the gas expenditure is theoretical rather than actual,
        /// this limit cannot be exceeded even when the method is run locally.
        /// </summary>
        public ulong GasLimit { get; set; }

        /// <summary>
        /// A wallet address containing the funds to cover transaction fees, gas, and any funds specified in the
        /// Amount field.
        /// Note that because the method call is local no funds are spent. However, the concept of the sender address
        /// is still valid and may need to be checked.
        /// For example, some methods, such as a withdrawal method on an escrow smart contract, should only be executed
        /// by the deployer, and in this case, it is the Sender address that identifies the deployer.
        /// </summary>
        public Address Sender { get; set; }

        /// <summary>
        /// An array of encoded strings containing the parameters (and their type) to pass to the smart contract
        /// method when it is called. More information on the
        /// format of a parameter string is available
        /// <a target="_blank" href="https://academy.stratisplatform.com/SmartContracts/working-with-contracts.html#parameter-serialization">here</a>.
        /// </summary>
        public string[] Parameters { get; set; }

        /// <summary>
        /// Block height at which to make the call. Setting this makes a point-in-time local call.
        /// </summary>
        public ulong? BlockHeight { get; }
    }
}
