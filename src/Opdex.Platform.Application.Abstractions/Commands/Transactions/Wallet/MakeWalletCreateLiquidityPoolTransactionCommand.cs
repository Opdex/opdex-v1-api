using MediatR;

namespace Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet
{
    public class MakeWalletCreateLiquidityPoolTransactionCommand : IRequest<string>
    {
        public MakeWalletCreateLiquidityPoolTransactionCommand(string token, string sender)
        {
            Token = token;
            Sender = sender;
        }
        
        public string Token { get; }
        public string Sender { get; }
    }
}