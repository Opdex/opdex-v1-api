using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Application.Abstractions.Models;
using Opdex.Platform.Application.Abstractions.Queries;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;

namespace Opdex.Platform.Application.Handlers
{
    public class RetrieveAllTokensQueryHandler : IRequestHandler<RetrieveAllTokensQuery, IEnumerable<TokenDto>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public RetrieveAllTokensQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<TokenDto>> Handle(RetrieveAllTokensQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectAllTokensQuery();
            
            var tokens = await _mediator.Send(query, cancellationToken);

            return _mapper.Map<IEnumerable<TokenDto>>(tokens);
        }
    }
}