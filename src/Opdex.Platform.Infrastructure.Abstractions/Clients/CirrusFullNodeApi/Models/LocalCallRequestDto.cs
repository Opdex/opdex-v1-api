namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    /// <summary>
    /// A class containing the necessary parameters to perform a local smart contract method call request.
    /// </summary>
    public class LocalCallRequestDto
    {
        public LocalCallRequestDto(string address, string sender, string methodName, string[] parameters)
        {
            Amount = "0.00";
            GasPrice = 100;
            GasLimit = 100_000;
            ContractAddress = address;
            Sender = sender;
            MethodName = methodName;
            Parameters = parameters;
        }

        /// <summary>
        /// The address of the smart contract containing the method.
        /// </summary>
        public string ContractAddress { get; set; }

        /// <summary>
        /// The name of the method to call.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// The amount of STRAT (or sidechain coin) to send to the smart contract address. 
        /// No funds are actually sent, but the Amount field allows
        /// certain scenarios, where the funds sent dictates the result, to be checked.
        /// </summary>
        public string Amount { get; set; }

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
        public string Sender { get; set; }

        /// <summary>
        /// An array of encoded strings containing the parameters (and their type) to pass to the smart contract
        /// method when it is called. More information on the
        /// format of a parameter string is available
        /// <a target="_blank" href="https://academy.stratisplatform.com/SmartContracts/working-with-contracts.html#parameter-serialization">here</a>.
        /// </summary> 
        public string[] Parameters { get; set; }
    }
}