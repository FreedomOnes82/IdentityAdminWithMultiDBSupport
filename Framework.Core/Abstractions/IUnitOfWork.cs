using Framework.Core.UOW;
using System.Data;

namespace Framework.Core.Abstractions
{
    public interface IUnitOfWork : IDisposable
    {
        Guid Id { get; }
        IDbConnection Connection { get; }
        IDbTransaction Transaction { get; }

        IRepository<Model> GetRepository<Model>() where Model : class;

        IPartitionRepository<Model> GetPartitionRepository<Model>() where Model : PartitionModelBase;
        Task<IEnumerable<ResultModel>> ExecuteQuery<ResultModel>(string command, object parameter = null);

        void SaveChanges();

        void AddReferredService(IServiceBase service);
    }

}
