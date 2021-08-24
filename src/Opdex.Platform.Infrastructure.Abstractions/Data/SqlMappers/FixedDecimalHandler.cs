using Dapper;
using Opdex.Platform.Common.Models;
using System.Data;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.SqlMappers
{
    public class FixedDecimalHandler : SqlMapper.TypeHandler<FixedDecimal>
    {
        public override FixedDecimal Parse(object value)
        {
            return FixedDecimal.Parse(value.ToString());
        }

        public override void SetValue(IDbDataParameter parameter, FixedDecimal value)
        {
            parameter.Value = value.ToString();
        }
    }
}
