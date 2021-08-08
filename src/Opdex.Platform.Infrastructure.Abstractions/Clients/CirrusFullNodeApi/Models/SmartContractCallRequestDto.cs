namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class SmartContractCallRequestDto
    {
        public SmartContractCallRequestDto(string address, string walletName, string walletAddress, string walletPassword, string amount, 
            string methodName, string[] parameters = null)
        {
            Amount = amount;
            ContractAddress = address;
            WalletName = walletName;
            Sender = walletAddress;
            Password = walletPassword;
            MethodName = methodName;
            Parameters = parameters;
        }

        public string AccountName => "account 0";
        public string FeeAmount => ".001";
        public ulong GasPrice => 100;
        public ulong GasLimit => 250_000;
        public string Password { get; set; }
        public string WalletName { get; set; }
        public string ContractAddress { get; set; }
        public string MethodName { get; set; }
        public string Amount { get; set; }
        public string Sender { get; set; }
        public string[] Parameters { get; set; }
    }
}