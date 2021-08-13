using Dapper;
using Opdex.Platform.Common.Models.UInt;
using System.Data;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.SqlMappers
{
    public class UInt128Handler : SqlMapper.TypeHandler<UInt128>
    {
        public override void SetValue(IDbDataParameter parameter, UInt128 value)
        {
            parameter.Value = value.ToString();
        }

        public override UInt128 Parse(object value)
        {
            return new UInt128(value.ToString());
        }
    }
}
