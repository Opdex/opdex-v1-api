using System;
using System.Data;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.SqlMapper
{
    public class DateTimeHandler : Dapper.SqlMapper.TypeHandler<DateTime>
    {
        public override void SetValue(IDbDataParameter parameter, DateTime value)
        {
            parameter.Value = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        public override DateTime Parse(object value)
        {
            return DateTime.SpecifyKind((DateTime)value, DateTimeKind.Utc).ToUniversalTime();
        }
    }
}
