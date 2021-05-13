using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens
{
    public class CallCirrusGetSrcTokenDetailsByAddressQueryHandler
        : IRequestHandler<CallCirrusGetSrcTokenDetailsByAddressQuery, Token>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetSrcTokenDetailsByAddressQueryHandler> _logger;
        
        public CallCirrusGetSrcTokenDetailsByAddressQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetSrcTokenDetailsByAddressQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // Todo: map the result dto to a domain model
        // Todo: TryCatch the module requests
        public async Task<Token> Handle(CallCirrusGetSrcTokenDetailsByAddressQuery request, CancellationToken cancellationToken)
        {
            var localCall = new LocalCallRequestDto(request.Address, request.Address, "get_Name", new string[0]);
            var nameResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var name = (string)nameResponse.Return;
            if (!name.HasValue()) return null;

            localCall.MethodName = "get_Symbol";
            var symbolResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var symbol = (string)symbolResponse.Return;
            if (!symbol.HasValue()) return null;

            localCall.MethodName = "get_Decimals";
            var decimalResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            if (!short.TryParse(decimalResponse.Return.ToString(), out var decimals)) return null;
            
            localCall.MethodName = "get_TotalSupply";
            var totalSupplyResponse = await _smartContractsModule.LocalCallAsync(localCall, CancellationToken.None);
            var totalSupply = totalSupplyResponse.Return.ToString();
            if (!totalSupply.HasValue()) return null;

            return new Token(request.Address, name, symbol, decimals, decimals.DecimalsToSatoshis(), totalSupply);
        }
    }
}