namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models;

public class OhlcEntity<T>
{
    public T Open { get; set; }
    public T High { get; set; }
    public T Low { get; set; }
    public T Close { get; set; }
}