using Framework.Core.Abstractions;

namespace Framework.Core.Abstractions
{
    public interface IDALBase
    {
        void InitUnitOfWork(IUnitOfWork unitOfWork);
    }
}
