using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Models.UInt;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens
{
    public class CallCirrusGetSrcTokenBalanceQueryHandler : IRequestHandler<CallCirrusGetSrcTokenBalanceQuery, UInt256>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetSrcTokenBalanceQueryHandler> _logger;

        private const string MethodName = StandardTokenConstants.Methods.GetBalance;

        public CallCirrusGetSrcTokenBalanceQueryHandler(ISmartContractsModule smartContractsModule, ILogger<CallCirrusGetSrcTokenBalanceQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<UInt256> Handle(CallCirrusGetSrcTokenBalanceQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var parameters = new[] { new SmartContractMethodParameter(request.Owner).Serialize() };
                var balanceRequest = new LocalCallRequestDto(request.Token, request.Owner, MethodName, parameters, request.BlockHeight);

                var response = await _smartContractsModule.LocalCallAsync(balanceRequest, cancellationToken);

                return response.DeserializeValue<UInt256>();
            }
            catch (Exception ex)
            {
                using (_logger.BeginScope(new Dictionary<string, object>{
                    ["Token"] = request.Token,
                    ["Owner"] = request.Owner,
                    ["BlockHeight"] = request.BlockHeight
                }))
                {
                    _logger.LogError(ex, "Unexpected error retrieving address balance.");
                }

                throw;
            }
        }
    }
}
