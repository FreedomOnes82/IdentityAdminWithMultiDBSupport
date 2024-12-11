using Dapper;
using Framework.Core.Abstractions;
using Framework.Core.UOW;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Core.Blazor.Admin.SqlServer.Auth.Providers
{
    internal class Base
    {
        protected IDbConnectionAccessor _dbConnectionAccessor;
        protected IDataTablePrefixProvider _tableNamePrefixProvider;
        protected IUpdateCommandGroupProvider _updateCommandGroupProvider;
        protected IUnitOfWork _unitOfWork = null;
        public Base(IDbConnectionAccessor dbConnectionAccessor, IDataTablePrefixProvider tableNamePrefixProvider)
        {
            _dbConnectionAccessor = dbConnectionAccessor;
            _tableNamePrefixProvider = tableNamePrefixProvider;
        }
        protected IDbConnection DbConnection
        {
            get
            {
                if (_unitOfWork != null)
                {
                    return _unitOfWork.Connection;
                }
                var dbConn = _dbConnectionAccessor.CreateConnection();

                return dbConn;
            }
        }

        protected IDbTransaction Transaction
        {
            get
            {
                if (_unitOfWork != null)
                {
                    return _unitOfWork.Transaction;
                }
                return null;
            }
        }

        protected IRepository<T> GetRepository<T>() where T : class
        {
            if (_unitOfWork != null)
                return _unitOfWork.GetRepository<T>();
            return new Repository<T>(_dbConnectionAccessor.CreateConnection(),
                _tableNamePrefixProvider, _updateCommandGroupProvider);
        }

        //public async Task<IEnumerable<Model>> QueryAsync<Model>(string sql)
        //{
        //    return await DbConnection.QueryAsync<Model>(sql, null, Transaction);
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Model"></typeparam>
        /// <returns></returns>
        protected IPartitionRepository<Model> GetPartitionRepository<Model>() where Model : PartitionModelBase
        {
            if (_unitOfWork != null)
                return _unitOfWork.GetPartitionRepository<Model>();
            return new PartitionedRepository<Model>(_dbConnectionAccessor.CreateConnection(),
                _tableNamePrefixProvider, _updateCommandGroupProvider);
        }

        public void InitUnitOfWork(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            OnUnitWorkInited();
        }

        public virtual void OnUnitWorkInited()
        {
        }

    }
}
