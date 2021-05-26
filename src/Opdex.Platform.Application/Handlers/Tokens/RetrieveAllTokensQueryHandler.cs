using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.Models;
using Opdex.Platform.Domain.Models.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens;

namespace Opdex.Platform.Application.Handlers.Tokens
{
    public class RetrieveAllTokensQueryHandler : IRequestHandler<RetrieveAllTokensQuery, IEnumerable<Token>>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        
        public RetrieveAllTokensQueryHandler(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Token>> Handle(RetrieveAllTokensQuery request, CancellationToken cancellationToken)
        {
            var query = new SelectAllTokensQuery();
            
            return await _mediator.Send(query, cancellationToken);
        }
    }
}