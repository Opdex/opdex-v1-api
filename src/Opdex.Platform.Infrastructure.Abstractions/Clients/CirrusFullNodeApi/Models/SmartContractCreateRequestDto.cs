using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class SmartContractCreateRequestDto
    {
        public SmartContractCreateRequestDto(string contractCode, Address sender, string[] parameters, string walletName = "cirrusdev", string password = "password")
        {
            ContractCode = contractCode;
            Sender = sender;
            Parameters = parameters;
            WalletName = walletName;
            Password = password;
        }

        public string AccountName => "account 0";
        public string FeeAmount => ".001";
        public ulong GasPrice => 100;
        public ulong GasLimit => 250_000;
        public string Password { get; set; }
        public string WalletName { get; set; }
        public string Amount => "0.00";
        public string ContractCode { get; set; }
        public Address Sender { get; set; }
        public string[] Parameters { get; set; }
    }
}
