using Framework.Core.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Framework.Core.DataLayer
{
    public class DALFactory :IDALFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public DALFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public DAL CreateDAL<DAL>(IUnitOfWork unitOfWork) where DAL : IDALBase
        {
            var dal = _serviceProvider.GetService<DAL>();
            dal.InitUnitOfWork(unitOfWork);
            return dal;
        }
    }
}
