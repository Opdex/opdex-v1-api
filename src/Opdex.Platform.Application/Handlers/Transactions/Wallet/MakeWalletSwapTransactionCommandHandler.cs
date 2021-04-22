using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;

namespace Opdex.Platform.Application.Handlers.Transactions.Wallet
{
    public class MakeWalletSwapTransactionCommandHandler : IRequestHandler<MakeWalletSwapTransactionCommand, string>
    {
        private readonly IMediator _mediator;
        private readonly OpdexConfiguration _opdexConfiguration;

        public MakeWalletSwapTransactionCommandHandler(IMediator mediator, IOptions<OpdexConfiguration> opdexConfiguration)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _opdexConfiguration = opdexConfiguration.Value ?? throw new ArgumentNullException(nameof(opdexConfiguration));
        }
        
        public Task<string> Handle(MakeWalletSwapTransactionCommand request, CancellationToken cancellationToken)
        {
            var isCrsOut = !request.TokenOut.HasValue();
            var isCrsIn = !request.TokenIn.HasValue();
            var methodName = string.Empty;
            var parameters = new List<string>();
            
            if (isCrsOut)
            {
                methodName = request.TokenInExactAmount ? "SwapExactSrcForCrs" : "SwapSrcForExactCrs";

                if (methodName == "SwapExactSrcForCrs")
                {
                    parameters = new List<string>
                    {
                        $"12#{request.TokenInAmount}", // amountSrcIn
                        "7#0", // amountCrsOutMin
                        $"9#{request.TokenIn}", // token
                        $"9#{request.To}", // to
                        "7#0", // deadline
                    };
                }
                else
                {
                    parameters = new List<string>
                    {
                        $"7#{request.TokenOutAmount}", // amountCrsOut
                        "12#10000000000000000000000000000000000", // amountSrcInMax
                        $"9#{request.TokenIn}", // token
                        $"9#{request.To}", // to
                        "7#0", // deadline
                    };
                }
            }
            else if (isCrsIn)
            {
                methodName = request.TokenInExactAmount ? "SwapExactCrsForSrc" : "SwapCrsForExactSrc";
                
                if (methodName == "SwapExactCrsForSrc")
                {
                    parameters = new List<string>
                    {
                        "12#0", // amountSrcOutMin
                        $"9#{request.TokenOut}", // token
                        $"9#{request.To}", // to
                        "7#0", // deadline
                    };
                }
                else
                {
                    parameters = new List<string>
                    {
                        $"12#{request.TokenOutAmount}", // amountSrcOut
                        $"9#{request.TokenOut}", // token
                        $"9#{request.To}", // to
                        "7#0", // deadline
                    };
                }
            }
            else
            {
                methodName = request.TokenInExactAmount ? "SwapExactSrcForSrc" : "SwapSrcForExactSrc";
                
                if (methodName == "SwapExactSrcForSrc")
                {
                    
                }
                else
                {
                    
                }
            }
            
            var amount = isCrsIn ? request.TokenInAmount : "0";
            
            var callDto = new SmartContractCallRequestDto(_opdexConfiguration.ControllerContract, request.To, amount, methodName, parameters.ToArray());
            
            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}