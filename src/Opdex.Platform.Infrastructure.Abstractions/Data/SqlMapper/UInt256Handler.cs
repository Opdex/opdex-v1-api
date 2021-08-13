using Opdex.Platform.Common.Models.UInt;
using System.Data;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.SqlMapper
{
    public class UInt256Handler : Dapper.SqlMapper.TypeHandler<UInt256>
    {
        public override void SetValue(IDbDataParameter parameter, UInt256 value)
        {
            parameter.Value = value.ToString();
        }

        public override UInt256 Parse(object value)
        {
            return new UInt256(value.ToString());
        }
    }
}
