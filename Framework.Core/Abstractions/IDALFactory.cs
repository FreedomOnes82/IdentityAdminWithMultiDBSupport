using Framework.Core.Abstractions;

namespace Framework.Core.Abstractions
{
    public interface IDALFactory 
    {
        DAL CreateDAL<DAL>(IUnitOfWork unitOfWork) where DAL : IDALBase;
    }
}
