using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Auth;

public class SelectAdminByAddressQueryHandler : IRequestHandler<SelectAdminByAddressQuery, Admin>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(AdminEntity.Id)},
                {nameof(AdminEntity.Address)}
            FROM admin
            WHERE {nameof(AdminEntity.Address)} = @{nameof(SqlParams.Address)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectAdminByAddressQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Admin> Handle(SelectAdminByAddressQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.Address), cancellationToken);

        var result = await _context.ExecuteFindAsync<AdminEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(Admin)} not found.");
        }

        return result == null ? null : _mapper.Map<Admin>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(Address address)
        {
            Address = address;
        }

        public Address Address { get; }
    }
}
