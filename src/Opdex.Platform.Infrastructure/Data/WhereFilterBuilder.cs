using System.Text;

namespace Opdex.Platform.Infrastructure.Data;

public class WhereFilterBuilder
{
    private readonly StringBuilder _stringBuilder;

    public WhereFilterBuilder()
    {
        _stringBuilder = new StringBuilder();
    }

    public WhereFilterBuilder AppendCondition(string condition)
    {
        var keyword = _stringBuilder.Length == 0 ? "WHERE" : "AND";
        _stringBuilder.Append(' ').Append(keyword).Append(' ').Append(condition);
        return this;
    }

    public override string ToString()
    {
        return _stringBuilder.ToString();
    }
}
