namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models
{
    public class SmartContractCallRequestDto
    {
        public SmartContractCallRequestDto(string address, string sender, string amount, string methodName, string[] parameters)
        {
            Amount = amount;
            ContractAddress = address;
            Sender = sender;
            MethodName = methodName;
            Parameters = parameters;
        }

        public string AccountName => "account 0";
        public string FeeAmount => ".001";
        public ulong GasPrice => 100;
        public ulong GasLimit => 250_000;
        public string Password => "password";
        public string WalletName => "cirrusdev";
        public string ContractAddress { get; set; }
        public string MethodName { get; set; }
        public string Amount { get; set; }
        public string Sender { get; set; }
        public string[] Parameters { get; set; }
    }
}