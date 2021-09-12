using Dapper;
using Opdex.Platform.Common.Models;
using System.Data;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.SqlMappers
{
    public class AddressHandler : SqlMapper.TypeHandler<Address>
    {
        public override void SetValue(IDbDataParameter parameter, Address value)
        {
            parameter.Value = value == Address.Empty ? null : value.ToString();
        }

        public override Address Parse(object value)
        {
            return new Address(value.ToString());
        }
    }
}
