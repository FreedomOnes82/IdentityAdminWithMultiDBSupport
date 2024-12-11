using Dapper;
using Framework.Core.Abstractions;
using System.Data;

namespace Framework.Core.UOW
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(IDbConnection dbConn, IDataTablePrefixProvider dataTablePrefixProvider, 
            IUpdateCommandGroupProvider updateCmdGroupProvider)
        {
            _id = Guid.NewGuid();
            //_connectionAccessor = connectionAccessor;
            _dataTablePrefixProvider = dataTablePrefixProvider;
            //if (_dbConn == null)
            //    _dbConn = _connectionAccessor.CreateConnection();
            bool wasClosed = dbConn.State == ConnectionState.Closed;
            if (wasClosed)
                dbConn.Open();
            _dbConn = dbConn;
            _transaction = _dbConn.BeginTransaction();
            _updateCmdGroupProvider = updateCmdGroupProvider;
        }

        IDbConnectionAccessor _connectionAccessor = null;
        private readonly IDataTablePrefixProvider _dataTablePrefixProvider;
        IDbTransaction _transaction = null;
        private readonly IUpdateCommandGroupProvider _updateCmdGroupProvider;
        Guid _id = Guid.Empty;

        IDbConnection _dbConn;
        public IDbConnection Connection
        {
            get 
            { 
                return _dbConn;
            }
        }
        IDbTransaction IUnitOfWork.Transaction
        {
            get { return _transaction; }
        }
        Guid IUnitOfWork.Id
        {
            get { return _id; }
        }

        #region "operations"

        public IRepository<Model> GetRepository<Model>() where Model : class
        {
            return new Repository<Model>(this, _dataTablePrefixProvider, _updateCmdGroupProvider);
        }

        public async Task<IEnumerable<ResultModel>> ExecuteQuery<ResultModel>(string command, object parameter = null)
        {
            return await Connection.QueryAsync<ResultModel>(command, parameter);
        }
        #endregion

        public void SaveChanges()
        {
            try
            {
                _transaction.Commit();
                Dispose();
            }
            catch (Exception ex)
            {
                _transaction.Rollback();
                Dispose();
                throw;
            }
        }
       

        public void Dispose()
        {
            if (_transaction != null)
                _transaction.Dispose();
            _transaction = null;
            _dbConn.Close();
            //if (_dbConn.GetType().FullName.Contains("mysql", StringComparison.OrdinalIgnoreCase))
            //    _dbConn.Dispose();
            foreach (var service in _relatedServiceObjects)
            {
                service.ClearUnitOfWork();
            }
        }

        public IPartitionRepository<Model> GetPartitionRepository<Model>() where Model : PartitionModelBase
        {
            return new PartitionedRepository<Model>(this, _dataTablePrefixProvider, _updateCmdGroupProvider);
        }

        private List<IServiceBase> _relatedServiceObjects = new List<IServiceBase>();
        public void AddReferredService(IServiceBase service)
        {
            _relatedServiceObjects.Add(service);
        }
    }
}
