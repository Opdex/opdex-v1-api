using System.Threading.Tasks;

namespace Opdex.Core.Application.Assemblers
{
    public interface IModelAssembler<in TSource, TDestination>
    {
        Task<TDestination> Assemble(TSource source);
    }
}