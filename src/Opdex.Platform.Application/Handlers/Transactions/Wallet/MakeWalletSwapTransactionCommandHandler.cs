using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletSwapTransactionCommandHandler : IRequestHandler<MakeWalletSwapTransactionCommand, string>
    {
        private readonly IMediator _mediator;

        public MakeWalletSwapTransactionCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        public Task<string> Handle(MakeWalletSwapTransactionCommand request, CancellationToken cancellationToken)
        {
            var isCrsOut = !request.TokenOut.HasValue();
            var isCrsIn = !request.TokenIn.HasValue();
            string[] parameters;
            string methodName;
            
            if (isCrsOut)
            {
                methodName = request.TokenInExactAmount ? "SwapExactSrcForCrs" : "SwapSrcForExactCrs";

                if (methodName == "SwapExactSrcForCrs")
                {
                    parameters = new[]
                    {
                        request.TokenInAmount.ToSmartContractParameter(SmartContractParameterType.UInt256),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64), // amountCrsOutMin
                        request.TokenIn.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.To.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64) // deadline
                    };
                }
                else // SwapSrcForExactCrs
                {
                    parameters = new[]
                    {
                        request.TokenOutAmount.ToSmartContractParameter(SmartContractParameterType.UInt64),
                        "10000000000000000000000000000000000".ToSmartContractParameter(SmartContractParameterType.UInt256), // amountSrcInMax
                        request.TokenIn.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.To.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64) // deadline
                    };
                }
            }
            else if (isCrsIn)
            {
                methodName = request.TokenInExactAmount ? "SwapExactCrsForSrc" : "SwapCrsForExactSrc";
                
                if (methodName == "SwapExactCrsForSrc")
                {
                    parameters = new[]
                    {
                        0.ToSmartContractParameter(SmartContractParameterType.UInt256), // amountSrcOutMin
                        request.TokenOut.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.To.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64) // deadline
                    };
                }
                else // SwapCrsForExactSrc
                {
                    parameters = new[]
                    {
                        request.TokenOutAmount.ToSmartContractParameter(SmartContractParameterType.UInt256), // amountSrcOut
                        request.TokenOut.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.To.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64) // deadline
                    };
                }
            }
            else
            {
                methodName = request.TokenInExactAmount ? "SwapExactSrcForSrc" : "SwapSrcForExactSrc";
                
                if (methodName == "SwapExactSrcForSrc")
                {
                    parameters = new[]
                    {
                        request.TokenInAmount.ToSmartContractParameter(SmartContractParameterType.UInt256),
                        request.TokenIn.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt256), // amountSrcOutMin
                        request.TokenOut.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.To.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64) // deadline
                    };
                }
                else // SwapSrcForExactSrc
                {
                    parameters = new[]
                    {
                        "10000000000000000000000000000000000".ToSmartContractParameter(SmartContractParameterType.UInt256), // amountSrcInMax
                        request.TokenIn.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.TokenOutAmount.ToSmartContractParameter(SmartContractParameterType.UInt256),
                        request.TokenOut.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.To.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64) // deadline
                    };
                }
            }
            
            var amount = isCrsIn ? request.TokenInAmount : "0";
            
            var callDto = new SmartContractCallRequestDto(request.Market, request.To, amount, methodName, parameters);
            
            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}