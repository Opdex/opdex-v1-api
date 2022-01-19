using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens;
using Opdex.Platform.Application.Abstractions.EntryQueries.Blocks;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Exceptions;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens;

public class CreateAddTokenCommandHandler : IRequestHandler<CreateAddTokenCommand, TokenDto>
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    public CreateAddTokenCommandHandler(IMapper mapper, IMediator mediator)
    {
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<TokenDto> Handle(CreateAddTokenCommand request, CancellationToken cancellationToken)
    {
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token, findOrThrow: false));
        if (!(token is null))
        {
            throw new TokenAlreadyIndexedException(request.Token);
        }

        var latestBlock = await _mediator.Send(new GetBestBlockReceiptQuery());

        StandardTokenContractSummary summary;
        try
        {
            summary = await _mediator.Send(new CallCirrusGetStandardTokenContractSummaryQuery(request.Token,
                                                                                              latestBlock.Height,
                                                                                              includeBaseProperties: true,
                                                                                              includeTotalSupply: true));
        }
        catch (Exception)
        {
            throw new InvalidDataException("token", "Unable to validate SRC token.");
        }

        token = new Token(request.Token, summary.Name, summary.Symbol, (int)summary.Decimals.Value, summary.Sats.Value,
                          summary.TotalSupply.Value, latestBlock.Height);

        var tokenId = await _mediator.Send(new MakeTokenCommand(token, latestBlock.Height));
        if (tokenId == 0) throw new Exception("Something went wrong when indexing the token.");

        return _mapper.Map<TokenDto>(token);
    }
}
