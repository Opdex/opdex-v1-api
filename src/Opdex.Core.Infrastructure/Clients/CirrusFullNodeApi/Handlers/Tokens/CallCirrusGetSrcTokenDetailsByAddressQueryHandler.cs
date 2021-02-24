using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens
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
            //Todo: Check for validity here
            var name = await _smartContractsModule.GetContractStorageAsync(request.Address, "Name", "string", cancellationToken);
            if (!name.HasValue())
            {
                return null;
                // Todo: throw on anything other than a valid response
            }

            var ticker = await _smartContractsModule.GetContractStorageAsync(request.Address, "Symbol", "string", cancellationToken);
            
            // Todo: Generic type on GetContractStorageAsync - return correct type
            var decimalString = await _smartContractsModule.GetContractStorageAsync(request.Address, "Decimals", "uint", cancellationToken);
            var parseDecimalsSuccess = short.TryParse(decimalString, out var decimals);

            var decimalsValue = parseDecimalsSuccess ? decimals : (short) 8;
            
            // Todo: Get Total Supply

            return new Token(request.Address, name, ticker, decimalsValue, 100_000_000, 100_000_000);
        }
    }
}