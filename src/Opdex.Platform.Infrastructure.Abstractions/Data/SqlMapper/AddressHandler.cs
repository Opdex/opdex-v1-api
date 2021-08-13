using Opdex.Platform.Common.Models;
using System.Data;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.SqlMapper
{
    public class AddressHandler : Dapper.SqlMapper.TypeHandler<Address>
    {
        public override void SetValue(IDbDataParameter parameter, Address value)
        {
            parameter.Value = value.ToString();
        }

        public override Address Parse(object value)
        {
            return new Address(value.ToString());
        }
    }
}
