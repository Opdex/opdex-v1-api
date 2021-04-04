using MediatR;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletApproveAllowanceTransactionCommand : IRequest<string>
    {
        public MakeWalletApproveAllowanceTransactionCommand(string token, string amount, string owner, string spender)
        {
            Token = token;
            Amount = amount;
            Owner = owner;
            Spender = spender;
        }
        
        public string Token { get; }
        public string Amount { get; }
        public string Owner { get; }
        public string Spender { get; }
    }
}