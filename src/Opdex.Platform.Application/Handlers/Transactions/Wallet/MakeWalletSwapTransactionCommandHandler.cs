using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Commands;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Application.Abstractions.Commands.Transactions.Wallet;
using Opdex.Platform.Common;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Enums;
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
            var isCrsOut = request.TokenOut == TokenConstants.Cirrus.Address;
            var isCrsIn = request.TokenIn == TokenConstants.Cirrus.Address;
            string[] parameters;
            string methodName;
            var crsToSend = "0";

            if (isCrsOut)
            {
                methodName = request.TokenInExactAmount ? "SwapExactSrcForCrs" : "SwapSrcForExactCrs";

                if (methodName == "SwapExactSrcForCrs")
                {
                    // var amountCrsOutMin = request.TokenOutAmount.ToleranceAsSatoshis(request.Tolerance);
                    var amountCrsOutMin = request.TokenOutAmount;

                    parameters = new[]
                    {
                        request.TokenInAmount.ToSmartContractParameter(SmartContractParameterType.UInt256),
                        amountCrsOutMin.ToSmartContractParameter(SmartContractParameterType.UInt64),
                        request.TokenIn.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.Recipient.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64) // deadline
                    };
                }
                else // SwapSrcForExactCrs
                {
                    // var amountSrcInMax = request.TokenInAmount.ToleranceAsSatoshis(request.Tolerance);
                    var amountSrcInMax = request.TokenInAmount;

                    parameters = new[]
                    {
                        request.TokenOutAmount.ToSmartContractParameter(SmartContractParameterType.UInt64),
                        amountSrcInMax.ToSmartContractParameter(SmartContractParameterType.UInt256),
                        request.TokenIn.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.Recipient.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64) // deadline
                    };
                }
            }
            else if (isCrsIn)
            {
                methodName = request.TokenInExactAmount ? "SwapExactCrsForSrc" : "SwapCrsForExactSrc";
                crsToSend = request.TokenInAmount.InsertDecimal(TokenConstants.Cirrus.Decimals);

                if (methodName == "SwapExactCrsForSrc")
                {
                    // var amountSrcOutMin = request.TokenOutAmount.ToleranceAsSatoshis(request.Tolerance);
                    var amountSrcOutMin = request.TokenOutAmount;

                    parameters = new[]
                    {
                        amountSrcOutMin.ToSmartContractParameter(SmartContractParameterType.UInt256), // amountSrcOutMin
                        request.TokenOut.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.Recipient.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64) // deadline
                    };
                }
                else // SwapCrsForExactSrc
                {
                    parameters = new[]
                    {
                        request.TokenOutAmount.ToSmartContractParameter(SmartContractParameterType.UInt256), // amountSrcOut
                        request.TokenOut.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.Recipient.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64) // deadline
                    };
                }
            }
            else
            {
                methodName = request.TokenInExactAmount ? "SwapExactSrcForSrc" : "SwapSrcForExactSrc";

                if (methodName == "SwapExactSrcForSrc")
                {
                    // var amountSrcOutMin = request.TokenOutAmount.ToleranceAsSatoshis(request.Tolerance);
                    var amountSrcOutMin = request.TokenOutAmount;

                    parameters = new[]
                    {
                        request.TokenInAmount.ToSmartContractParameter(SmartContractParameterType.UInt256),
                        request.TokenIn.ToSmartContractParameter(SmartContractParameterType.Address),
                        amountSrcOutMin.ToSmartContractParameter(SmartContractParameterType.UInt256), // amountSrcOutMin
                        request.TokenOut.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.Recipient.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64) // deadline
                    };
                }
                else // SwapSrcForExactSrc
                {
                    // var amountSrcInMax = request.TokenInAmount.ToleranceAsSatoshis(request.Tolerance);
                    var amountSrcInMax = request.TokenInAmount;

                    parameters = new[]
                    {
                        amountSrcInMax.ToSmartContractParameter(SmartContractParameterType.UInt256), // amountSrcInMax
                        request.TokenIn.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.TokenOutAmount.ToSmartContractParameter(SmartContractParameterType.UInt256),
                        request.TokenOut.ToSmartContractParameter(SmartContractParameterType.Address),
                        request.Recipient.ToSmartContractParameter(SmartContractParameterType.Address),
                        0.ToSmartContractParameter(SmartContractParameterType.UInt64) // deadline
                    };
                }
            }

            var callDto = new SmartContractCallRequestDto(request.Router, request.WalletName, request.WalletAddress, request.WalletPassword,
                crsToSend, methodName, parameters);

            return _mediator.Send(new CallCirrusCallSmartContractMethodCommand(callDto), cancellationToken);
        }
    }
}