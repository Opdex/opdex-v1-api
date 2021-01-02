using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Common.Extensions;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Models;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Core.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;

namespace Opdex.Core.Infrastructure.Clients.CirrusFullNodeApi.Handlers.Tokens
{
    public class CallCirrusGetSmartContractTokenDetailsByAddressQueryHandler
        : IRequestHandler<CallCirrusGetSmartContractTokenDetailsByAddressQuery, TokenDto>
    {
        private readonly ISmartContractsModule _smartContractsModule;
        private readonly ILogger<CallCirrusGetSmartContractTokenDetailsByAddressQueryHandler> _logger;
        
        public CallCirrusGetSmartContractTokenDetailsByAddressQueryHandler(ISmartContractsModule smartContractsModule, 
            ILogger<CallCirrusGetSmartContractTokenDetailsByAddressQueryHandler> logger)
        {
            _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        // Todo: map the result dto to a domain model
        // Todo: TryCatch the module requests
        public async Task<TokenDto> Handle(CallCirrusGetSmartContractTokenDetailsByAddressQuery request, CancellationToken cancellationToken)
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

            // Todo: Should be a Token Domain model w/ validations
            return new TokenDto
            {
                Address = request.Address,
                Name = name,
                Ticker = ticker,
                Decimals = parseDecimalsSuccess ? decimals : (short)8
            };
        }
    }
}