using Dapper;
using Opdex.Platform.Common.Models;
using System.Data;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.SqlMappers;

public class Sha256Handler : SqlMapper.TypeHandler<Sha256>
{
    public override Sha256 Parse(object value)
    {
        return Sha256.Parse(value.ToString());
    }

    public override void SetValue(IDbDataParameter parameter, Sha256 value)
    {
        parameter.Value = value.ToString();
    }
}