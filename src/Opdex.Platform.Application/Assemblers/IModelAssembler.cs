using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers
{
    public interface IModelAssembler<in TSource, TDestination>
    {
        Task<TDestination> Assemble(TSource source);
    }
}