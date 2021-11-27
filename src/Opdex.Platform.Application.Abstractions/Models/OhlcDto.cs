namespace Opdex.Platform.Application.Abstractions.Models
{
    public class OhlcDto<T>
    {
        public T Open { get; set; }
        public T High { get; set; }
        public T Low { get; set; }
        public T Close { get; set; }
    }
}
