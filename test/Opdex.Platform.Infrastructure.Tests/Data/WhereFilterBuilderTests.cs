using FluentAssertions;
using Opdex.Platform.Infrastructure.Data;
using Xunit;

namespace Opdex.Platform.Infrastructure.Tests.Data;

public class WhereFilterBuilderTests
{
    [Fact]
    public void AppendCondition_FirstCondition_UseWhere()
    {
        // Arrange
        var builder = new WhereFilterBuilder();

        // Act
        builder.AppendCondition("t.Id > 5");

        // Assert
        builder.ToString().Should().Be(" WHERE t.Id > 5");
    }

    [Fact]
    public void AppendCondition_SecondCondition_UseAnd()
    {
        // Arrange
        var builder = new WhereFilterBuilder();
        builder.AppendCondition("t.Id > 5");

        // Act
        builder.AppendCondition("t.Address = 'PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh'");

        // Assert
        builder.ToString().Should().EndWith(" AND t.Address = 'PVwyqbwu5CazeACoAMRonaQSyRvTHZvAUh'");
    }
}
