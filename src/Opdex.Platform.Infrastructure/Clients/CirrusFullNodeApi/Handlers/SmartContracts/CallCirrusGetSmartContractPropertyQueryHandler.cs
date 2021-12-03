using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Domain.Models.Transactions;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Modules;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Clients.CirrusFullNodeApi.Handlers.SmartContracts;

public class CallCirrusGetSmartContractPropertyQueryHandler : IRequestHandler<CallCirrusGetSmartContractPropertyQuery, SmartContractMethodParameter>
{
    private readonly ISmartContractsModule _smartContractsModule;
    private readonly ILogger<CallCirrusGetSmartContractPropertyQueryHandler> _logger;

    public CallCirrusGetSmartContractPropertyQueryHandler(ISmartContractsModule smartContractsModule,
                                                          ILogger<CallCirrusGetSmartContractPropertyQueryHandler> logger)
    {
        _smartContractsModule = smartContractsModule ?? throw new ArgumentNullException(nameof(smartContractsModule));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<SmartContractMethodParameter> Handle(CallCirrusGetSmartContractPropertyQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var result = await _smartContractsModule.GetContractStorageAsync(request.Contract, request.PropertyStateKey,
                                                                             request.PropertyType, request.BlockHeight, cancellationToken);

            return new SmartContractMethodParameter(result, request.PropertyType);
        }
        catch (Exception ex)
        {
            using (_logger.BeginScope(new Dictionary<string, object>{
                       ["Contract"] = request.Contract,
                       ["StateKey"] = request.PropertyStateKey,
                       ["PropertyType"] = request.PropertyType,
                       ["BlockHeight"] = request.BlockHeight
                   }))
            {
                _logger.LogError(ex, "Unexpected error retrieving smart contact property value.");
            }

            throw;
        }
    }
}